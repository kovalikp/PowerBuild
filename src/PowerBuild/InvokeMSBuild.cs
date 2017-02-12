namespace PowerBuild
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Management.Automation;
    using System.Reflection;
    using Logging;
    using Microsoft.Build.Framework;

    [OutputType(typeof(MSBuildResult))]
    [Cmdlet("Invoke", "MSBuild")]
    public class InvokeMSBuild : PSCmdlet
    {
        private AppDomain _appDomain;

        private MSBuildHelper _msBuildHelper;

        [Parameter]
        public DefaultLoggerType DefaultLogger { get; set; } = DefaultLoggerType.Streams;

        [Parameter(
                    Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Project to build.")]
        [ValidateNotNullOrEmpty]
        public string[] Project { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = false,
            HelpMessage = "Target to build.")]
        public string[] Target { get; set; }

        [Parameter]
        [ValidateSet("4.0", "12.0", "14.0")]
        public string ToolVersion { get; set; }

        [Parameter]
        public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;

        protected override void BeginProcessing()
        {
            WriteDebug("Begin processing");
            base.BeginProcessing();
            var configurationPath = Assembly.GetExecutingAssembly().Location + ".config";
            WriteDebug($"Configuration path: {configurationPath}");
            _msBuildHelper = MSBuildHelper.CreateCrossDomain(configurationPath, out _appDomain);
            _msBuildHelper.BeginProcessing();
        }

        protected override void EndProcessing()
        {
            WriteDebug("End processing");
            _msBuildHelper.StopProcessing();
            _msBuildHelper = null;
            if (_appDomain != null)
            {
                AppDomain.Unload(_appDomain);
                _appDomain = null;
            }

            base.EndProcessing();
        }

        protected override void ProcessRecord()
        {
            WriteDebug("Process record");
            _msBuildHelper.Project = Project;
            _msBuildHelper.Verbosity = Verbosity;
            _msBuildHelper.ToolVersion = ToolVersion;
            _msBuildHelper.Target = Target;

            var loggers = new List<ILogger>();
            IPowerShellLogger powerShellLogger;
            switch (DefaultLogger)
            {
                case DefaultLoggerType.Streams:
                    powerShellLogger = new StreamsLogger(Verbosity, this);
                    loggers.Add(powerShellLogger);
                    break;

                case DefaultLoggerType.Host:
                    powerShellLogger = new HostLogger(Verbosity, this);
                    loggers.Add(powerShellLogger);
                    break;

                case DefaultLoggerType.None:
                    powerShellLogger = null;
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }

            _msBuildHelper.Loggers = new ILogger[]
            {
                new CrossDomainLogger(loggers)
            };

            try
            {
                var asyncResult = _msBuildHelper.BeginProcessRecord(null, null);
                powerShellLogger?.WriteEvents();
                var results = _msBuildHelper.EndProcessRecord(asyncResult);
                WriteObject(results, true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ProcessRecordError", ErrorCategory.NotSpecified, null));
            }
        }

        protected override void StopProcessing()
        {
            WriteDebug("Stop processing");
            _msBuildHelper.StopProcessing();
            base.StopProcessing();
        }
    }
}