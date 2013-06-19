using System;
using System.Threading;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Async
{
    public abstract class AsyncTask :  ITask, IAsyncResult
    {
        private readonly AsyncCallback _callback;
        private readonly object _asyncState;
        private Exception _exception;
        private int _completionStatus;
        private const int NotCompleted = 0;
        private const int RanToCompletion = 1;
        private const int Failed = 2;

        public AsyncTask(AsyncCallback callback, object asyncState)
        {
            _callback = callback;
            _asyncState = asyncState;
        }

        public void Execute()
        {
            try
            {
                InternalExecute();
                Thread.VolatileWrite(ref _completionStatus, RanToCompletion);
            }
            catch (Exception ex)
            {
                _exception = ex;
                Thread.VolatileWrite(ref _completionStatus, Failed);
            }
            
            if (_callback == null) return;
            _callback(this);
        }

        protected abstract void InternalExecute();

        public bool IsCompleted
        {
            get { return _completionStatus > NotCompleted; }
        }

        public virtual WaitHandle AsyncWaitHandle
        {
            get { throw new NotSupportedException("You should not use waiting for result. Use callback instead."); }
        }

        public object AsyncState
        {
            get { return _asyncState; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public void EnsureRanToCompletion()
        {
            //I hope happy path would be most often. Check it first. 
            if (_completionStatus == RanToCompletion)
            {
                return;
            }

            if (_completionStatus == NotCompleted)
            {
                throw new InvalidOperationException("Task is not completed yet. No result avaible");
            }

            if (_completionStatus != Failed) return;
            
            if (_exception == null)
            {
                throw new InvalidOperationException("Task failed but Exception is null. Looks like race condition. Please report a bug.");
            }

            throw _exception.WithPreservedStackTrace();
        }
    }

    public abstract class AsyncTask<TResult> :  AsyncTask
    {
        public AsyncTask(AsyncCallback callback, object asyncState) : base(callback, asyncState)
        {
            
        }

        public TResult Result
        {
            get
            {
                EnsureRanToCompletion();
                return InternalResult;
            }
        }

        protected abstract TResult InternalResult { get; }
    }
}