// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using Microsoft.Build.Execution;
    using Microsoft.Build.Framework;

    [Serializable]
    public class TargetResult : ITargetResult
    {
        /// <summary>Gets the exception generated when the target was built.</summary>
        /// <returns>Returns the exception generated when the target was built. Return null if no exception occurred.</returns>
        public Exception Exception { get; internal set; }

        /// <summary>Gets the set of build items output by the target. </summary>
        /// <returns>Returns the set of build items output by the target. </returns>
        public ITaskItem[] Items { get; internal set; }

        /// <summary>Gets the name of the target.</summary>
        /// <returns>Returns the name of the target.</returns>
        public string Name { get; internal set; }

        /// <summary>Gets the result code returned when the target was built.</summary>
        /// <returns>Returns the result code returned when the target was built.</returns>
        public TargetResultCode ResultCode { get; internal set; }
    }
}