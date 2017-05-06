// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Build.Execution;

    /// <summary>
    /// Overall results for targets and requests.
    /// </summary>
    [Serializable]
    public class BuildResult
    {
        [NonSerialized]
        private TaskItem[] _items;

        [NonSerialized]
        private TargetResult[] _targets;

        /// <summary>Gets a flag indicating whether a circular dependency was detected.</summary>
        /// <returns>Returns true if a circular dependency was detected; false otherwise.</returns>
        public bool CircularDependency { get; internal set; }

        /// <summary>Gets the configuration ID for this build result set.</summary>
        /// <returns>Returns the configuration ID for this build result set.</returns>
        public int ConfigurationId { get; internal set; }

        /// <summary>Gets the exception generated for this build result set. </summary>
        /// <returns>Returns the exception generated for this build result set. Returns false if no exception occurred.</returns>
        public Exception Exception { get; internal set; }

        /// <summary>Gets the build request id for this build result set.</summary>
        /// <returns>Returns the build request id for this build result set.</returns>
        public int GlobalRequestId { get; internal set; }

        /// <summary>Gets an enumerator over all items in this build result set.</summary>
        /// <returns>Returns an enumerator over all items in this build result set.</returns>
        public TaskItem[] Items => _items ?? (_items = ResultsByTarget.Values.SelectMany(x => x.Items).ToArray());

        /// <summary>Gets the build request ID of the originating node.</summary>
        /// <returns>Returns the build request ID of the originating node.</returns>
        public int NodeRequestId { get; internal set; }

        /// <summary>Gets the overall result for this build.</summary>
        /// <returns>Returns the overall result for this build.</returns>
        public BuildResultCode OverallResult { get; internal set; }

        /// <summary>Gets the global build request ID which issued the request leading to this build result set.</summary>
        /// <returns>Returns the global build request ID which issued the request leading to this build result set.</returns>
        public int ParentGlobalRequestId { get; internal set; }

        /// <summary>Gets the full project path.</summary>
        /// <returns>The full project path.</returns>
        public string Project { get; internal set; }

        /// <summary>Gets an enumerator over all target results in this build result set.</summary>
        /// <returns>Returns an enumerator over all target results in this build result set.</returns>
        public IReadOnlyDictionary<string, TargetResult> ResultsByTarget { get; internal set; }

        /// <summary>Gets the build submission which this build result set is associated with.</summary>
        /// <returns>Returns the build submission which this build result set is associated with.</returns>
        public int SubmissionId { get; internal set; }

        /// <summary>Gets an enumerator over all targets in this build result set.</summary>
        /// <returns>Returns an enumerator over all targets in this build result set.</returns>
        public TargetResult[] Targets => _targets ?? (_targets = ResultsByTarget.Values.ToArray());

        /// <summary>Gets an indexer which can be used to get the build result for the given target.</summary>
        /// <returns>The build result for the indexed target.</returns>
        /// <param name="target">The indexed target.</param>
        public TargetResult this[string target] => ResultsByTarget[target];

        /// <summary>Determines if there are any results for the given target.</summary>
        /// <returns>Returns true if results exist; false otherwise.</returns>
        /// <param name="target">The target whose results are retrieved.</param>
        public bool HasResultsForTarget(string target) => ResultsByTarget.ContainsKey(target);
    }
}