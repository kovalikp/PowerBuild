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
        [Parameter]
        public SwitchParameter DisableConsoleColor { get; set; }

        [Parameter]
        public SwitchParameter DisableMPLogging { get; set; }

        [Parameter]
        public SwitchParameter EnableMPLogging { get; set; }

        [Parameter]
        public SwitchParameter ErrorsOnly { get; set; }

        [Parameter]
        public SwitchParameter ForceConsoleColor { get; set; }

        [Parameter]
        public SwitchParameter ForceNoAlign { get; set; }

        [Parameter]
        public SwitchParameter NoItemAndPropertyList { get; set; }

        [Parameter]
        public SwitchParameter NoSummary { get; set; }

        [Parameter]
        public SwitchParameter PerformanceSummary { get; set; }

        [Parameter]
        public SwitchParameter ShowCommandLine { get; set; }

        [Parameter]
        public SwitchParameter ShowEventId { get; set; }

        [Parameter]
        public SwitchParameter ShowTimestamp { get; set; }

        [Parameter]
        public SwitchParameter Summary { get; set; }

        [Parameter(Position = 0)]
        public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;

        [Parameter]
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