// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using System.Management.Automation;
    using Microsoft.Build.Framework;

    public class PSStreamsLogger : PSLogger
    {
        private IEventSource _eventSource;

        public override void Initialize(IEventSource eventSource, int noteCount)
        {
            base.Initialize(eventSource, noteCount);
            _eventSource = eventSource;
            _eventSource.ErrorRaised += EventSourceOnErrorRaised;
            _eventSource.WarningRaised += EventSourceOnWarningRaised;
        }

        public override void Shutdown()
        {
            _eventSource.ErrorRaised -= EventSourceOnErrorRaised;
            _eventSource.WarningRaised -= EventSourceOnWarningRaised;
            _eventSource = null;
            base.Shutdown();
        }

        protected override void Write(string message)
        {
            PSEventSink?.WriteVerbose(message);
        }

        private void EventSourceOnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            var errorRecord = new ErrorRecord(
                new Exception(FormatErrorEvent(e)),
                $"MSBuildError({e.Code})",
                ErrorCategory.NotSpecified,
                e);
            PSEventSink?.WriteError(errorRecord);
        }

        private void EventSourceOnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            var message = FormatWarningEvent(e);
            PSEventSink?.WriteWarning(message);
        }
    }
}