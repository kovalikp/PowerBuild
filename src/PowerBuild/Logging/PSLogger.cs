// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;
    using Microsoft.Build.Utilities;

    public abstract class PSLogger : Logger, INodeLogger, IPSLogger
    {
        private readonly ConsoleLogger _consoleLogger;

        public PSLogger()
        {
            _consoleLogger = new ConsoleLogger(LoggerVerbosity.Normal, Write, ColorSet, ColorReset);
        }

        public override string Parameters
        {
            get { return _consoleLogger.Parameters; }
            set { _consoleLogger.Parameters = value; }
        }

        public override LoggerVerbosity Verbosity
        {
            get { return _consoleLogger.Verbosity; }
            set { _consoleLogger.Verbosity = value; }
        }

        protected IPSEventSink PSEventSink { get; private set; }

        public sealed override void Initialize(IEventSource eventSource)
        {
            Initialize(eventSource, 1);
        }

        public virtual void Initialize(IPSEventSink psEventSink)
        {
            PSEventSink = psEventSink;
        }

        public virtual void Initialize(IEventSource eventSource, int nodeCount)
        {
            _consoleLogger.Initialize(eventSource, nodeCount);
        }

        public override void Shutdown()
        {
            _consoleLogger.Shutdown();
        }

        protected abstract void Write(string message);

        private void ColorReset()
        {
            PSEventSink?.ColorReset();
        }

        private void ColorSet(ConsoleColor color)
        {
            PSEventSink?.ColorSet(color);
        }
    }
}