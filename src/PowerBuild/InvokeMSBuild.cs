using System;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace PowerBuild
{
    [OutputType(typeof(MSBuildResult))]
    [Cmdlet("Invoke", "MSBuild")]
    public class InvokeMSBuild : PSCmdlet
    {
        private AppDomain _appDomain;

        private MSBuildHelper _msBuildHelper;

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
            var cmdletHelper = new CmdletHelper();
            _msBuildHelper.Project = Project;
            _msBuildHelper.Verbosity = Verbosity;
            _msBuildHelper.ToolVersion = ToolVersion;
            _msBuildHelper.Target = Target;
            _msBuildHelper.CmdletHelper = cmdletHelper;
            _msBuildHelper.Loggers = new ILogger[]
            {
                new CrossDomainLogger(new PowerShellLogger(Verbosity, cmdletHelper, Host.UI.RawUI.ForegroundColor))
            };

            var asyncResult = _msBuildHelper.BeginProcessRecord(null, null);

            foreach (var messageContainer in cmdletHelper.ConsumeBuildEvents())
            {
                if (messageContainer.BuildEvent is BuildErrorEventArgs)
                {
                    WriteError(
                        new ErrorRecord(new Exception(messageContainer.FormattedMessage), 
                        ((BuildErrorEventArgs)messageContainer.BuildEvent).Code,
                        ErrorCategory.NotSpecified, 
                        messageContainer.BuildEvent));
                }
                else if (messageContainer.BuildEvent is BuildWarningEventArgs)
                {
                    WriteWarning(messageContainer.FormattedMessage);
                }
                else
                {
                    Host.UI.Write(messageContainer.Color, Host.UI.RawUI.BackgroundColor, messageContainer.FormattedMessage);
                }
            }

            var results = _msBuildHelper.EndProcessRecord(asyncResult);

            WriteObject(results, true);
        }

        protected override void StopProcessing()
        {
            WriteDebug("Stop processing");
            _msBuildHelper.StopProcessing();
            base.StopProcessing();
        }
    }
}