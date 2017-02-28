// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.Management.Automation;
    using Microsoft.Build.Framework;

    /// <para type="synopsis">
    /// Create new structured logger.
    /// </para>
    /// <para type="description">
    /// Create new structured logger. For more details see <see href="https://github.com/KirillOsenkov/MSBuildStructuredLog">MSBuildStructuredLog</see>.
    /// </para>
    /// <example>
    ///   <code>New-StructuredLogger -LogFile 1.buildlog</code>
    /// </example>
    [OutputType(typeof(ILogger))]
    [Cmdlet(VerbsCommon.New, "StructuredLogger")]
    public class NewStructuredLogger : PSCmdlet
    {
        /// <para type="description">
        /// Path to the log file into which the build log will be written. Logger supports two formats: *.xml (for large human-readable XML logs)
        /// and *.buildlog (compact binary logs). Depending on which file extension you pass to the logger it will either write XML or binary.
        /// </para>
        [Parameter(Position = 0, Mandatory = true)]
        public string LogFile { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var logger = Factory.InvokeInstance.CreateStructuredLogger(LogFile);

            WriteObject(logger);
        }
    }
}