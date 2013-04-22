using System;

namespace Codestellation.DarkFlow.Async
{
    public abstract class AsyncTask<TResult> :  ITask
    {
        private readonly AsyncCallback _callback;
        private LeanResult<TResult> _asyncResult;

        public LeanResult<TResult> AsyncResult
        {
            get { return _asyncResult; }
        }

        public AsyncTask(AsyncCallback callback, object asyncState) 
        {
            _asyncResult = GetAsyncResult(asyncState);
            _callback = callback;
        }

        private LeanResult<TResult> GetAsyncResult(object asyncState)
        {
            //TODO Implement AsyncResult pool to reduce pressure on GC.
            return new LeanResult<TResult>(asyncState);
        }

        public void Execute()
        {
            try
            {
                var result = InternalExecute();
                _asyncResult.Succeed(result);
            }
            catch (Exception ex)
            {
                _asyncResult.Failed(ex);
            }

            _callback(_asyncResult);
        }

        protected abstract TResult InternalExecute();
    }

    public abstract class AsyncTask :  ITask
    {
        private readonly AsyncCallback _callback;
        private LeanResult _asyncResult;

        public LeanResult AsyncResult
        {
            get { return _asyncResult; }
        }

        public AsyncTask(AsyncCallback callback, object asyncState)
        {
            _asyncResult = GetAsyncResult(asyncState);
            _callback = callback;
        }

        private LeanResult GetAsyncResult(object asyncState)
        {
            //TODO Implement AsyncResult pool to reduce pressure on GC.
            return new LeanResult(asyncState);
        }

        public void Execute()
        {
            try
            {
               InternalExecute();

            }
            catch (Exception ex)
            {
                _asyncResult.Failed(ex);
            }

            _callback(_asyncResult);
        }

        protected abstract void InternalExecute();
    }
}