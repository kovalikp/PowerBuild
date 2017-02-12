using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace PowerBuild
{
    public class MSBuildHelper : MarshalByRefObject
    {
        private BuildManager _buildManager;

        private CancellationTokenSource _processingCancellationTokenSource;

        public MSBuildHelper()
        {
            _buildManager = new BuildManager();
        }

        public IEnumerable<ILogger> Loggers { get; set; } = Enumerable.Empty<ILogger>();

        public string[] Project { get; set; }

        public string[] Target { get; set; }

        public string ToolsVersion { get; set; }

        public int MaxCpuCount { get; set; }

        public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;

        public bool NodeReuse { get; set; }

        public static MSBuildHelper CreateCrossDomain(string configurationFile, out AppDomain appDomain)
        {
            var appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = Path.GetDirectoryName(typeof(MSBuildHelper).Assembly.Location);
            appDomainSetup.ConfigurationFile = configurationFile;
            appDomain = AppDomain.CreateDomain("invoke-msbuild", AppDomain.CurrentDomain.Evidence, appDomainSetup);

            return (MSBuildHelper)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(MSBuildHelper).FullName);
        }

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
                    MaxNodeCount = MaxCpuCount,
                    DefaultToolsVersion = ToolsVersion,
                    EnableNodeReuse = NodeReuse,

                };

                foreach (var project in Project)
                {
                    _buildManager.BeginBuild(parameters);
                    try
                    {
                        IDictionary<string, string> globalProperties = new Dictionary<string, string>();
                        var targetsToBuild = Target ?? new string[0];

                        var requestData = new BuildRequestData(project, globalProperties, ToolsVersion, targetsToBuild, null);
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
            catch (Exception ex)
            {
            }
            finally
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