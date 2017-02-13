// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using Microsoft.Build.Framework;

    [Serializable]
    public class MessageContainer
    {
        public BuildEventArgs BuildEvent { get; set; }

        public ConsoleColor Color { get; set; }

        public string FormattedMessage { get; set; }
    }
}