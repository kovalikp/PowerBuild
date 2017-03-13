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
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
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
                string projectPath = null;
                var ignoreProjectExtensions = new string[] { ".metaproj" };

                var sessionState = new SessionState();
                var projects = string.IsNullOrEmpty(projectPath)
                    ? new[] { sessionState.Path.CurrentFileSystemLocation.Path }
                    : new[] { projectPath };
                var projectFile = MSBuildApp.ProcessProjectSwitch(projects, ignoreProjectExtensions, Directory.GetFiles);


                var globalProperties = new Dictionary<string, string>();

                var toolsVersion = "14.0";

                if (FileUtilities.IsSolutionFilename(projectFile))
                {
                    var oldMSBuildEmitSolution = Environment.GetEnvironmentVariable("MSBuildEmitSolution");
                    var helper = Factory.InvokeInstance.CreateMSBuildHelper();
                    try
                    {
                        helper.BeginProcessing();
                        helper.Parameters = new InvokeMSBuildParameters
                        {
                            Project = projectFile,
                            Verbosity = Microsoft.Build.Framework.LoggerVerbosity.Quiet,
                            ToolsVersion = toolsVersion,
                            Target = new string[] { "ValidateProjects" },
                            Properties = globalProperties,
                        };
                        var asyncResult = helper.BeginProcessRecord(null, null);
                        var results = helper.EndProcessRecord(asyncResult);
                        projectFile = projectFile + ".metaproj";
                    }
                    finally
                    {
                        Environment.SetEnvironmentVariable("MSBuildEmitSolution", oldMSBuildEmitSolution);
                        helper.EndProcessing();
                    }
                }

                var project = new Project(projectFile, globalProperties, toolsVersion);
                IEnumerable<string> targets = project.Targets.Select(x => x.Key);

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
