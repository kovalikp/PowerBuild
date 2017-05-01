// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Management.Automation;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    internal class StreamsLogger : Logger, IPowerShellLogger
    {
        private readonly PSCmdlet _cmdlet;
        private readonly Microsoft.Build.Logging.ConsoleLogger _consoleLogger;
        private BlockingCollection<Action<Cmdlet>> _buildEvents;
        private IEventSource _eventSource;

        public StreamsLogger(LoggerVerbosity verbosity, PSCmdlet cmdlet)
        {
            _cmdlet = cmdlet;
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

        public override void Initialize(IEventSource eventSource)
        {
            _buildEvents = new BlockingCollection<Action<Cmdlet>>();
            _eventSource = eventSource;
            _eventSource.ErrorRaised += EventSourceOnErrorRaised;
            _eventSource.WarningRaised += EventSourceOnWarningRaised;
            _consoleLogger.Initialize(eventSource);
        }

        public override void Shutdown()
        {
            _consoleLogger.Shutdown();
            _buildEvents.CompleteAdding();
            _eventSource.ErrorRaised -= EventSourceOnErrorRaised;
            _eventSource.WarningRaised -= EventSourceOnWarningRaised;
            _eventSource = null;
            _buildEvents = null;
        }

        public void WriteEvents()
        {
            foreach (var action in _buildEvents.GetConsumingEnumerable())
            {
                action(_cmdlet);
            }
        }

        private void ColorReset()
        {
        }

        private void ColorSet(ConsoleColor color)
        {
        }

        private void EventSourceOnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            var errorRecord = new ErrorRecord(new Exception(FormatErrorEvent(e)), $"MSBuildError({e.Code})", ErrorCategory.NotSpecified, e);
            _buildEvents.Add(cmdlet => _cmdlet.WriteError(errorRecord));
        }

        private void EventSourceOnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            var message = FormatWarningEvent(e);
            _buildEvents.Add(cmdlet => _cmdlet.WriteWarning(message));
        }

        private void WriteHandler(string message)
        {
            _buildEvents.Add(cmdlet => _cmdlet.WriteVerbose(message));
        }
    }
}