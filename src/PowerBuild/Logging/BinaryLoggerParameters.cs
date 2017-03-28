// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;

    [Serializable]
    internal class BinaryLoggerParameters
    {
        public BinaryLoggerParameters()
        {
        }

        public string LogFile { get; set; }

        public override string ToString()
        {
            return LogFile;
        }
    }
}