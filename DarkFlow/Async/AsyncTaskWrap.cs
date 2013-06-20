using System;

namespace Codestellation.DarkFlow.Async
{
    public class AsyncTaskWrap : AsyncTask
    {
        private readonly ITask _task;

        public AsyncTaskWrap(ITask task, AsyncCallback callback, object asyncState) : base(callback, asyncState)
        {
            _task = task;
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }
        }

        protected override void InternalExecute()
        {
            _task.Execute();
        }
    }

    public class AsyncTaskWrap<TTask, TResult> : AsyncTask<TResult>
        where TTask : ITask
    {
        private readonly TTask _task;
        private readonly Func<TTask, TResult> _resultGetter;

        public AsyncTaskWrap(TTask task, Func<TTask, TResult> resultGetter, AsyncCallback callback, object asyncState) : base(callback, asyncState)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            if (resultGetter == null)
            {
                throw new ArgumentNullException("resultGetter");
            }

            _task = task;
            _resultGetter = resultGetter;
        }

        protected override void InternalExecute()
        {
            _task.Execute();
        }

        protected override TResult InternalResult
        {
            get { return _resultGetter(_task); }
        }
    }
}