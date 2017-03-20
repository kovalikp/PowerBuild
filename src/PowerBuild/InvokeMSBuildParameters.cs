// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Build.Framework;

    [Serializable]
    internal class InvokeMSBuildParameters
    {
        public const string DefaultToolsVersion = "15.0";

        public bool DetailedSummary { get; set; }

        public int MaxCpuCount { get; set; } = 1;

        public bool NodeReuse { get; set; }

        public string Project { get; set; }

        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public string[] Target { get; set; } = new string[0];

        public string ToolsVersion { get; set; }

        public LoggerVerbosity Verbosity { get; set; }

        public ISet<string> WarningsAsErrors { get; set; }

        public ISet<string> WarningsAsMessages { get; set; }
    }
}