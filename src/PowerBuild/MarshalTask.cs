// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild
{
    using System;
    using System.Threading.Tasks;

    public static class MarshalTask
    {
        public static Task<TResult> FromAsync<TResult>(Func<AsyncCallback, object, IAsyncResult> beginFunc, Func<IAsyncResult, TResult> endFunc)
        {
            var completer = new Completer<TResult>(endFunc);
            var tcs = new TaskCompletionSource<TResult>(completer);
            completer.TaskCompletionSource = tcs;
            beginFunc(completer.Complete, null);
            return tcs.Task;
        }

        public static IAsyncResult FromTask(Task task, AsyncCallback callback, object state)
        {
            return new TaskAsyncResult(task, callback, state);
        }

        public static IAsyncResult FromTask<T>(Task<T> task, AsyncCallback callback, object state)
        {
            return new TaskAsyncResult<T>(task, callback, state);
        }

        public static void GetResult(IAsyncResult asyncResult)
        {
            ((TaskAsyncResult)asyncResult).Wait();
        }

        public static T GetResult<T>(IAsyncResult asyncResult)
        {
            return ((TaskAsyncResult<T>)asyncResult).Result;
        }

        private class Completer<TResult> : MarshalByRefObject
        {
            private readonly Func<IAsyncResult, TResult> _endFunc;

            public Completer(Func<IAsyncResult, TResult> endFunc)
            {
                _endFunc = endFunc;
            }

            public TaskCompletionSource<TResult> TaskCompletionSource { get; set; }

            public void Complete(IAsyncResult asyncResult)
            {
                try
                {
                    TaskCompletionSource.SetResult(_endFunc(asyncResult));
                }
                catch (Exception e)
                {
                    TaskCompletionSource.SetException(e);
                }
            }
        }
    }
}