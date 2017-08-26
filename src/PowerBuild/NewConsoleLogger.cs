// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Management.Automation;
    using Logging;
    using Microsoft.Build.Framework;

    /// <para type="synopsis">
    /// Create new console logger.
    /// </para>
    /// <para type="description">
    /// Create new configured console logger. Parameters are equivalent of msbuild.exe's /consoleloggerparameters:&lt;parameters&gt; switch.
    /// </para>
    /// <example>
    ///   <code>New-ConsoleLogger -Verbosity Minimal -PerformanceSummary</code>
    /// </example>
    [OutputType(typeof(LoggerDescription))]
    [Cmdlet(VerbsCommon.New, "ConsoleLogger")]
    public class NewConsoleLogger : PSCmdlet
    {
        internal static readonly string Assembly = typeof(PSLogger).Assembly.FullName;

        internal static readonly string PSHostLoggerClassName = typeof(PSHostLogger).FullName;

        internal static readonly string PSStreamsLoggerClassName = typeof(PSStreamsLogger).FullName;

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
        /// Log to PowerShell host instead of output streams.
        /// </para>
        [Parameter]
        public SwitchParameter PSHost { get; set; }

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
        /// Overrides the Verbosity setting for this logger. Default verbosity is Normal.
        /// </para>
        [Parameter(Position = 0)]
        public LoggerVerbosity? Verbosity { get; set; }

        /// <para type="description">
        /// Show only warnings.
        /// </para>
        [Parameter]
        public SwitchParameter WarningsOnly { get; set; }

        internal static bool IsPSLogger(LoggerDescription loggerParameters)
        {
            var comparision = StringComparison.OrdinalIgnoreCase;
            var classMatch =
                typeof(PSHostLogger).FullName.Equals(loggerParameters.ClassName, comparision)
                || typeof(PSStreamsLogger).FullName.Equals(loggerParameters.ClassName, comparision)
                || typeof(PSHostLogger).Name.Equals(loggerParameters.ClassName, comparision)
                || typeof(PSStreamsLogger).Name.Equals(loggerParameters.ClassName, comparision);
            var assemblyMach = typeof(PSLogger).Assembly.FullName.Equals(loggerParameters.Assembly, comparision);

            return assemblyMach && classMatch;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var loggerParameters = new ConsoleLoggerParameters()
            {
                Verbosity = Verbosity,
                PerformanceSummary = PerformanceSummary,
                DisableConsoleColor = DisableConsoleColor,
                DisableMPLogging = DisableMPLogging,
                EnableMPLogging = EnableMPLogging,
                ErrorsOnly = ErrorsOnly,
                ForceConsoleColor = ForceConsoleColor,
                ForceNoAlign = ForceNoAlign,
                NoItemAndPropertyList = NoItemAndPropertyList,
                NoSummary = NoSummary,
                ShowCommandLine = ShowCommandLine,
                ShowEventId = ShowEventId,
                ShowTimestamp = ShowTimestamp,
                Summary = Summary,
                WarningsOnly = WarningsOnly
            };

            var logger = new LoggerDescription
            {
                Assembly = Assembly,
                ClassName = PSHost.IsPresent ? PSHostLoggerClassName : PSStreamsLoggerClassName,
                Parameters = loggerParameters.ToString(),
                Verbosity = Verbosity
            };

            WriteObject(logger);
        }
    }
}