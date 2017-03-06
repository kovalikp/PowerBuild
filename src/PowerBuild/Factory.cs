// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.IO;
    using System.Management.Automation.Host;
    using System.Reflection;
    using System.Threading;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;
    using Microsoft.Build.Logging.StructuredLogger;
    using PowerBuild.Logging;

    internal class Factory : MarshalByRefObject
    {
        private static Lazy<Factory> _instance = new Lazy<Factory>(CreateInvokeFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        private static Lazy<AppDomain> _invokeAppDomain = new Lazy<AppDomain>(CreateInvokeAppDomain, LazyThreadSafetyMode.ExecutionAndPublication);

        public static Factory PowerShellInstance { get; } = new Factory();

        public static Factory InvokeInstance => _instance.Value;

        public ILogger CreateConsoleLogger(ConsoleLoggerParameters consoleLoggerParameters, PSHost host)
        {
            var verbosity = consoleLoggerParameters.Verbosity ?? LoggerVerbosity.Normal;
            var consoleLogger = new Logging.ConsoleLogger(verbosity, host);
            consoleLogger.Parameters = consoleLoggerParameters.ToString();
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

        private static AppDomain CreateInvokeAppDomain()
        {
            var configurationFile = Assembly.GetExecutingAssembly().Location + ".config";
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(typeof(MSBuildHelper).Assembly.Location),
                ConfigurationFile = configurationFile
            };
            return AppDomain.CreateDomain("powerbuild", AppDomain.CurrentDomain.Evidence, appDomainSetup);
        }

        private static Factory CreateInvokeFactory()
        {
            return (Factory)_invokeAppDomain.Value.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(Factory).FullName);
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