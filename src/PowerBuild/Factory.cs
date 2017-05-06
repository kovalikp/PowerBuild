// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Microsoft.Build.Execution;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;
    using Microsoft.Build.Logging.StructuredLogger;
    using PowerBuild.Logging;

    internal class Factory : MarshalByRefObject
    {
        private static Lazy<Factory> _instance = new Lazy<Factory>(
            CreateInvokeFactory,
            LazyThreadSafetyMode.ExecutionAndPublication);

        private static Lazy<AppDomain> _invokeAppDomain = new Lazy<AppDomain>(
            CreateInvokeAppDomain,
            LazyThreadSafetyMode.ExecutionAndPublication);

        public Factory()
        {
        }

        public Factory(string moduleDir)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var targetAssembly = Path.Combine(moduleDir, new AssemblyName(eventArgs.Name).Name + ".dll");
                return File.Exists(targetAssembly) ? Assembly.LoadFrom(targetAssembly) : null;
            };
        }

        public static Factory InvokeInstance => _instance.Value;

        public static Factory PowerShellInstance { get; } = new Factory();

        public ILogger CreateBinaryLogger(BinaryLoggerParameters fileLoggerParameters)
        {
            var fileLogger = new BinaryLogger();
            fileLogger.Parameters = fileLoggerParameters.ToString();
            return Wrap(fileLogger);
        }

        public ILogger CreateConsoleLogger(ConsoleLoggerParameters consoleLoggerParameters, bool usePSHost)
        {
            var verbosity = consoleLoggerParameters.Verbosity ?? LoggerVerbosity.Normal;
            return CreateConsoleLogger(verbosity, consoleLoggerParameters.ToString(), usePSHost);
        }

        public ILogger CreateConsoleLogger(LoggerVerbosity verbosity, string parameters, bool usePSHost)
        {
            var consoleLogger = usePSHost
                ? (ILogger)new PSHostLogger(verbosity)
                : new StreamsLogger(verbosity);
            consoleLogger.Parameters = parameters;
            return consoleLogger;
        }

        public ILogger CreateFileLogger(FileLoggerParameters fileLoggerParameters)
        {
            var fileLogger = new FileLogger();
            fileLogger.Parameters = fileLoggerParameters.ToString();
            return Wrap(fileLogger);
        }

        public ILogger CreateLogger(LoggerParameters loggerParameters)
        {
            string assemblyName = null;
            string assemblyFile = null;
            if (File.Exists(loggerParameters.Assembly))
            {
                assemblyFile = loggerParameters.Assembly;
            }
            else
            {
                assemblyName = loggerParameters.Assembly;
            }

            var loggerDescription = new LoggerDescription(
                loggerParameters.ClassName,
                assemblyName,
                assemblyFile,
                loggerParameters.Parameters,
                loggerParameters.Verbosity);

            var logger = loggerDescription.CreateLogger();
            return Wrap(logger);
        }

        public MSBuildHelper CreateMSBuildHelper()
        {
            return new MSBuildHelper();
        }

        public ILogger CreateStructuredLogger(string logFile)
        {
            var logger = new StructuredLogger();
            logger.Verbosity = LoggerVerbosity.Diagnostic;
            logger.Parameters = logFile;
            return Wrap(logger);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void SetCurrentDirectory(string path)
        {
            Environment.CurrentDirectory = path;
        }

        internal static AppDomain CreateInvokeAppDomain()
        {
            var msbuildPath = GetMSBuildPath();
            var msbuildDir = Path.GetDirectoryName(msbuildPath);
            var configurationFile = msbuildPath + ".config";
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = msbuildDir,
                ConfigurationFile = configurationFile
            };
            return AppDomain.CreateDomain("powerbuild", AppDomain.CurrentDomain.Evidence, appDomainSetup);
        }

        internal static Factory CreateInvokeFactory(AppDomain appDomain)
        {
            var assemblyFile = Assembly.GetExecutingAssembly().CodeBase;
            var assemblyDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            return (Factory)_invokeAppDomain.Value.CreateInstanceFromAndUnwrap(
                assemblyFile,
                typeof(Factory).FullName,
                false,
                BindingFlags.Default,
                null,
                new object[] { assemblyDir },
                null,
                null);
        }

        private static Factory CreateInvokeFactory()
        {
            return CreateInvokeFactory(_invokeAppDomain.Value);
        }

        private static string GetMSBuildPath()
        {
            return new BuildParameters().NodeExeLocation;
        }

        private ILogger Wrap(ILogger logger)
        {
            if (logger is MarshalByRefObject)
            {
                return logger;
            }

            return new InvokeDomainLogger(logger);
        }
    }
}