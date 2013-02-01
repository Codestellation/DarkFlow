using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class TaskDispatcherTests
    {
        private TaskQueue _queue;
        private TaskDispatcher _pool;

        [SetUp]
        public void SetUp()
        {
            _queue = new TaskQueue(x => true, 3, 1) ;
            _pool = new TaskDispatcher(1, new IExecutionQueue[] { _queue });
        }

        [Test]
        public void Starts_thread_if_a_task_added()
        {
            var task = new LongRunningTask(false);

            _queue.Enqueue(task);

            task.WaitForFinish();

            Assert.That(task.Executed, Is.True);
        }

        [Test]
        public void Do_not_exceed_max_concurrency()
        {
            var shouldRun = new LongRunningTask(true);
            var shouldWait = new LongRunningTask(true);

            _queue.Enqueue(shouldRun);
            _queue.Enqueue(shouldWait);

            shouldRun.WaitForStart(100);
            shouldWait.WaitForStart();

            Assert.That(shouldRun.Running, Is.True);
            Assert.That(shouldWait.Running, Is.False);
        }
    }
}