// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.IO;
    using System.Management.Automation;
    using Logging;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;

    [OutputType(typeof(ILogger))]
    [Cmdlet(VerbsCommon.New, "Logger")]
    public class NewLogger : PSCmdlet
    {
        [Parameter(Position = 0)]
        public string Assembly { get; set; }

        [Parameter(Position = 1)]
        public string ClassName { get; set; }

        [Parameter(Position = 3)]
        public string Parameters { get; set; }

        [Parameter(Position = 2)]
        public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var loggerParameters = new LoggerParameters
            {
                Assembly = Assembly,
                ClassName = ClassName,
                Parameters = Parameters,
                Verbosity = Verbosity
            };

            var logger = Factory.InvokeInstance.CreateLogger(loggerParameters);

            WriteObject(logger);
        }
    }
}