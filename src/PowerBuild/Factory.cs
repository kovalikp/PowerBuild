// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;
    using Microsoft.Build.Shared;
    using PowerBuild.Logging;

    internal class Factory : MarshalByRefObject
    {
        private static readonly string DefaultModuleDir;

        private static readonly string DefaultMSBuildDir;

        private readonly Lazy<AppDomain> _invokeAppDomain;

        private readonly Lazy<Factory> _invokeInstance;

        private string _moduleDir;

        private string _msbuildDir;

        static Factory()
        {
            var assemblyFile = Assembly.GetExecutingAssembly().CodeBase;
            DefaultModuleDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            MSBuildVersion = GetMSBuildVersion();
            DefaultMSBuildDir = Path.GetDirectoryName(GetMSBuildPath());
            PowerShellInstance = new Factory();
        }

        public Factory()
            : this(DefaultModuleDir, DefaultMSBuildDir)
        {
        }

        public Factory(string moduleDir, string msbuildDir)
        {
            _moduleDir = moduleDir ?? throw new ArgumentNullException(nameof(moduleDir));
            _msbuildDir = msbuildDir ?? throw new ArgumentNullException(nameof(msbuildDir));

            _invokeInstance = new Lazy<Factory>(CreateInvokeFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            _invokeAppDomain = new Lazy<AppDomain>(CreateInvokeAppDomain, LazyThreadSafetyMode.ExecutionAndPublication);

            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var targetAssembly = Path.Combine(msbuildDir, new AssemblyName(eventArgs.Name).Name + ".dll");
                if (File.Exists(targetAssembly))
                {
                    return Assembly.LoadFrom(targetAssembly);
                }

                targetAssembly = Path.Combine(moduleDir, new AssemblyName(eventArgs.Name).Name + ".dll");
                if (File.Exists(targetAssembly))
                {
                    return Assembly.LoadFrom(targetAssembly);
                }

                return null;
            };
        }

        public static Factory InvokeInstance => PowerShellInstance._invokeInstance.Value;

        public static Version MSBuildVersion { get; }

        public static Factory PowerShellInstance { get; }

        public ILogger CreateBinaryLogger(BinaryLoggerParameters fileLoggerParameters)
        {
            var binaryLoggerType = Assembly.GetAssembly(typeof(ConsoleLogger)).GetType("Microsoft.Build.Logging.BinaryLogger", false, true);
            var binaryLogger = binaryLoggerType == null
                ? (ILogger)Activator.CreateInstance(binaryLoggerType)
                : new BinaryLogger();

            binaryLogger.Parameters = fileLoggerParameters.ToString();
            return binaryLogger;
        }

        public MSBuildHelper CreateMSBuildHelper()
        {
            return new MSBuildHelper();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void SetCurrentDirectory(string path)
        {
            Environment.CurrentDirectory = path;
        }

        internal static string GetMSBuildPath()
        {
            if (Environment.Is64BitProcess)
            {
                return Path.Combine(BuildEnvironmentHelper.Instance.MSBuildToolsDirectory64, "msbuild.exe");
            }
            else
            {
                return Path.Combine(BuildEnvironmentHelper.Instance.MSBuildToolsDirectory32, "msbuild.exe");
            }
        }

        internal AppDomain CreateInvokeAppDomain()
        {
            var configurationFile = Path.Combine(_msbuildDir, "MSBuild.exe.config");
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = _msbuildDir,
                ConfigurationFile = configurationFile
            };
            return AppDomain.CreateDomain("powerbuild", AppDomain.CurrentDomain.Evidence, appDomainSetup);
        }

        internal Factory CreateInvokeFactory(AppDomain appDomain)
        {
            var assemblyFile = Assembly.GetExecutingAssembly().CodeBase;
            return (Factory)_invokeAppDomain.Value.CreateInstanceFromAndUnwrap(
                assemblyFile,
                typeof(Factory).FullName,
                false,
                BindingFlags.Default,
                null,
                new object[] { _moduleDir, _msbuildDir },
                null,
                null);
        }

        private static string GetModuleDir()
        {
            var assemblyFile = Assembly.GetExecutingAssembly().CodeBase;
            return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        }

        private static string GetMSBuildDir()
        {
            return Path.GetDirectoryName(GetMSBuildPath());
        }

        private static Version GetMSBuildVersion()
        {
            try
            {
                var msbuildPath = GetMSBuildPath();
                var fileVersion = FileVersionInfo.GetVersionInfo(msbuildPath);
                return new Version(
                    fileVersion.FileMajorPart,
                    fileVersion.FileMinorPart,
                    fileVersion.FileBuildPart,
                    fileVersion.FilePrivatePart);
            }
            catch
            {
                return new Version(0, 0);
            }
        }

        private Factory CreateInvokeFactory()
        {
            return CreateInvokeFactory(_invokeAppDomain.Value);
        }
    }
}