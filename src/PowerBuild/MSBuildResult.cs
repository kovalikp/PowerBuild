using System;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace PowerBuild
{
    [Serializable]
    public class MSBuildResult
    {
        public ITaskItem[] Items { get; set; }

        public TargetResultCode ResultCode { get; set; }

        public string Target { get; set; }
    }
}