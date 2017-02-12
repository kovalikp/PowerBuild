using System;
using System.Threading.Tasks;

namespace PowerBuild
{
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