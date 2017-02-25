// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using System.Text;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;

    [Serializable]
    internal class ConsoleLoggerParameters
    {
        public bool DisableConsoleColor { get; set; }

        public bool DisableMPLogging { get; set; }

        public bool EnableMPLogging { get; set; }

        public bool ErrorsOnly { get; set; }

        public bool ForceConsoleColor { get; set; }

        public bool ForceNoAlign { get; set; }

        public bool NoItemAndPropertyList { get; set; }

        public bool NoSummary { get; set; }

        public bool PerformanceSummary { get; set; }

        public bool ShowCommandLine { get; set; }

        public bool ShowEventId { get; set; }

        public bool ShowTimestamp { get; set; }

        public bool Summary { get; set; }

        public LoggerVerbosity? Verbosity { get; set; }

        public bool WarningsOnly { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (PerformanceSummary)
            {
                sb.Append(nameof(PerformanceSummary));
                sb.Append(";");
            }

            if (Summary)
            {
                sb.Append(nameof(Summary));
                sb.Append(";");
            }

            if (NoSummary)
            {
                sb.Append(nameof(NoSummary));
                sb.Append(";");
            }

            if (ErrorsOnly)
            {
                sb.Append(nameof(ErrorsOnly));
                sb.Append(";");
            }

            if (WarningsOnly)
            {
                sb.Append(nameof(WarningsOnly));
                sb.Append(";");
            }

            if (NoItemAndPropertyList)
            {
                sb.Append(nameof(NoItemAndPropertyList));
                sb.Append(";");
            }

            if (ShowCommandLine)
            {
                sb.Append(nameof(ShowCommandLine));
                sb.Append(";");
            }

            if (ShowTimestamp)
            {
                sb.Append(nameof(ShowTimestamp));
                sb.Append(";");
            }

            if (ShowEventId)
            {
                sb.Append(nameof(ShowEventId));
                sb.Append(";");
            }

            if (ForceNoAlign)
            {
                sb.Append(nameof(ForceNoAlign));
                sb.Append(";");
            }

            if (DisableConsoleColor)
            {
                sb.Append(nameof(DisableConsoleColor));
                sb.Append(";");
            }

            if (DisableMPLogging)
            {
                sb.Append(nameof(DisableMPLogging));
                sb.Append(";");
            }

            if (EnableMPLogging)
            {
                sb.Append(nameof(EnableMPLogging));
                sb.Append(";");
            }

            if (ForceConsoleColor)
            {
                sb.Append(nameof(ForceConsoleColor));
                sb.Append(";");
            }

            if (Verbosity != null)
            {
                sb.Append($"{nameof(Verbosity)}={Verbosity.Value}");
                sb.Append(";");
            }

            return sb.ToString();
        }
    }
}