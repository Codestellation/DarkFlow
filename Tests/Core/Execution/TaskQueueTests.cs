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

            int enqueued = int.MinValue;
            queue.TaskCountChanged += arg => enqueued = arg;
            
            //TODO: this task should look like that new LongRunningTask(Finish.Auto | Finish.Manual)
            queue.Enqueue(new ExecutionEnvelope(new LongRunningTask(false), DefaultReleaser.Instance));

            Assert.That(enqueued, Is.EqualTo(1));
        }

        [Test]
        public void Returns_null_if_no_tasks_added()
        {
            int? dequeued = null;

            var queue = new QueuedExecutor(_queuedExecutorSettings, NullPersister.Instance);

            queue.TaskCountChanged += x => dequeued = x;

            var task = queue.Dequeue();

            Assert.That(task, Is.Null);
            Assert.That(dequeued, Is.Null); //Means not called at all
        }

        [Test]
        public void Returns_enqueued_task_if_added()
        {
            int dequeed = int.MinValue;
            var queue = new QueuedExecutor(_queuedExecutorSettings, NullPersister.Instance);

            queue.TaskCountChanged += delegate { };
            var expected = new ExecutionEnvelope(new LongRunningTask(false), DefaultReleaser.Instance);
            queue.Enqueue(expected);
            queue.TaskCountChanged += x => dequeed = x;
            var task = queue.Dequeue();
            Assert.That(task, Is.Not.Null);
            Assert.That(dequeed, Is.EqualTo(-1));
        }

        [Test]
        public void Do_not_return_new_task_until_previous_completed()
        {
            var queue = new QueuedExecutor(_queuedExecutorSettings, NullPersister.Instance);
            queue.TaskCountChanged += delegate { };
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
            queue.TaskCountChanged += delegate { };

            var task1 = new LongRunningTask(false);
            var task2 = new LongRunningTask(false);
            queue.Enqueue(new ExecutionEnvelope(task1, DefaultReleaser.Instance));
            queue.Enqueue(new ExecutionEnvelope(task2, DefaultReleaser.Instance));

            var extracted = queue.Dequeue();
            extracted.ExecuteTask();

            Assert.That(queue.Dequeue(), Is.Not.Null);
        }
    }
}