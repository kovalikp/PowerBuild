using System;
using Microsoft.Build.BackEnd.Logging;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace PowerBuild
{
    using System.Collections.Concurrent;
    using System.Management.Automation;
    using Microsoft.Build.Utilities;
    internal class PowerShellLogger : Logger
    {
        private readonly LoggerVerbosity _verbosity;
        private readonly Cmdlet _cmdlet;
        private IEventSource _eventSource;
        private BlockingCollection<Action<Cmdlet>> _buildEvents = new BlockingCollection<Action<Cmdlet>>();
        private ConsoleLogger _consoleLogger;

        public PowerShellLogger(LoggerVerbosity verbosity,  Cmdlet cmdlet)
        {
            _verbosity = verbosity;
            _cmdlet = cmdlet;
            _consoleLogger = new ConsoleLogger(verbosity, WriteHandler, ColorSet, ColorReset);
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
        

        public void ConsumeEvents()
        {
            foreach (var action in _buildEvents.GetConsumingEnumerable())
            {
                action(_cmdlet);
            }
        }

        private void EventSourceOnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            var message = FormatWarningEvent(e);
            _buildEvents.Add(cmdlet => _cmdlet.WriteWarning(message));
        }

        private void EventSourceOnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            var errorRecord = new ErrorRecord(new Exception(FormatErrorEvent(e)), e.Code, ErrorCategory.NotSpecified, e);
            _buildEvents.Add(cmdlet => _cmdlet.WriteError(errorRecord));
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

        private void ColorReset()
        {
        }

        private void ColorSet(ConsoleColor color)
        {
        }

        private void WriteHandler(string message)
        {
            _buildEvents.Add(cmdlet => _cmdlet.WriteVerbose(message));
        }

    }
}