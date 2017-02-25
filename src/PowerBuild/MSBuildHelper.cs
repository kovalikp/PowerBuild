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

    public class MSBuildHelper : MarshalByRefObject
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

        public IEnumerable<MSBuildResult> EndProcessRecord(IAsyncResult asyncResult)
        {
            return MarshalTask.GetResult<IEnumerable<MSBuildResult>>(asyncResult);
        }

        public async Task<IEnumerable<MSBuildResult>> ProcessRecordAsync()
        {
            var results = Enumerable.Empty<MSBuildResult>();
            try
            {
                var parameters = new BuildParameters
                {
                    Loggers = Loggers,
                    MaxNodeCount = Parameters.MaxCpuCount ?? Environment.ProcessorCount,
                    DetailedSummary = Parameters.DetailedSummary,
                    DefaultToolsVersion = Parameters.ToolsVersion,
                    EnableNodeReuse = Parameters.NodeReuse
                };

                foreach (var project in Parameters.Project)
                {
                    _buildManager.BeginBuild(parameters);
                    try
                    {
                        IDictionary<string, string> globalProperties = new Dictionary<string, string>();
                        var targetsToBuild = Parameters.Target ?? new string[0];

                        var requestData = new BuildRequestData(project, globalProperties, Parameters.ToolsVersion, targetsToBuild, null);
                        var submission = _buildManager.PendBuildRequest(requestData);

                        var buildResult = await submission.ExecuteAsync();
                        var partialResults = GetMSBuildResults(buildResult).ToArray();
                        results = results.Union(partialResults);
                    }
                    finally
                    {
                        _buildManager.EndBuild();
                    }
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

        protected void EndProcessing()
        {
            _processingCancellationTokenSource?.Dispose();
            _buildManager?.Dispose();
            _processingCancellationTokenSource = null;
            _buildManager = null;
        }

        private IEnumerable<MSBuildResult> GetMSBuildResults(BuildResult buildResult)
        {
            return
                from resultByTarget in buildResult.ResultsByTarget
                let target = resultByTarget.Key
                let targetResult = resultByTarget.Value
                select new MSBuildResult
                {
                    Target = target,
                    ResultCode = targetResult.ResultCode,
                    Items = targetResult.Items
                };
        }
    }
}