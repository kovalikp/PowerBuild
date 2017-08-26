// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Management.Automation;
    using Microsoft.Build.Framework;
    using PowerBuild.Logging;

    /// <para type="synopsis">
    /// Create new binary file logger.
    /// </para>
    /// <para type="description">
    /// A logger that serializes all incoming BuildEventArgs in a compressed binary file (*.binlog). The file
    /// can later be played back and piped into other loggers (file, console, etc) to reconstruct the log contents
    /// as if a real build was happening. Additionally, this format can be read by tools for
    /// analysis or visualization. Since the file format preserves structure, tools don't have to parse
    /// text logs that erase a lot of useful information.
    /// </para>
    /// <example>
    ///   <code>New-BinaryLogger msbuild.binlog</code>
    /// </example>
    [OutputType(typeof(LoggerDescription))]
    [Cmdlet(VerbsCommon.New, "BinaryLogger")]
    public class NewBinaryLogger : PSCmdlet
    {
        /// <para type="description">
        /// Path to the log file into which the build log will be written. Default log file name is 'msbuild.binlog'.
        /// </para>
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string LogFile { get; set; } = "msbuild.binlog";

        /// <para type="description">
        /// Describes whether to collect the project files (including imported project files) used during the build.
        /// If the project files are collected they can be embedded in the log file or as a separate zip archive.
        /// </para>
        [Parameter(Mandatory = false)]
        [ValidateSet("None", "Embed", "ZipFile", IgnoreCase = true)]
        public string ProjectImports { get; set; } = "Embed";

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var loggerParameters = new LoggerDescription
            {
                ClassName = "Microsoft.Build.Logging.BinaryLogger",
                Parameters = $"{nameof(ProjectImports)}={ProjectImports};{LogFile}",
                Assembly = Factory.MSBuildVersion < new Version(15, 3) ? "StructuredLogger" : "Microsoft.Build",
                Verbosity = LoggerVerbosity.Diagnostic
            };

            WriteObject(loggerParameters);
        }
    }
}