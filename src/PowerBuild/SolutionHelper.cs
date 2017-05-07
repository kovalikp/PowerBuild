// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using Microsoft.Build.Shared;

    internal static class SolutionHelper
    {
        private static readonly Timer _deleteTempFilesTimer;
        private static readonly string _tempDir = Path.Combine(Path.GetTempPath(), "PowerBuild");

        static SolutionHelper()
        {
            _deleteTempFilesTimer = new Timer(DeleteTempFiles, null, TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
        }

        public static string CreateTempMetaproj(InvokeMSBuildParameters parameters)
        {
            return CreateTempMetaproj(parameters.Project, parameters.Properties, parameters.ToolsVersion);
        }

        public static string CreateTempMetaproj(string projectFile, IDictionary<string, string> globalProperties, string toolsVersion)
        {
            projectFile = FileUtilities.NormalizePath(projectFile);
            var tempName = GetTempFile(projectFile, globalProperties, toolsVersion);
            if (!Directory.Exists(_tempDir))
            {
                Directory.CreateDirectory(_tempDir);
            }

            var tempProjectFile = Path.Combine(_tempDir, tempName + ".sln");

            var tempMetaproj = tempProjectFile + ".metaproj";
            var tempMetaprojtmp = tempMetaproj + ".tmp";
            var msbuildEmitSolution = Environment.GetEnvironmentVariable("MSBuildEmitSolution");

            try
            {
                var solutionChanged = !(File.Exists(tempProjectFile)
                    && File.GetLastWriteTime(projectFile) < File.GetLastWriteTime(tempMetaproj));

                if (solutionChanged)
                {
                    File.Copy(projectFile, tempProjectFile, true);

                    EmitMetaproj(tempProjectFile, globalProperties, toolsVersion);
                }

                return tempMetaproj;
            }
            finally
            {
                Environment.SetEnvironmentVariable("MSBuildEmitSolution", msbuildEmitSolution);
            }
        }

        private static void DeleteTempFiles(object state)
        {
            if (!Directory.Exists(_tempDir))
            {
                return;
            }

            foreach (var file in Directory.EnumerateFiles(_tempDir, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    if (File.GetLastAccessTimeUtc(file) < DateTime.UtcNow.AddMinutes(-10))
                    {
                        File.Delete(file);
                    }
                }
                catch
                {
                    // pass
                }
            }
        }

        private static void EmitMetaproj(string projectFile, IDictionary<string, string> globalProperties, string toolsVersion)
        {
            Environment.SetEnvironmentVariable("MSBuildEmitSolution", "1");

            var msbuildHelper = Factory.InvokeInstance.CreateMSBuildHelper();
            msbuildHelper.Parameters = new InvokeMSBuildParameters
            {
                Project = projectFile,
                Verbosity = Microsoft.Build.Framework.LoggerVerbosity.Quiet,
                ToolsVersion = toolsVersion,
                Target = new[] { "ValidateSolutionConfiguration" },
                MaxCpuCount = 1,
                NodeReuse = false,
                Properties = globalProperties,
                DetailedSummary = false,
                WarningsAsErrors = null,
                WarningsAsMessages = null
            };

            msbuildHelper.BeginProcessing();
            var asyncResult = msbuildHelper.BeginProcessRecord(null, null);
            var results = msbuildHelper.EndProcessRecord(asyncResult);
            msbuildHelper.EndProcessing();
        }

        private static string GetTempFile(string projectFile, IDictionary<string, string> globalProperties, string toolsVersion)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(projectFile);
            stringBuilder.AppendLine(toolsVersion);
            foreach (var property in globalProperties.OrderBy(x => x.Key))
            {
                stringBuilder.AppendLine(property.Key);
                stringBuilder.AppendLine(property.Value);
            }

            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
            return string.Join(string.Empty, hash.Select(b => b.ToString("x2")).ToArray());
        }
    }
}