// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using Microsoft.Build.Framework;

    [Serializable]
    internal class InvokeMSBuildParameters
    {
        public bool DetailedSummary { get; set; }

        public int? MaxCpuCount { get; set; }

        public bool NodeReuse { get; set; }

        public string[] Project { get; set; }

        public string[] Target { get; set; }

        public string ToolsVersion { get; set; }

        public LoggerVerbosity Verbosity { get; set; }
    }
}