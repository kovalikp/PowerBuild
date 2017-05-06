// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using Microsoft.Build.Framework;

    public class PSHostLogger : PSLogger
    {
        public PSHostLogger(LoggerVerbosity verbosity)
            : base(verbosity)
        {
        }

        protected override void Write(string message)
        {
            PSEventSink?.WriteHost(message);
        }
    }
}