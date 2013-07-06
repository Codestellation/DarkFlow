using System;
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
            object state = new object();
            IAsyncResult ar = null;
            AsyncCallback callback = delegate(IAsyncResult result) { ar = result; };

            var task = new TestAsyncTask(callback, state);

            task.Execute();

            Assert.That(ar, Is.InstanceOf<TestAsyncTask>());
            Assert.That(ar.IsCompleted, Is.True);
            Assert.That(ar.AsyncState, Is.EqualTo(state));
        }

        [Test]
        public void Task_with_result_return_result()
        {
            object state = new object();
            IAsyncResult ar = null;
            AsyncCallback callback = delegate(IAsyncResult result) { ar = result; };
            var task = new TestResultAsyncTask(false, callback, state);

            task.Execute();

            Assert.That(ar, Is.SameAs(task));
            Assert.That(ar.IsCompleted, Is.True);
            Assert.That(ar.AsyncState, Is.EqualTo(state));
            Assert.That(task.Result, Is.EqualTo(10));
        }

        [Test]
        public void Task_with_result_throws_if_not_finished()
        {
            object state = new object();
            IAsyncResult ar = null;
            AsyncCallback callback = delegate(IAsyncResult result) { ar = result; };

            var task = new TestResultAsyncTask(false, callback, state);

            Assert.That(task.IsCompleted, Is.False);
            Assert.Throws<InvalidOperationException>(() => { var x = task.Result; });
        }

        [Test]
        public void Task_with_result_throws_if_failed()
        {
            object state = new object();
            IAsyncResult ar = null;
            AsyncCallback callback = delegate(IAsyncResult result) { ar = result; };
            var task = new TestResultAsyncTask(true, callback, state);

            task.Execute();

            Assert.That(task.IsCompleted, Is.True);
            Assert.Throws<ArgumentOutOfRangeException>(() => { var x = task.Result; });
        }
    }

    public class TestAsyncTask : AsyncTask
    {
        public TestAsyncTask(AsyncCallback callback, object asyncState) : base(callback, asyncState)
        {
        }

        protected override void InternalExecute()
        {
        }
    }

    public class TestResultAsyncTask : AsyncTask<int>
    {
        private readonly bool _shouldThrow;

        public TestResultAsyncTask(bool shouldThrow, AsyncCallback callback, object asyncState)
            : base(callback, asyncState)
        {
            _shouldThrow = shouldThrow;
        }

        protected override void InternalExecute()
        {
            if (_shouldThrow) throw new ArgumentOutOfRangeException();
        }

        protected override int InternalResult
        {
            get { return 10; }
        }
    }
}
