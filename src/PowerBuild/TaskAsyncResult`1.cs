// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Threading.Tasks;

    internal class TaskAsyncResult<T> : TaskAsyncResult
    {
        private readonly Task<T> _task;

        public TaskAsyncResult(Task<T> task, AsyncCallback callback, object state)
            : base(task, callback, state)
        {
            _task = task;
        }

        public T Result => _task.Result;
    }
}