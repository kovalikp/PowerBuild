using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerBuild
{
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