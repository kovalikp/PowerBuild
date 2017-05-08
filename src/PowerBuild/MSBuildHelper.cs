// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Execution;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Shared;

    internal class MSBuildHelper : MarshalByRefObject
    {
        private BuildManager _buildManager;

        private CancellationTokenSource _processingCancellationTokenSource;

        public IEnumerable<ILogger> Loggers { get; set; } = Enumerable.Empty<ILogger>();

        public InvokeMSBuildParameters Parameters { get; set; }

        public void BeginProcessing()
        {
            _processingCancellationTokenSource = new CancellationTokenSource();
            _buildManager = new BuildManager();
        }

        public IAsyncResult BeginProcessRecord(AsyncCallback callback, object state)
        {
            return MarshalTask.FromTask(ProcessRecordAsync(), callback, state);
        }

        public void EndProcessing()
        {
            _processingCancellationTokenSource?.Dispose();
            _buildManager?.Dispose();
            _processingCancellationTokenSource = null;
            _buildManager = null;
        }

        public IEnumerable<BuildResult> EndProcessRecord(IAsyncResult asyncResult)
        {
            return MarshalTask.GetResult<IEnumerable<BuildResult>>(asyncResult);
        }

        public IEnumerable<string> GetTargets()
        {
            Project project = null;
            try
            {
                var projectFile = FileUtilities.NormalizePath(Parameters.Project);
                project = new Project(projectFile, Parameters.Properties, Parameters.ToolsVersion);
                return project.Targets.Select(x => x.Key).ToArray();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
            finally
            {
                if (project != null)
                {
                    ProjectCollection.GlobalProjectCollection.UnloadProject(project);
                }
            }
        }

        public void PreprocessProject(string projectFile, IDictionary<string, string> globalProperties, string toolsVersion, TextWriter preprocessWriter)
        {
            projectFile = FileUtilities.NormalizePath(projectFile);
            var projectCollection = new ProjectCollection(ToolsetDefinitionLocations.Default);
            Project project = null;
            try
            {
                project = projectCollection.LoadProject(projectFile, globalProperties, toolsVersion);
                project.SaveLogicalProject(preprocessWriter);
            }
            finally
            {
                if (project != null)
                {
                    projectCollection.UnloadProject(project);
                }
            }
        }

        public async Task<IEnumerable<BuildResult>> ProcessRecordAsync()
        {
            var results = new List<BuildResult>();
            try
            {
                bool logTaskInputs = Parameters.Verbosity == LoggerVerbosity.Diagnostic;

                if (!logTaskInputs)
                {
                    foreach (var logger in Loggers)
                    {
                        if ((logger.Parameters != null &&
                            (logger.Parameters.IndexOf("V=DIAG", StringComparison.OrdinalIgnoreCase) != -1 ||
                            logger.Parameters.IndexOf("VERBOSITY=DIAG", StringComparison.OrdinalIgnoreCase) != -1)) ||
                            logger.Verbosity == LoggerVerbosity.Diagnostic)
                        {
                            logTaskInputs = true;
                            break;
                        }
                    }
                }

                var parameters = new BuildParameters
                {
                    Loggers = Loggers,
                    MaxNodeCount = Parameters.MaxCpuCount,
                    DetailedSummary = Parameters.DetailedSummary,
                    DefaultToolsVersion = Parameters.ToolsVersion,
                    EnableNodeReuse = Parameters.NodeReuse,
                    WarningsAsErrors = Parameters.WarningsAsErrors,
                    WarningsAsMessages = Parameters.WarningsAsMessages,
                    LogTaskInputs = logTaskInputs,
                    NodeExeLocation = Factory.GetMSBuildPath()
                };

                _buildManager.BeginBuild(parameters);
                try
                {
                    var targetsToBuild = Parameters.Target ?? new string[0];

                    var requestData = new BuildRequestData(Parameters.Project, Parameters.Properties, Parameters.ToolsVersion, targetsToBuild, null);
                    var submission = _buildManager.PendBuildRequest(requestData);

                    var buildResult = await submission.ExecuteAsync();
                    var partialResult = MapBuildResult(Parameters.Project, buildResult);
                    results.Add(partialResult);
                }
                finally
                {
                    _buildManager.EndBuild();
                }
            }
            catch (OperationCanceledException)
            {
            }

            return results.ToArray();
        }

        public void StopProcessing()
        {
            _buildManager.CancelAllSubmissions();
            _processingCancellationTokenSource.Cancel();
        }

        private BuildResult MapBuildResult(string project, Microsoft.Build.Execution.BuildResult buildResult)
        {
            var escapedProject = EscapingUtilities.Escape(project);
            return new BuildResult
            {
                Project = project,
                Exception = buildResult.Exception,
                CircularDependency = buildResult.CircularDependency,
                ConfigurationId = buildResult.ConfigurationId,
                GlobalRequestId = buildResult.GlobalRequestId,
                NodeRequestId = buildResult.NodeRequestId,
                OverallResult = buildResult.OverallResult,
                ParentGlobalRequestId = buildResult.ParentGlobalRequestId,
                ResultsByTarget = new ResultsByTarget(buildResult.ResultsByTarget.ToDictionary(x => x.Key, x => new TargetResult
                {
                    Name = x.Key,
                    Exception = x.Value.Exception,
                    ResultCode = x.Value.ResultCode,
                    Items = x.Value.Items.Cast<ITaskItem2>().Select(
                        y => new TaskItem(
                            y.EvaluatedIncludeEscaped,
                            escapedProject,
                            ToGeneric(y.CloneCustomMetadataEscaped())))
                        .ToArray()
                }))
            };
        }

        private Dictionary<string, string> ToGeneric(IDictionary dictionary)
        {
            return dictionary.Keys.Cast<object>().ToDictionary(x => x.ToString(), x => dictionary[x].ToString());
        }
    }
}