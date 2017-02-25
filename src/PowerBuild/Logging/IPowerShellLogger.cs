// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System.Management.Automation.Host;
    using Microsoft.Build.Framework;

    public interface IPowerShellLogger : ILogger
    {
        void WriteEvents();
    }
}