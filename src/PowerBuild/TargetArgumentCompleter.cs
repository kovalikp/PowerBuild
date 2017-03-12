using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.CommandLine;
using Microsoft.Build.Execution;

namespace PowerBuild
{
    internal class TargetArgumentCompleter : IArgumentCompleter
    {
        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName, 
            string parameterName, 
            string wordToComplete, 
            CommandAst commandAst, 
            IDictionary fakeBoundParameters)
        {
            try
            {
                string Project = null;
                var IgnoreProjectExtensions = new string[0];

                var sessionState = new SessionState();
                var projects = string.IsNullOrEmpty(Project)
                    ? new[] { sessionState.Path.CurrentFileSystemLocation.Path }
                    : new[] { Project };
                var project = MSBuildApp.ProcessProjectSwitch(projects, IgnoreProjectExtensions, Directory.GetFiles);

                var properties = new Dictionary<string, string>();
                var toolsVersion = "14.0";
                var projectInstance = new ProjectInstance(project, properties, toolsVersion);
                var targets = projectInstance.Targets
                    .Select(x => x.Key);
                if (!string.IsNullOrEmpty(wordToComplete))
                {
                    targets = targets.Where(x => x.StartsWith(wordToComplete, StringComparison.InvariantCultureIgnoreCase));
                }

                return targets
                    .OrderBy(x => x)
                    .Select(x => new CompletionResult(x, x, CompletionResultType.ParameterValue, "tool tip"))
                    .ToArray();

            }
            catch
            {
                return Enumerable.Empty<CompletionResult>();
            }
        }
    }
}
