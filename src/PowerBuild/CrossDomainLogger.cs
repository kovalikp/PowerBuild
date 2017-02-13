// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Build.Framework;

    internal class CrossDomainLogger : MarshalByRefObject, INodeLogger, IEventSource
    {
        private readonly IEnumerable<ILogger> _loggers;
        private IEventSource _eventSource;
        private int _nodeCount = -1;

        public CrossDomainLogger(IEnumerable<ILogger> loggers)
        {
            _loggers = loggers;
        }

        public CrossDomainLogger(params ILogger[] loggers)
        {
            _loggers = loggers;
        }

        public event AnyEventHandler AnyEventRaised;

        public event BuildFinishedEventHandler BuildFinished;

        public event BuildStartedEventHandler BuildStarted;

        public event CustomBuildEventHandler CustomEventRaised;

        public event BuildErrorEventHandler ErrorRaised;

        public event BuildMessageEventHandler MessageRaised;

        public event ProjectFinishedEventHandler ProjectFinished;

        public event ProjectStartedEventHandler ProjectStarted;

        public event BuildStatusEventHandler StatusEventRaised;

        public event TargetFinishedEventHandler TargetFinished;

        public event TargetStartedEventHandler TargetStarted;

        public event TaskFinishedEventHandler TaskFinished;

        public event TaskStartedEventHandler TaskStarted;

        public event BuildWarningEventHandler WarningRaised;

        public string Parameters { get; set; }

        public LoggerVerbosity Verbosity { get; set; }

        public void Initialize(IEventSource eventSource)
        {
            Initialize(eventSource, 1);
        }

        public void Initialize(IEventSource eventSource, int nodeCount)
        {
            _nodeCount = nodeCount;
            _eventSource = eventSource;
            _eventSource.MessageRaised += EventSourceOnMessageRaised;
            _eventSource.ErrorRaised += EventSourceOnErrorRaised;
            _eventSource.WarningRaised += EventSourceOnWarningRaised;
            _eventSource.BuildStarted += EventSourceOnBuildStarted;
            _eventSource.BuildFinished += EventSourceOnBuildFinished;
            _eventSource.ProjectStarted += EventSourceOnProjectStarted;
            _eventSource.ProjectFinished += EventSourceOnProjectFinished;
            _eventSource.TargetStarted += EventSourceOnTargetStarted;
            _eventSource.TargetFinished += EventSourceOnTargetFinished;
            _eventSource.TaskStarted += EventSourceOnTaskStarted;
            _eventSource.TaskFinished += EventSourceOnTaskFinished;
            _eventSource.CustomEventRaised += EventSourceOnCustomEventRaised;
            _eventSource.StatusEventRaised += EventSourceOnStatusEventRaised;
            _eventSource.AnyEventRaised += EventSourceOnAnyEventRaised;

            foreach (var logger in _loggers)
            {
                var nodeLogger = _nodeCount > 1 ? logger as INodeLogger : null;
                if (nodeLogger == null)
                {
                    logger.Initialize(this);
                }
                else
                {
                    nodeLogger.Initialize(this, _nodeCount);
                }
            }
        }

        public void Shutdown()
        {
            foreach (var logger in _loggers)
            {
                logger.Shutdown();
            }

            _eventSource.MessageRaised -= EventSourceOnMessageRaised;
            _eventSource.ErrorRaised -= EventSourceOnErrorRaised;
            _eventSource.WarningRaised -= EventSourceOnWarningRaised;
            _eventSource.BuildStarted -= EventSourceOnBuildStarted;
            _eventSource.BuildFinished -= EventSourceOnBuildFinished;
            _eventSource.ProjectStarted -= EventSourceOnProjectStarted;
            _eventSource.ProjectFinished -= EventSourceOnProjectFinished;
            _eventSource.TargetStarted -= EventSourceOnTargetStarted;
            _eventSource.TargetFinished -= EventSourceOnTargetFinished;
            _eventSource.TaskStarted -= EventSourceOnTaskStarted;
            _eventSource.TaskFinished -= EventSourceOnTaskFinished;
            _eventSource.CustomEventRaised -= EventSourceOnCustomEventRaised;
            _eventSource.StatusEventRaised -= EventSourceOnStatusEventRaised;
            _eventSource.AnyEventRaised -= EventSourceOnAnyEventRaised;
            _eventSource = null;
        }

        private void EventSourceOnAnyEventRaised(object sender, BuildEventArgs e)
        {
            AnyEventRaised?.Invoke(sender, e);
        }

        private void EventSourceOnBuildFinished(object sender, BuildFinishedEventArgs e)
        {
            BuildFinished?.Invoke(sender, e);
        }

        private void EventSourceOnBuildStarted(object sender, BuildStartedEventArgs e)
        {
            BuildStarted?.Invoke(sender, e);
        }

        private void EventSourceOnCustomEventRaised(object sender, CustomBuildEventArgs e)
        {
            CustomEventRaised?.Invoke(sender, e);
        }

        private void EventSourceOnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            ErrorRaised?.Invoke(sender, e);
        }

        private void EventSourceOnMessageRaised(object sender, BuildMessageEventArgs e)
        {
            MessageRaised?.Invoke(sender, e);
        }

        private void EventSourceOnProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            ProjectFinished?.Invoke(sender, e);
        }

        private void EventSourceOnProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            ProjectStarted?.Invoke(sender, e);
        }

        private void EventSourceOnStatusEventRaised(object sender, BuildStatusEventArgs e)
        {
            StatusEventRaised?.Invoke(sender, e);
        }

        private void EventSourceOnTargetFinished(object sender, TargetFinishedEventArgs e)
        {
            TargetFinished?.Invoke(sender, e);
        }

        private void EventSourceOnTargetStarted(object sender, TargetStartedEventArgs e)
        {
            TargetStarted?.Invoke(sender, e);
        }

        private void EventSourceOnTaskFinished(object sender, TaskFinishedEventArgs e)
        {
            TaskFinished?.Invoke(sender, e);
        }

        private void EventSourceOnTaskStarted(object sender, TaskStartedEventArgs e)
        {
            TaskStarted?.Invoke(sender, e);
        }

        private void EventSourceOnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            WarningRaised?.Invoke(sender, e);
        }
    }
}