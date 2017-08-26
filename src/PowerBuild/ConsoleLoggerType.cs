// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    /// <summary>
    /// Enumeration for ConsoleLogger parameter of Invoke-MSBuild.
    /// </summary>
    public enum ConsoleLoggerType
    {
        /// <summary>
        /// No logger
        /// </summary>
        None,

        /// <summary>
        /// Logger writing to PowerShell output streams.
        /// </summary>
        PSStreams,

        /// <summary>
        /// Logger writing to PowerShell host.
        /// </summary>
        PSHost
    }
}