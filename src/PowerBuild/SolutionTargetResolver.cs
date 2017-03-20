// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Build.Construction;

    internal static class SolutionTargetResolver
    {
        /// <summary>
        /// Project names that need to be disambiguated when forming a target name
        /// </summary>
        private static readonly string[] DefaultSolutionTargets =
        {
            "Build",
            "Rebuild",
            "Clean",
            "Publish"
        };

        /// <summary>
        /// The initial target with some solution configuration validation/information
        /// </summary>
        private static readonly string[] InitialTargets =
        {
            "Restore",
            "GenerateRestoreGraphFile",
            "ValidateSolutionConfiguration",
            "ValidateToolsVersions",
            "ValidateProjects",
            "GetSolutionConfigurationContents",
        };

        public static IEnumerable<string> GetSolutionTargets(string solutionFile)
        {
            var solution = SolutionFile.Parse(solutionFile);
            var selectedSolutionConfiguration = $"{solution.GetDefaultConfigurationName()}|{solution.GetDefaultPlatformName()}";

            var result = new List<string>();
            result.AddRange(InitialTargets);
            result.AddRange(DefaultSolutionTargets);
            result.AddRange(GetProjectTargets(solution, selectedSolutionConfiguration));

            return result;
        }

        internal static string DisambiguateProjectTargetName(string uniqueProjectName)
        {
            // Test our unique project name against those names that collide with Solution
            // entry point targets
            foreach (string solutionTarget in DefaultSolutionTargets)
            {
                if (string.Compare(uniqueProjectName, solutionTarget, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    // Prepend "Solution:" so that the collision is resolved, but the
                    // log of the solution project still looks reasonable.
                    return "Solution:" + uniqueProjectName;
                }
            }

            return uniqueProjectName;
        }

        private static string GetActualTargetName(ProjectInSolution project, string targetToBuild)
        {
            string baseProjectName = DisambiguateProjectTargetName(project.GetUniqueProjectName());
            string actualTargetName = baseProjectName;
            if (targetToBuild != null)
            {
                actualTargetName += ":" + targetToBuild;
            }

            return actualTargetName;
        }

        private static IEnumerable<string> GetProjectTargets(SolutionFile solutionFile, string selectedSolutionConfiguration)
        {
            foreach (ProjectInSolution project in solutionFile.ProjectsInOrder)
            {
                project.ProjectConfigurations.TryGetValue(selectedSolutionConfiguration, out ProjectConfigurationInSolution projectConfiguration);
                if (!WouldProjectBuild(solutionFile, selectedSolutionConfiguration, project, projectConfiguration))
                {
                    // Project wouldn't build, so omit it from further processing.
                    continue;
                }

                yield return GetActualTargetName(project, null);
                yield return GetActualTargetName(project, "Clean");
                yield return GetActualTargetName(project, "Rebuild");
                yield return GetActualTargetName(project, "Publish");
            }
        }

        private static string GetUniqueProjectName(this ProjectInSolution projectInSolution)
        {
            var getUniqueProjectName = typeof(ProjectInSolution).GetMethod("GetUniqueProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)getUniqueProjectName.Invoke(projectInSolution, new object[0]);
        }

        private static bool WouldProjectBuild(
            SolutionFile solutionFile,
            string selectedSolutionConfiguration,
            ProjectInSolution project,
            ProjectConfigurationInSolution projectConfiguration)
        {
            if (projectConfiguration == null)
            {
                if (project.ProjectType == SolutionProjectType.WebProject)
                {
                    // Sometimes web projects won't have the configuration we need (Release typically.)  But they should still build if there is
                    // a solution configuration for it
                    foreach (SolutionConfigurationInSolution configuration in solutionFile.SolutionConfigurations)
                    {
                        if (string.Equals(configuration.FullName, selectedSolutionConfiguration, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }

                // No configuration, so it can't build.
                return false;
            }

            if (!projectConfiguration.IncludeInBuild)
            {
                // Not included in the build.
                return false;
            }

            return true;
        }
    }
}