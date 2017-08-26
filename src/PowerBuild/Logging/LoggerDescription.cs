// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using System.IO;
    using Microsoft.Build.Framework;

    [Serializable]
    public class LoggerDescription
    {
        public string Assembly { get; set; }

        public string ClassName { get; set; }

        public string Parameters { get; set; }

        public LoggerVerbosity? Verbosity { get; set; }

        public ILogger CreateLogger(LoggerVerbosity defaultVerbosity)
        {
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

            var loggerDescription = new Microsoft.Build.Logging.LoggerDescription(
                ClassName,
                assemblyName,
                assemblyFile,
                Parameters,
                Verbosity ?? defaultVerbosity);

            var logger = loggerDescription.CreateLogger();

            logger.Verbosity = Verbosity ?? defaultVerbosity;
            if (loggerDescription.LoggerSwitchParameters != null)
            {
                logger.Parameters = loggerDescription.LoggerSwitchParameters;
            }

            return logger;
        }
    }
}