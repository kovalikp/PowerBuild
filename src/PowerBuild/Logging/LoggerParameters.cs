// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using Microsoft.Build.Framework;

    [Serializable]
    public class LoggerParameters
    {
        public string Assembly { get; set; }

        public string ClassName { get; set; }

        public string Parameters { get; set; }

        public LoggerVerbosity Verbosity { get; set; }
    }
}