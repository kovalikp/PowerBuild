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
            Project project = null;
            try
            {
                project = new Project(parameters.Project, parameters.Properties, parameters.ToolsVersion);
                return project.Targets.Select(x => x.Key);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
            finally
            {
                if (project != null)
                {
                    ProjectCollection.GlobalProjectCollection.UnloadProject(project);
                }
            }
        }

        protected override IEnumerable<string> GetSolutionCompletionResults(InvokeMSBuildParameters parameters)
        {
            try
            {
                parameters.Properties.TryGetValue(nameof(InvokeMSBuild.Configuration), out string configuration);
                parameters.Properties.TryGetValue(nameof(InvokeMSBuild.Platform), out string platform);
                return SolutionTargetResolver.GetSolutionTargets(parameters.Project, configuration, platform);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}