// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using System.Management.Automation.Host;
    using Microsoft.Build.Framework;

    public class ConsoleLogger : IPowerShellLogger, INodeLogger
    {
        private readonly Microsoft.Build.Logging.ConsoleLogger _consoleLogger;
        private readonly ConsoleColor _defaultForegroundColor;
        private readonly PSHost _host;
        private ConsoleColor _foregroundColor;

        public ConsoleLogger(LoggerVerbosity verbosity, PSHost host)
        {
            _host = host;
            _defaultForegroundColor = host.UI.RawUI.ForegroundColor;
            _consoleLogger = new Microsoft.Build.Logging.ConsoleLogger(verbosity, WriteHandler, ColorSet, ColorReset);
        }

        public string Parameters
        {
            get { return _consoleLogger.Parameters; }
            set { _consoleLogger.Parameters = value; }
        }

        public LoggerVerbosity Verbosity
        {
            get { return _consoleLogger.Verbosity; }
            set { _consoleLogger.Verbosity = value; }
        }

        public void Initialize(IEventSource eventSource)
        {
            _consoleLogger.Initialize(eventSource);
        }

        public void Initialize(IEventSource eventSource, int nodeCount)
        {
            _consoleLogger.Initialize(eventSource, nodeCount);
        }

        public void Shutdown()
        {
            _consoleLogger.Shutdown();
        }

        public void WriteEvents()
        {
        }

        private void ColorReset()
        {
            _foregroundColor = _defaultForegroundColor;
        }

        private void ColorSet(ConsoleColor color)
        {
            _foregroundColor = color;
        }

        private void WriteHandler(string message)
        {
            _host.UI.Write(_foregroundColor, _host.UI.RawUI.BackgroundColor, message);
        }
    }
}