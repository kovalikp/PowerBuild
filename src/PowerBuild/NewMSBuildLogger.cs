// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.IO;
    using System.Management.Automation;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;

    [OutputType(typeof(MSBuildResult))]
    [Cmdlet(VerbsCommon.New, "MSBuildLogger")]
    public class NewMSBuildLogger : PSCmdlet
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

            string assemblyName = null;
            string assemblyFile = null;
            if (File.Exists(Assembly))
            {
                assemblyFile = Assembly;
            }
            else
            {
                assemblyName = Assembly;
            }

            var loggerDescription = new LoggerDescription(ClassName, assemblyName, assemblyFile, Parameters, Verbosity);
            var logger = loggerDescription.CreateLogger();

            WriteObject(logger);
        }
    }
}