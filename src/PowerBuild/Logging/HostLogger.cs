// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;

    using System.Management.Automation;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;

    public class HostLogger : IPowerShellLogger, INodeLogger
    {
        private readonly PSCmdlet _cmdlet;
        private readonly ConsoleLogger _consoleLogger;
        private readonly ConsoleColor _defaultForegroundColor;
        private ConsoleColor _foregroundColor;

        public HostLogger(LoggerVerbosity verbosity, PSCmdlet cmdlet)
        {
            _cmdlet = cmdlet;
            _defaultForegroundColor = cmdlet.Host.UI.RawUI.ForegroundColor;
            _consoleLogger = new ConsoleLogger(verbosity, WriteHandler, ColorSet, ColorReset);
        }

        public string Parameters
        {
            get { return _consoleLogger.Parameters; }
            set { _consoleLogger.Parameters = value; }
        }

        public bool? ShowSummary
        {
            get
            {
                return _consoleLogger.ShowSummary;
            }

            set
            {
                if (value != null)
                {
                    _consoleLogger.ShowSummary = value.Value;
                }
            }
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
            _cmdlet.Host.UI.Write(_foregroundColor, _cmdlet.Host.UI.RawUI.BackgroundColor, message);
        }
    }
}