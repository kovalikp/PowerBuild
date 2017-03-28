// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.Management.Automation;
    using Microsoft.Build.Framework;
    using PowerBuild.Logging;

    /// <para type="synopsis">
    /// Create new binary file logger.
    /// </para>
    /// <para type="description">
    /// Create new configured binary file logger.
    /// </para>
    /// <example>
    ///   <code>New-BinaryLogger msbuild.binlog</code>
    /// </example>
    [OutputType(typeof(ILogger))]
    [Cmdlet(VerbsCommon.New, "BinaryLogger")]
    public class NewBinaryLogger : PSCmdlet
    {
        /// <para type="description">
        /// Path to the log file into which the build log will be written. Default log file name is 'msbuild.binlog'.
        /// </para>
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        public string LogFile { get; set; } = "msbuild.binlog";

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var loggerParameters = new BinaryLoggerParameters()
            {
                LogFile = LogFile
            };

            var logger = Factory.InvokeInstance.CreateBinaryLogger(loggerParameters);

            WriteObject(logger);
        }
    }
}