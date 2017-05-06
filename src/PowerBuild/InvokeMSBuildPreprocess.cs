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
    using System.Text;
    using Microsoft.Build.CommandLine;
    using Microsoft.Build.Evaluation;

    /// <summary>
    /// Use MSBuild to preprocess a project.
    /// </summary>
    /// <para type="synopsis">
    /// Use MSBuild to preprocess a project.
    /// </para>
    /// <para type="description">
    /// Creates a single, aggregated project file by inlining all the files that would be imported during a build,
    /// with their boundaries marked. This can be useful for figuring out what files are being imported and from where,
    /// and what they will contribute to the build. By default the output is written to the output stream.
    /// If the path to an output file is provided that will be used instead.
    /// </para>
    /// <example>
    ///   <code>Invoke-MSBuildPreprocess -Project Project.csproj</code>
    /// </example>
    [OutputType(typeof(string))]
    [Alias("msbuild")]
    [Cmdlet(VerbsLifecycle.Invoke, "MSBuildPreprocess")]
    public class InvokeMSBuildPreprocess : PSCmdlet
    {
        private MSBuildHelper _msBuildHelper;

        /// <summary>
        /// Gets or sets Configuration property.
        /// </summary>
        /// <para type="description">
        /// Set build Configuration property.
        /// </para>
        [Parameter(Mandatory = false)]
        [ArgumentCompleter(typeof(ConfigurationArgumentCompleter))]
        public string Configuration { get; set; }

        /// <summary>
        /// Get or sets project extensions to ignore.
        /// </summary>
        /// <para type="description">
        /// List of extensions to ignore when determining which project file to build.
        /// </para>
        [Parameter]
        [Alias("ignore")]
        public string[] IgnoreProjectExtensions { get; set; }

        /// <summary>
        /// Gets or sets Platform property.
        /// </summary>
        /// <para type="description">
        /// Set build Platform property.
        /// </para>
        [Parameter(Mandatory = false)]
        [ArgumentCompleter(typeof(PlatformArgumentCompleter))]
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets project to build.
        /// </summary>
        /// <para type="description">
        /// Project to build.
        /// </para>
        [Alias("FullName")]
        [Parameter(
            Position = 0,
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Project { get; set; }

        /// <summary>
        /// Gets or sets properties.
        /// </summary>
        /// <para type="description">
        /// Set or override these project-level properties.
        /// </para>
        [Alias("p")]
        [Parameter(Mandatory = false)]
        public Hashtable Property { get; set; }

        /// <summary>
        /// Gets or sets tools version.
        /// </summary>
        /// <para type="description">
        /// The version of the MSBuild Toolset (tasks, targets, etc.) to use during build.This version will override
        /// the versions specified by individual projects.
        /// </para>
        [Parameter]
        [ValidateSet("2.0", "3.5", "4.0", "12.0", "14.0", "15.0")]
        [Alias("tv")]
        public string ToolsVersion { get; set; } = InvokeMSBuildParameters.DefaultToolsVersion;

        protected override void BeginProcessing()
        {
            WriteDebug("Begin processing");
            base.BeginProcessing();
            Factory.InvokeInstance.SetCurrentDirectory(SessionState.Path.CurrentFileSystemLocation.Path);
            _msBuildHelper = Factory.InvokeInstance.CreateMSBuildHelper();
            _msBuildHelper.BeginProcessing();
        }

        protected override void EndProcessing()
        {
            WriteDebug("End processing");
            _msBuildHelper.EndProcessing();
            _msBuildHelper = null;
            base.EndProcessing();
        }

        protected override void ProcessRecord()
        {
            WriteDebug("Process record");

            var globalProperties = Property?.Cast<DictionaryEntry>().ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
            globalProperties = globalProperties ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var projects = string.IsNullOrEmpty(Project)
                ? new[] { SessionState.Path.CurrentFileSystemLocation.Path }
                : new[] { Project };
            var projectFile = MSBuildApp.ProcessProjectSwitch(projects, IgnoreProjectExtensions, Directory.GetFiles);

            if (!string.IsNullOrEmpty(Configuration))
            {
                globalProperties[nameof(Configuration)] = Configuration;
            }

            projectFile = Path.GetFullPath(projectFile);
            if (!string.IsNullOrEmpty(Platform))
            {
                globalProperties[nameof(Platform)] = Platform;
            }

            if (FileUtilities.IsSolutionFilename(projectFile))
            {
                PreprocessSolution(projectFile, globalProperties, ToolsVersion);
            }
            else
            {
                PreprocessProject(projectFile, globalProperties, ToolsVersion);
            }
        }

        private void PreprocessProject(string projectFile, IDictionary<string, string> globalProperties, string toolsVersion)
        {
            var projectCollection = new ProjectCollection(ToolsetDefinitionLocations.Default);

            TextWriter preprocessWriter = null;
            StringBuilder stringBuilder = null;
            try
            {
                stringBuilder = new StringBuilder();
                preprocessWriter = new StringWriter(stringBuilder);

                var project = projectCollection.LoadProject(projectFile, globalProperties, ToolsVersion);
                project.SaveLogicalProject(preprocessWriter);
                projectCollection.UnloadProject(project);

                if (stringBuilder != null)
                {
                    WriteObject(stringBuilder.ToString());
                }
            }
            finally
            {
                preprocessWriter?.Dispose();
            }
        }

        private void PreprocessSolution(string projectFile, IDictionary<string, string> globalProperties, string toolsVersion)
        {
            var metaproj = projectFile + ".metaproj";
            var metaprojtmp = metaproj + ".tmp";
            var metaprojExists = File.Exists(metaproj);
            var msbuildEmitSolution = Environment.GetEnvironmentVariable("MSBuildEmitSolution");

            try
            {
                if (metaprojExists)
                {
                    File.Delete(metaproj);
                }

                Environment.SetEnvironmentVariable("MSBuildEmitSolution", "1");

                _msBuildHelper.Parameters = new InvokeMSBuildParameters
                {
                    Project = projectFile,
                    Verbosity = Microsoft.Build.Framework.LoggerVerbosity.Quiet,
                    ToolsVersion = ToolsVersion,
                    Target = new[] { "ValidateProjects" },
                    MaxCpuCount = 1,
                    NodeReuse = false,
                    Properties = globalProperties,
                    DetailedSummary = false,
                    WarningsAsErrors = null,
                    WarningsAsMessages = null
                };

                var asyncResult = _msBuildHelper.BeginProcessRecord(null, null);
                var results = _msBuildHelper.EndProcessRecord(asyncResult);
                WriteObject(File.ReadAllText(metaproj));
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ProcessRecordError", ErrorCategory.NotSpecified, null));
            }
            finally
            {
                Environment.SetEnvironmentVariable("MSBuildEmitSolution", msbuildEmitSolution);
                if (!metaprojExists && File.Exists(metaproj))
                {
                    File.Delete(metaproj);
                }

                if (!metaprojExists && File.Exists(metaprojtmp))
                {
                    File.Delete(metaprojtmp);
                }
            }
        }
    }
}