// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Management.Automation;
    using Logging;
    using Microsoft.Build.Framework;

    [OutputType(typeof(BuildResult))]
    [Cmdlet("Invoke", "MSBuild")]
    public class InvokeMSBuild : PSCmdlet
    {
        private MSBuildHelper _msBuildHelper;

        [Parameter]
        [Alias("dl")]
        public DefaultLoggerType DefaultLogger { get; set; } = DefaultLoggerType.Streams;

        [Parameter]
        [Alias("dlp")]
        public string DefaultLoggerParameters { get; set; }

        [Parameter]
        [Alias("ds")]
        public SwitchParameter DetailedSummary { get; set; }

        [Parameter]
        [Alias("l")]
        public ILogger[] Logger { get; set; }

        [Parameter]
        [AllowNull]
        [Alias("m")]
        public int? MaxCpuCount { get; set; } = 1;

        [Parameter]
        [Alias("nr")]
        public bool NodeReuse { get; set; }

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
        [Alias("t")]
        public string[] Target { get; set; }

        [Parameter]
        [ValidateSet("4.0", "12.0", "14.0")]
        [Alias("tv")]
        public string ToolsVersion { get; set; }

        [Parameter]
        [Alias("v")]
        public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;

        protected override void BeginProcessing()
        {
            WriteDebug("Begin processing");
            base.BeginProcessing();
            _msBuildHelper = Factory.InvokeInstance.CreateMSBuildHelper();
            _msBuildHelper.BeginProcessing();
        }

        protected override void EndProcessing()
        {
            WriteDebug("End processing");
            _msBuildHelper.StopProcessing();
            _msBuildHelper = null;
            base.EndProcessing();
        }

        protected override void ProcessRecord()
        {
            WriteDebug("Process record");
            _msBuildHelper.Parameters = new InvokeMSBuildParameters
            {
                Project = Project,
                Verbosity = Verbosity,
                ToolsVersion = ToolsVersion,
                Target = Target,
                MaxCpuCount = MaxCpuCount,
                NodeReuse = NodeReuse,
                DetailedSummary = DetailedSummary || Verbosity == LoggerVerbosity.Diagnostic
            };

            var loggers = new List<ILogger>();
            IPowerShellLogger powerShellLogger;
            switch (DefaultLogger)
            {
                case DefaultLoggerType.Streams:
                    powerShellLogger = new StreamsLogger(Verbosity, this);
                    break;

                case DefaultLoggerType.Host:
                    powerShellLogger = new ConsoleLogger(Verbosity, Host);
                    break;

                case DefaultLoggerType.None:
                    powerShellLogger = null;
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }

            if (Logger != null)
            {
                loggers.AddRange(Logger);
            }

            if (powerShellLogger != null)
            {
                loggers.Add(powerShellLogger);
            }

            var crossDomainLoggers = (
                from unknownLogger in loggers
                group unknownLogger by unknownLogger is MarshalByRefObject
                into marshalByRefLogger
                from logger in MakeLoggersCrossDomain(marshalByRefLogger.Key, marshalByRefLogger)
                select logger).ToArray();

            _msBuildHelper.Loggers = crossDomainLoggers;

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
            _msBuildHelper = null;
            base.StopProcessing();
        }

        private IEnumerable<ILogger> MakeLoggersCrossDomain(bool isMarshalByRef, IEnumerable<ILogger> loggers)
        {
            return isMarshalByRef ? loggers : new[] { new CrossDomainLogger(loggers) };
        }
    }
}