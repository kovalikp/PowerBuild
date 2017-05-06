// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using System.Management.Automation;

    public interface IPSEventSink
    {
        void ColorReset();

        void ColorSet(ConsoleColor color);

        void WriteError(ErrorRecord error);

        void WriteHost(string message);

        void WriteVerbose(string message);

        void WriteWarning(string message);
    }
}