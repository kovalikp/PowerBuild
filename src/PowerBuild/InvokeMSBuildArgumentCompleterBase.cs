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
    using System.Text.RegularExpressions;
    using Microsoft.Build.CommandLine;
    using Microsoft.Build.Shared;

    internal abstract class InvokeMSBuildArgumentCompleterBase : IArgumentCompleter
    {
        private static readonly Regex DoesNotRequireEscape = new Regex(@"^\w+$", RegexOptions.Compiled);

        internal InvokeMSBuildArgumentCompleterBase()
        {
        }

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
            var target = GetParameter<string[]>(fakeBoundParameters, nameof(InvokeMSBuild.Target));
            var configuration = GetParameter<string>(fakeBoundParameters, nameof(InvokeMSBuild.Configuration));
            var platform = GetParameter<string>(fakeBoundParameters, nameof(InvokeMSBuild.Platform));

            var properties = property?.Cast<DictionaryEntry>().ToDictionary(x => x.Key.ToString(), x => x.Value.ToString(), StringComparer.OrdinalIgnoreCase);
            properties = properties ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(configuration))
            {
                properties[nameof(InvokeMSBuild.Configuration)] = configuration;
            }

            if (!string.IsNullOrEmpty(platform))
            {
                properties[nameof(InvokeMSBuild.Platform)] = platform;
            }

            if (string.IsNullOrEmpty(toolsVersion))
            {
                toolsVersion = InvokeMSBuildParameters.DefaultToolsVersion;
            }

            var sessionState = new SessionState();
            var projects = string.IsNullOrEmpty(projectPath)
                ? new[] { sessionState.Path.CurrentFileSystemLocation.Path }
                : new[] { Path.Combine(sessionState.Path.CurrentFileSystemLocation.Path, projectPath) };
            var projectFile = MSBuildApp.ProcessProjectSwitch(projects, ignoreProjectExtensions, Directory.GetFiles);

            var parameters = new InvokeMSBuildParameters
            {
                Properties = properties,
                ToolsVersion = toolsVersion,
                Project = projectFile,
                Target = target
            };

            var completionResults = FileUtilities.IsSolutionFilename(projectFile)
                ? GetSolutionCompletionResults(parameters)
                : GetProjectCompletionResults(parameters);

            if (!string.IsNullOrEmpty(wordToComplete))
            {
                completionResults = completionResults.Where(x => x.StartsWith(wordToComplete, StringComparison.InvariantCultureIgnoreCase));
            }

            return completionResults
                .OrderBy(x => x)
                .Select(x => new CompletionResult(EscapeIfRequired(x), x, CompletionResultType.ParameterValue, "tool tip"))
                .ToArray();
        }

        protected abstract IEnumerable<string> GetProjectCompletionResults(InvokeMSBuildParameters parameters);

        protected abstract IEnumerable<string> GetSolutionCompletionResults(InvokeMSBuildParameters parameters);

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

        private string EscapeIfRequired(string completionText)
        {
            if (DoesNotRequireEscape.IsMatch(completionText))
            {
                return completionText;
            }

            return "'" + completionText.Replace("'", "''") + "'";
        }
    }
}