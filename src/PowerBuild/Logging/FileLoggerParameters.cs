// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using System.Text;
    using Microsoft.Build.Framework;

    [Serializable]
    internal class FileLoggerParameters : ConsoleLoggerParameters
    {
        public FileLoggerParameters()
        {
            Verbosity = LoggerVerbosity.Detailed;
        }

        public bool Append { get; set; }

        public string Encoding { get; set; }

        public string LogFile { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{nameof(LogFile)}={LogFile};");

            if (Append)
            {
                sb.Append(nameof(Append));
                sb.Append(";");
            }

            if (!string.IsNullOrEmpty(Encoding))
            {
                sb.Append($"{nameof(Encoding)}={Encoding};");
            }

            sb.Append(base.ToString());

            return sb.ToString();
        }
    }
}