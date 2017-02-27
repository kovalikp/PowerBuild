// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using Logging;
    using Microsoft.Build.CommandLine;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Use MSBuild to build a project.
    /// </summary>
    /// <para type="synopsis">
    /// Use MSBuild to build a project.
    /// </para>
    /// <para type="description">
    /// Builds the specified targets in the project file. If a project file is not specified, MSBuild searches
    /// the current working directory for a file that has a file extension that ends in "proj" and uses that file.
    /// </para>
    /// <example>
    ///   <code>Invoke-MSBuild -Project Project.sln -Target Build -Property @{Configuration=&quot;Release&quot;} -Verbosity Minimal</code>
    /// </example>
    [OutputType(typeof(BuildResult))]
    [Alias("msbuild")]
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

        /// <summary>
        /// Gets or sets detailed summary parameter.
        /// </summary>
        /// <para type="description">
        /// Shows detailed information at the end of the build about the configurations built and how they were scheduled to nodes.
        /// </para>
        [Parameter]
        [Alias("ds")]
        public SwitchParameter DetailedSummary { get; set; }

        /// <summary>
        /// Get or sets project extensions to ignore.
        /// </summary>
        /// <para type="description">
        /// List of extensions to ignore when determining which project file to build.
        /// </para>
        [Parameter]
        [Alias("ignore")]
        public string[] IgnoreProjectExtensions { get; set; }

        /// <summary>
        /// Get or sets logger collection.
        /// </summary>
        /// <para type="description">
        /// Use this loggers to log events from MSBuild.
        /// </para>
        [Parameter]
        [Alias("l")]
        public ILogger[] Logger { get; set; }

        /// <summary>
        /// Gets or sets number of concurrent processes to build with.
        /// </summary>
        /// <para type="description">
        /// Specifies the maximum number of concurrent processes to build with. If the switch is not used, the default
        /// value used is 1. If the switch is used with a $null, value MSBuild will use up to the number of processors
        /// on the computer.
        /// </para>
        [Parameter]
        [AllowNull]
        [Alias("m")]
        public int? MaxCpuCount { get; set; } = 1;

        /// <summary>
        /// Gets or sets node reuse.
        /// </summary>
        /// <para type="description">
        /// Enables or Disables the reuse of MSBuild nodes.
        /// </para>
        [Parameter]
        [Alias("nr")]
        public bool? NodeReuse { get; set; } = null;

        /// <summary>
        /// Gets or sets project to build.
        /// </summary>
        /// <para type="description">
        /// Project to build.
        /// </para>
        [Alias("FullName")]
        [Parameter(
            Position = 0,
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Project { get; set; }

        /// <summary>
        /// Gets or sets properties.
        /// </summary>
        /// <para type="description">
        /// Set or override these project-level properties.
        /// </para>
        [Alias("p")]
        [Parameter]
        public Hashtable Property { get; set; }

        /// <summary>
        /// Gets or sets targets to build.
        /// </summary>
        /// <para type="description">
        /// Build these targets in the project.
        /// </para>
        [Parameter(Position = 1, Mandatory = false)]
        [Alias("t")]
        public string[] Target { get; set; }

        /// <summary>
        /// Gets or sets tools version.
        /// </summary>
        /// <para type="description">
        /// The version of the MSBuild Toolset (tasks, targets, etc.) to use during build.This version will override
        /// the versions specified by individual projects.
        /// </para>
        [Parameter]
        [ValidateSet("4.0", "12.0", "14.0")]
        [Alias("tv")]
        public string ToolsVersion { get; set; } = "14.0";

        /// <summary>
        /// Gets or sets logging verbosity.
        /// </summary>
        /// <para type="description">
        /// Display this amount of information in the event log.
        /// </para>
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

            var properties = Property?.Cast<DictionaryEntry>().ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
            properties = properties ?? new Dictionary<string, string>();

            var projects = string.IsNullOrEmpty(Project)
                ? new[] { SessionState.Path.CurrentFileSystemLocation.Path }
                : new[] { Project };
            var project = MSBuildApp.ProcessProjectSwitch(projects, IgnoreProjectExtensions, Directory.GetFiles);

            _msBuildHelper.Parameters = new InvokeMSBuildParameters
            {
                Project = project,
                Verbosity = Verbosity,
                ToolsVersion = ToolsVersion,
                Target = Target,
                MaxCpuCount = MaxCpuCount ?? Environment.ProcessorCount,
                NodeReuse = NodeReuse ?? Environment.GetEnvironmentVariable("MSBUILDDISABLENODEREUSE") != "1",
                Properties = properties,
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