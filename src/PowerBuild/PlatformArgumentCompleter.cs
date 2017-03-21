// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Build.Construction;

    internal class PlatformArgumentCompleter : InvokeMSBuildArgumentCompleterBase
    {
        private static readonly string[] DefaultProjectValues =
        {
            "AnyCPU",
            "x64",
            "x86",
        };

        protected override IEnumerable<string> GetProjectCompletionResults(InvokeMSBuildParameters parameters)
        {
            return DefaultProjectValues;
        }

        protected override IEnumerable<string> GetSolutionCompletionResults(InvokeMSBuildParameters parameters)
        {
            var solution = SolutionFile.Parse(parameters.Project);
            return solution.SolutionConfigurations.Select(x => x.PlatformName).Distinct();
        }
    }
}