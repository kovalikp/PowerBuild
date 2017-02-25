// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.Management.Automation;
    using Logging;
    using Microsoft.Build.Framework;

    [OutputType(typeof(ILogger))]
    [Cmdlet(VerbsCommon.New, "ConsoleLogger")]
    public class NewConsoleLogger : PSCmdlet
    {
        [Parameter(HelpMessage = "Use the default console colors for all logging messages.")]
        public SwitchParameter DisableConsoleColor { get; set; }

        [Parameter(HelpMessage = "Disable the multiprocessor logging style of output when running in non - multiprocessor mode.")]
        public SwitchParameter DisableMPLogging { get; set; }

        [Parameter(HelpMessage = "Enable the multiprocessor logging style even when running in non - multiprocessor mode.This logging style is on by default.")]
        public SwitchParameter EnableMPLogging { get; set; }

        [Parameter(HelpMessage = "Show only errors.")]
        public SwitchParameter ErrorsOnly { get; set; }

        [Parameter(HelpMessage = "Use ANSI console colors even if console does not support it.")]
        public SwitchParameter ForceConsoleColor { get; set; }

        [Parameter(HelpMessage = "Does not align the text to the size of the console buffer.")]
        public SwitchParameter ForceNoAlign { get; set; }

        [Parameter(HelpMessage = "Don't show list of items and properties at the start of each project build.")]
        public SwitchParameter NoItemAndPropertyList { get; set; }

        [Parameter(HelpMessage = "Don't show error and warning summary at the end.")]
        public SwitchParameter NoSummary { get; set; }

        [Parameter(HelpMessage = "Show time spent in tasks, targets and projects.")]
        public SwitchParameter PerformanceSummary { get; set; }

        [Parameter(HelpMessage = "Show TaskCommandLineEvent messages.")]
        public SwitchParameter ShowCommandLine { get; set; }

        [Parameter(HelpMessage = "Show eventId for started events, finished events, and messages")]
        public SwitchParameter ShowEventId { get; set; }

        [Parameter(HelpMessage = "Display the Timestamp as a prefix to any message.")]
        public SwitchParameter ShowTimestamp { get; set; }

        [Parameter(HelpMessage = "Show error and warning summary at the end.")]
        public SwitchParameter Summary { get; set; }

        [Parameter(Position = 0, HelpMessage = "Overrides the Verbosity setting for this logger. Default verbosity is Normal.")]
        public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;

        [Parameter(HelpMessage = "Show only warnings.")]
        public SwitchParameter WarningsOnly { get; set; }

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

            var logger = Factory.PowerShellInstance.CreateConsoleLogger(loggerParameters, Host);

            WriteObject(logger);
        }
    }
}