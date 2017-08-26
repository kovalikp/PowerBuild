// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.Management.Automation;
    using Logging;
    using Microsoft.Build.Framework;

    /// <para type="synopsis">
    /// Create new logger.
    /// </para>
    /// <para type="description">
    /// Create new logger using class name and assembly. Parameters are equivalent of msbuild.exe's /l:&lt;logger&gt; switch.
    /// </para>
    /// <example>
    ///   <code>New-Logger -ClassName FileLogger -Assembly Microsoft.Build.Engine -Parameters &quot;LogFile=MyLog.log;Append;Verbosity=diagnostic;Encoding=UTF-8&quot;</code>
    /// </example>
    [OutputType(typeof(LoggerDescription))]
    [Cmdlet(VerbsCommon.New, "Logger")]
    public class NewLogger : PSCmdlet
    {
        /// <para type="description">
        /// Logger assembly name or file path.
        /// </para>
        [Parameter(Position = 1)]
        public string Assembly { get; set; }

        /// <para type="description">
        /// Logger class name. Can contain partial or full namespace.
        /// </para>
        [Parameter(Position = 0)]
        public string ClassName { get; set; }

        /// <para type="description">
        /// Parameters passed to logger.
        /// </para>
        [Parameter(Position = 2)]
        public string Parameters { get; set; }

        /// <para type="description">
        /// Overrides the Verbosity setting for this logger. Default verbosity is Normal.
        /// </para>
        [Parameter(Position = 3)]
        public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var loggerParameters = new LoggerDescription
            {
                Assembly = Assembly,
                ClassName = ClassName,
                Parameters = Parameters,
                Verbosity = Verbosity
            };

            WriteObject(loggerParameters);
        }
    }
}