using System;
using System.Threading;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Async
{
    public class LeanResult : IAsyncResult
    {
        private object _asyncState;
        private Exception _exception;

        public LeanResult(object asyncState)
        {
            AsyncState = asyncState;
        }

        public bool IsCompleted
        {
            get { return true; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { throw new NotSupportedException("You should not use waiting for result. Use callbacks instead."); }
        }

        public object AsyncState
        {
            get { return _asyncState; }
            set { _asyncState = value; }
        }

        public bool CompletedSynchronously
        {
            get { return true; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        internal void Failed(Exception exception)
        {
            _exception = exception;
        }

        protected void EnsureSucceed()
        {
            if (_exception != null)
            {
                throw _exception.WithPreservedStackTrace();
            }
        }

        public virtual void Release()
        {
            _exception = null;
        }
    }

    public class LeanResult<TResult> : LeanResult
    {
        private TResult _result;

        public LeanResult(object asyncState) : base(asyncState)
        {
        }

        public TResult Result
        {
            get
            {
                EnsureSucceed();
                return _result;
            }
        }

        internal void Succeed(TResult result)
        {
            _result = result;
        }
        public override void Release()
        {
            _result = default(TResult);
            base.Release();
        }
    }
}