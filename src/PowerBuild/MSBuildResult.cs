// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using Microsoft.Build.Execution;
    using Microsoft.Build.Framework;

    [Serializable]
    public class MSBuildResult
    {
        public ITaskItem[] Items { get; set; }

        public TargetResultCode ResultCode { get; set; }

        public string Target { get; set; }
    }
}