// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Wrapper for loggers created in Invoke-MSBuild application domain.
    /// </summary>
    internal class InvokeDomainLogger : MarshalByRefObject, INodeLogger
    {
        private readonly ILogger _logger;

        public InvokeDomainLogger(ILogger logger)
        {
            _logger = logger;
        }

        public string Parameters
        {
            get { return _logger.Parameters; }
            set { _logger.Parameters = value; }
        }

        public LoggerVerbosity Verbosity
        {
            get { return _logger.Verbosity; }
            set { _logger.Verbosity = value; }
        }

        public void Initialize(IEventSource eventSource)
        {
            _logger.Initialize(eventSource);
        }

        public void Initialize(IEventSource eventSource, int nodeCount)
        {
            var nodeLogger = _logger as INodeLogger;
            if (nodeLogger != null)
            {
                nodeLogger.Initialize(eventSource, nodeCount);
            }
            else
            {
                _logger.Initialize(eventSource);
            }
        }

        public void Shutdown()
        {
            _logger.Shutdown();
        }
    }
}