// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.Management.Automation;
    using Logging;
    using Microsoft.Build.Framework;

    /// <para type="synopsis">
    /// Create new file logger.
    /// </para>
    /// <para type="description">
    /// Create new configured file logger. Parameters are equivalent of msbuild.exe's /fileloggerparameters:&lt;parameters&gt; switch.
    /// </para>
    /// <example>
    ///   <code>New-FileLogger -Verbosity Normal -PerformanceSummary</code>
    /// </example>
    [OutputType(typeof(ILogger))]
    [Cmdlet(VerbsCommon.New, "FileLogger")]
    public class NewFileLogger : PSCmdlet
    {
        /// <summary>
        /// </summary>
        /// <para type="description">
        /// Determines if the build log will be appended to or overwrite the log file.Setting the switch appends the build log to the log file; Not setting the switch overwrites the contents of an existing log file. The default is not to append to the log file.
        /// </para>
        [Parameter]
        public SwitchParameter Append { get; set; }

        /// <para type="description">
        /// Use the default console colors for all logging messages.
        /// </para>
        [Parameter]
        public SwitchParameter DisableConsoleColor { get; set; }

        /// <para type="description">
        /// Disable the multiprocessor logging style of output when running in non - multiprocessor mode.
        /// </para>
        [Parameter]
        public SwitchParameter DisableMPLogging { get; set; }

        /// <para type="description">
        /// Enable the multiprocessor logging style even when running in non - multiprocessor mode.This logging style is on by default.
        /// </para>
        [Parameter]
        public SwitchParameter EnableMPLogging { get; set; }

        /// <para type="description">
        /// Specifies the encoding for the file, for example, UTF-8, Unicode, or ASCII
        /// </para>
        [Parameter]
        public string Encoding { get; set; }

        /// <para type="description">
        /// Show only errors.
        /// </para>
        [Parameter]
        public SwitchParameter ErrorsOnly { get; set; }

        /// <para type="description">
        /// Use ANSI console colors even if console does not support it.
        /// </para>
        [Parameter]
        public SwitchParameter ForceConsoleColor { get; set; }

        /// <para type="description">
        /// Does not align the text to the size of the console buffer.
        /// </para>
        [Parameter]
        public SwitchParameter ForceNoAlign { get; set; }

        /// <para type="description">
        /// Path to the log file into which the build log will be written.
        /// </para>
        [Parameter(Position = 0, Mandatory = true)]
        public string LogFile { get; set; }

        /// <para type="description">
        /// Don't show list of items and properties at the start of each project build.
        /// </para>
        [Parameter]
        public SwitchParameter NoItemAndPropertyList { get; set; }

        /// <para type="description">
        /// Don't show error and warning summary at the end.
        /// </para>
        [Parameter]
        public SwitchParameter NoSummary { get; set; }

        /// <para type="description">
        /// Show time spent in tasks, targets and projects.
        /// </para>
        [Parameter]
        public SwitchParameter PerformanceSummary { get; set; }

        /// <para type="description">
        /// Show TaskCommandLineEvent messages.
        /// </para>
        [Parameter]
        public SwitchParameter ShowCommandLine { get; set; }

        /// <para type="description">
        /// Show eventId for started events, finished events, and messages.
        /// </para>
        [Parameter]
        public SwitchParameter ShowEventId { get; set; }

        /// <para type="description">
        /// Display the Timestamp as a prefix to any message.
        /// </para>
        [Parameter]
        public SwitchParameter ShowTimestamp { get; set; }

        /// <para type="description">
        /// Show error and warning summary at the end.
        /// </para>
        [Parameter]
        public SwitchParameter Summary { get; set; }

        /// <para type="description">
        /// Overrides the Verbosity setting for this logger. Default verbosity is Detailed.
        /// </para>
        [Parameter(Position = 0)]
        public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Detailed;

        /// <para type="description">
        /// Show only warnings.
        /// </para>
        [Parameter]
        public SwitchParameter WarningsOnly { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var loggerParameters = new FileLoggerParameters()
            {
                Verbosity = Verbosity,
                PerformanceSummary = PerformanceSummary,
                Append = Append,
                DisableConsoleColor = DisableConsoleColor,
                DisableMPLogging = DisableMPLogging,
                EnableMPLogging = EnableMPLogging,
                Encoding = Encoding,
                ErrorsOnly = ErrorsOnly,
                ForceConsoleColor = ForceConsoleColor,
                ForceNoAlign = ForceNoAlign,
                LogFile = LogFile,
                NoItemAndPropertyList = NoItemAndPropertyList,
                NoSummary = NoSummary,
                ShowCommandLine = ShowCommandLine,
                ShowEventId = ShowEventId,
                ShowTimestamp = ShowTimestamp,
                Summary = Summary,
                WarningsOnly = WarningsOnly
            };

            var logger = Factory.InvokeInstance.CreateFileLogger(loggerParameters);

            WriteObject(logger);
        }
    }
}