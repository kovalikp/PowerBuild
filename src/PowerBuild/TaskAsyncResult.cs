// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TaskAsyncResult : MarshalByRefObject, IAsyncResult
    {
        private readonly Task _task;

        public TaskAsyncResult(Task task, AsyncCallback callback, object state)
        {
            _task = task;
            AsyncState = state;
            if (callback != null)
            {
                task.ContinueWith(completedTask => callback(this));
            }
        }

        public object AsyncState { get; }

        public WaitHandle AsyncWaitHandle => ((IAsyncResult)_task).AsyncWaitHandle;

        public bool CompletedSynchronously => ((IAsyncResult)_task).CompletedSynchronously;

        public bool IsCompleted => _task.IsCompleted;

        public void Wait() => _task.Wait();
    }
}