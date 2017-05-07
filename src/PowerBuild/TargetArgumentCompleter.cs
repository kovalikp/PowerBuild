// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Build.Evaluation;

    internal class TargetArgumentCompleter : InvokeMSBuildArgumentCompleterBase
    {
        protected override IEnumerable<string> GetProjectCompletionResults(InvokeMSBuildParameters parameters)
        {
            var msbuildHelper = Factory.InvokeInstance.CreateMSBuildHelper();
            msbuildHelper.Parameters = parameters;
            return msbuildHelper.GetTargets();
        }

        protected override IEnumerable<string> GetSolutionCompletionResults(InvokeMSBuildParameters parameters)
        {
            try
            {
                var tempProjectFile = SolutionHelper.CreateTempMetaproj(parameters);
                var temProjectParameters = new InvokeMSBuildParameters
                {
                    DetailedSummary = parameters.DetailedSummary,
                    MaxCpuCount = parameters.MaxCpuCount,
                    NodeReuse = parameters.NodeReuse,
                    Project = tempProjectFile,
                    Properties = parameters.Properties,
                    Target = parameters.Target,
                    ToolsVersion = parameters.ToolsVersion,
                    Verbosity = parameters.Verbosity,
                    WarningsAsErrors = parameters.WarningsAsErrors,
                    WarningsAsMessages = parameters.WarningsAsMessages,
                };

                return GetProjectCompletionResults(temProjectParameters);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}