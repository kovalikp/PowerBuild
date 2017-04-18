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
        /// Gets or sets the output file path.
        /// </summary>
        /// <para type="description">
        /// Path of aggregate project file to write the output to.
        /// </para>
        [Parameter(Mandatory = false)]
        public string OutputFile { get; set; }

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
            Factory.PowerShellInstance.SetCurrentDirectory(SessionState.Path.CurrentFileSystemLocation.Path);
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

            if (FileUtilities.IsSolutionFilename(projectFile))
            {
                WriteError(new ErrorRecord(new InvalidOperationException("Preprocessing solution files is not supported."), "PreprocessSolutionNotSupported", ErrorCategory.InvalidOperation, projectFile));
                return;
            }

            if (!string.IsNullOrEmpty(Configuration))
            {
                globalProperties[nameof(Configuration)] = Configuration;
            }

            if (!string.IsNullOrEmpty(Platform))
            {
                globalProperties[nameof(Platform)] = Platform;
            }

            var projectCollection = new ProjectCollection(ToolsetDefinitionLocations.Default);

            TextWriter preprocessWriter = null;
            StringBuilder stringBuilder = null;
            try
            {
                if (string.IsNullOrEmpty(OutputFile))
                {
                    stringBuilder = new StringBuilder();
                    preprocessWriter = new StringWriter(stringBuilder);
                }
                else
                {
                    preprocessWriter = new StreamWriter(new FileStream(OutputFile, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.SequentialScan));
                }

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
    }
}