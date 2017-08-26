// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.IO;
    using System.Management.Automation;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging.StructuredLogger;
    using PowerBuild.Logging;

    /// <para type="synopsis">
    /// Create new structured logger.
    /// </para>
    /// <para type="description">
    /// Create new structured logger. For more details see <see href="https://github.com/KirillOsenkov/MSBuildStructuredLog">MSBuildStructuredLog</see>.
    /// </para>
    /// <example>
    ///   <code>New-StructuredLogger -LogFile 1.buildlog</code>
    /// </example>
    [OutputType(typeof(LoggerDescription))]
    [Cmdlet(VerbsCommon.New, "StructuredLogger")]
    public class NewStructuredLogger : PSCmdlet
    {
        private static readonly string Assembly = "StructuredLogger";

        private static readonly string ClassName = "Microsoft.Build.Logging.StructuredLogger.StructuredLogger";

        /// <para type="description">
        /// Path to the log file into which the build log will be written. Logger supports two formats: *.xml (for large human-readable XML logs)
        /// and *.buildlog (compact binary logs). Depending on which file extension you pass to the logger it will either write XML or binary.
        /// </para>
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string LogFile { get; set; } = "msbuild.xml";

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var logger = new LoggerDescription
            {
                Assembly = Assembly,
                ClassName = ClassName,
                Parameters = LogFile,
                Verbosity = LoggerVerbosity.Diagnostic
            };

            WriteObject(logger);
        }
    }
}