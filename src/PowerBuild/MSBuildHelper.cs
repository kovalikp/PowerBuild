// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Build.Execution;
    using Microsoft.Build.Framework;

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

        public IEnumerable<BuildResult> EndProcessRecord(IAsyncResult asyncResult)
        {
            return MarshalTask.GetResult<IEnumerable<BuildResult>>(asyncResult);
        }

        public async Task<IEnumerable<BuildResult>> ProcessRecordAsync()
        {
            var results = new List<BuildResult>();
            try
            {
                var parameters = new BuildParameters
                {
                    Loggers = Loggers,
                    MaxNodeCount = Parameters.MaxCpuCount,
                    DetailedSummary = Parameters.DetailedSummary,
                    DefaultToolsVersion = Parameters.ToolsVersion,
                    EnableNodeReuse = Parameters.NodeReuse
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

        public void EndProcessing()
        {
            _processingCancellationTokenSource?.Dispose();
            _buildManager?.Dispose();
            _processingCancellationTokenSource = null;
            _buildManager = null;
        }

        private BuildResult MapBuildResult(string project, Microsoft.Build.Execution.BuildResult buildResult)
        {
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
                    Items = x.Value.Items.Select(y => (ITaskItem)new TaskItem(y)).ToArray()
                }))
            };
        }
    }
}