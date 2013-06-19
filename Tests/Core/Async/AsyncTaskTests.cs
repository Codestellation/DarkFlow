using System;
using System.Threading;
using Codestellation.DarkFlow.Async;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Async
{
    [TestFixture]
    public class AsyncTaskTests
    {
        [Test]
        public void Calls_end_when_completed()
        {
            var service = new AsyncService();

            var asyncResult = service.BeginTask(false, service.EndTask, null);

            asyncResult.AsyncWaitHandle.WaitOne();

            Assert.That(asyncResult.IsCompleted, Is.True);
        }
    }

    public class AsyncService
    {
        public IAsyncResult BeginTask(bool throwEx, AsyncCallback callback, object state)
        {
            var task = new TestAsyncTask(throwEx, callback, state);
            ThreadPool.QueueUserWorkItem(delegate { task.Execute(); });
            return task;
        }

        public void EndTask(IAsyncResult asyncResult)
        {
            var leanResult = (AsyncTask)asyncResult;
            leanResult.EnsureRanToCompletion();
        }
    }

    public class TestAsyncTask : AsyncTask
    {
        private readonly bool _throwEx;
        private readonly ManualResetEvent _event;

        public TestAsyncTask(bool throwEx, AsyncCallback callback, object asyncState) : base(callback, asyncState)
        {
            _throwEx = throwEx;
            _event = new ManualResetEvent(false);
        }

        protected override void InternalExecute()
        {
            try
            {
                if (_throwEx)
                {
                    throw new InvalidOperationException();
                }
            }
            finally
            {
                _event.Set();
            }
        }

        public override WaitHandle AsyncWaitHandle
        {
            get { return _event; }
        }
    }
}
