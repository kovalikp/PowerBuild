// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Management.Automation;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;
    using Microsoft.Build.Utilities;

    internal class StreamsLogger : Logger, IPowerShellLogger
    {
        private readonly Cmdlet _cmdlet;
        private readonly ConsoleLogger _consoleLogger;
        private BlockingCollection<Action<Cmdlet>> _buildEvents = new BlockingCollection<Action<Cmdlet>>();
        private IEventSource _eventSource;

        public StreamsLogger(LoggerVerbosity verbosity, Cmdlet cmdlet)
        {
            _cmdlet = cmdlet;
            _consoleLogger = new ConsoleLogger(verbosity, WriteHandler, ColorSet, ColorReset);
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

        public override void Initialize(IEventSource eventSource)
        {
            _buildEvents = new BlockingCollection<Action<Cmdlet>>();
            _eventSource = eventSource;
            _eventSource.MessageRaised += _consoleLogger.MessageHandler;
            _eventSource.ErrorRaised += EventSourceOnErrorRaised;
            _eventSource.WarningRaised += EventSourceOnWarningRaised;
            _eventSource.BuildStarted += _consoleLogger.BuildStartedHandler;
            _eventSource.BuildFinished += _consoleLogger.BuildFinishedHandler;
            _eventSource.ProjectStarted += _consoleLogger.ProjectStartedHandler;
            _eventSource.ProjectFinished += _consoleLogger.ProjectFinishedHandler;
            _eventSource.TargetStarted += _consoleLogger.TargetStartedHandler;
            _eventSource.TargetFinished += _consoleLogger.TargetFinishedHandler;
            _eventSource.TaskStarted += _consoleLogger.TaskStartedHandler;
            _eventSource.TaskFinished += _consoleLogger.TaskFinishedHandler;
            _eventSource.CustomEventRaised += _consoleLogger.CustomEventHandler;
        }

        public override void Shutdown()
        {
            _eventSource.MessageRaised -= _consoleLogger.MessageHandler;
            _eventSource.ErrorRaised -= EventSourceOnErrorRaised;
            _eventSource.WarningRaised -= EventSourceOnWarningRaised;
            _eventSource.BuildStarted -= _consoleLogger.BuildStartedHandler;
            _eventSource.BuildFinished -= _consoleLogger.BuildFinishedHandler;
            _eventSource.ProjectStarted -= _consoleLogger.ProjectStartedHandler;
            _eventSource.ProjectFinished -= _consoleLogger.ProjectFinishedHandler;
            _eventSource.TargetStarted -= _consoleLogger.TargetStartedHandler;
            _eventSource.TargetFinished -= _consoleLogger.TargetFinishedHandler;
            _eventSource.TaskStarted -= _consoleLogger.TaskStartedHandler;
            _eventSource.TaskFinished -= _consoleLogger.TaskFinishedHandler;
            _eventSource.CustomEventRaised -= _consoleLogger.CustomEventHandler;
            _buildEvents.CompleteAdding();
            _eventSource = null;
            _buildEvents = null;
            _consoleLogger.Shutdown();
            base.Shutdown();
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