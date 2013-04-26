using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class TaskQueueTests
    {
        private QueuedExecutorSettings _queuedExecutorSettings;

        [TestFixtureSetUp]
        public void MakeSettings()
        {
            _queuedExecutorSettings = new QueuedExecutorSettings { Name = "test" };
        }

        [Test]
        public void Invoke_delegate_when_task_enqueued()
        {
            var queue = new QueuedExecutor(_queuedExecutorSettings, NullPersister.Instance);

            IExecutionQueue invokedArg = null;
            queue.TaskAdded += arg => invokedArg = arg;
            
            //TODO: this task should look like that new LongRunningTask(Finish.Auto | Finish.Manual)
            queue.Enqueue(new ExecutionEnvelope(new LongRunningTask(false), DefaultReleaser.Instance));

            Assert.That(invokedArg, Is.SameAs(queue));
        }

        [Test]
        public void Do_no_invoke_delegate_when_concurrency_reached()
        {
            var queue = new QueuedExecutor(_queuedExecutorSettings, NullPersister.Instance);

            int invokedTimes = 0;
            queue.TaskAdded += arg => invokedTimes++;

            //TODO: this task should look like that new LongRunningTask(Finish.Auto | Finish.Manual)
            queue.Enqueue(new ExecutionEnvelope(new LongRunningTask(false), DefaultReleaser.Instance));

            Assert.That(invokedTimes, Is.EqualTo(1));
        }


        [Test]
        public void Returns_null_if_no_tasks_added()
        {
            int? dequeued = null;

            var queue = new QueuedExecutor(_queuedExecutorSettings, NullPersister.Instance);

            //queue.TaskCountChanged += x => dequeued = x;

            var task = queue.Dequeue();

            Assert.That(task, Is.Null);
            Assert.That(dequeued, Is.Null); //Means not called at all
        }

        [Test]
        public void Returns_enqueued_task_if_added()
        {
            int dequeed = int.MinValue;
            var queue = new QueuedExecutor(_queuedExecutorSettings, NullPersister.Instance);

            queue.TaskAdded += delegate { };
            var expected = new ExecutionEnvelope(new LongRunningTask(false), DefaultReleaser.Instance);
            queue.Enqueue(expected);
            
            var task = queue.Dequeue();

            Assert.That(task, Is.SameAs(expected));
        }

        [Test]
        public void Do_not_return_new_task_until_previous_completed()
        {
            var queue = new QueuedExecutor(_queuedExecutorSettings, NullPersister.Instance);
            queue.TaskAdded += delegate { };
            var task1 = new LongRunningTask(false);
            var task2 = new LongRunningTask(false);
            queue.Enqueue(new ExecutionEnvelope(task1, DefaultReleaser.Instance));
            queue.Enqueue(new ExecutionEnvelope(task2, DefaultReleaser.Instance));

            queue.Dequeue();

            Assert.That(queue.Dequeue(), Is.Null);
        }

        [Test]
        public void When_task_completed_allow_to_take_next_task()
        {
            var queue = new QueuedExecutor(_queuedExecutorSettings, NullPersister.Instance);
            queue.TaskAdded += delegate { };

            var task1 = new LongRunningTask(false);
            var task2 = new LongRunningTask(false);
            var envelope1 = new ExecutionEnvelope(task1, DefaultReleaser.Instance);
            queue.Enqueue(envelope1);
            var envelope2 = new ExecutionEnvelope(task2, DefaultReleaser.Instance);
            queue.Enqueue(envelope2);

            var extracted = queue.Dequeue();
            extracted.ExecuteTask();

            Assert.That(queue.Dequeue(), Is.SameAs(envelope2));
        }
    }
}