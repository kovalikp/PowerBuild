// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Language;
    using Microsoft.Build.CommandLine;
    using Microsoft.Build.Evaluation;

    internal class TargetArgumentCompleter : IArgumentCompleter
    {
        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName,
            string parameterName,
            string wordToComplete,
            CommandAst commandAst,
            IDictionary fakeBoundParameters)
        {
            var property = GetParameter<Hashtable>(fakeBoundParameters, nameof(InvokeMSBuild.Property));
            var toolsVersion = GetParameter<string>(fakeBoundParameters, nameof(InvokeMSBuild.ToolsVersion));
            var projectPath = GetParameter<string>(fakeBoundParameters, nameof(InvokeMSBuild.Project));
            var ignoreProjectExtensions = GetParameter<string[]>(fakeBoundParameters, nameof(InvokeMSBuild.IgnoreProjectExtensions));

            var properties = property?.Cast<DictionaryEntry>().ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
            properties = properties ?? new Dictionary<string, string>();

            if (string.IsNullOrEmpty(toolsVersion))
            {
                toolsVersion = InvokeMSBuildParameters.DefaultToolsVersion;
            }

            var sessionState = new SessionState();
            var projects = string.IsNullOrEmpty(projectPath)
                ? new[] { sessionState.Path.CurrentFileSystemLocation.Path }
                : new[] { Path.Combine(sessionState.Path.CurrentFileSystemLocation.Path, projectPath) };
            var projectFile = MSBuildApp.ProcessProjectSwitch(projects, ignoreProjectExtensions, Directory.GetFiles);

            var targets = FileUtilities.IsSolutionFilename(projectFile)
                ? GetSolutionTargets(projectFile)
                : GetProjectTargets(projectFile, properties, toolsVersion);

            if (!string.IsNullOrEmpty(wordToComplete))
            {
                targets = targets.Where(x => x.StartsWith(wordToComplete, StringComparison.InvariantCultureIgnoreCase));
            }

            return targets
                .OrderBy(x => x)
                .Select(x => new CompletionResult(x, x, CompletionResultType.ParameterValue, "tool tip"))
                .ToArray();
        }

        private static T GetParameter<T>(IDictionary fakeBoundParameters, string name)
        {
            if (!fakeBoundParameters.Contains(name))
            {
                return default(T);
            }

            var value = fakeBoundParameters[name];
            if (!(value is T))
            {
                return default(T);
            }

            return (T)value;
        }

        private static IEnumerable<string> GetProjectTargets(string projectFile, IDictionary<string, string> properties, string toolsVersion)
        {
            Project project = null;
            try
            {
                project = new Project(projectFile, properties, toolsVersion);
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

        private static IEnumerable<string> GetSolutionTargets(string solutionFile)
        {
            try
            {
                return SolutionTargetResolver.GetSolutionTargets(solutionFile);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}