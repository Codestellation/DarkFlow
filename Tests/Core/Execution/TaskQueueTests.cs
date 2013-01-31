using System;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class TaskQueueTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void Interpret_rules_properly(bool canEnqueue)
        {
            var queue = new TaskQueue(task => canEnqueue, () => { }, 2);

            Assert.That(queue.CanEnqueue(new LongRunningTask(false)), Is.EqualTo(canEnqueue));
        }


        [Test]
        public void Invoke_delegate_when_task_enqueued()
        {
            var enqueued = false;
            Action onEnqueued = () => enqueued = true;

            var queue = new TaskQueue(x => true, onEnqueued, 1);
            //TODO: this task should look like that new LongRunningTask(Finish.Auto | Finish.Manual)
            queue.Enqueue(new LongRunningTask(false));

            Assert.That(enqueued, Is.True);
        }

        [Test]
        public void Returns_null_if_no_tasks_added()
        {
            var queue = new TaskQueue(x => true, delegate {}, 1);

            var task = queue.Dequeue();

            Assert.That(task, Is.Null);
        }

        [Test]
        public void Returns_enqueued_task_if_added()
        {
            var queue = new TaskQueue(x => true, delegate { }, 1);
            var expected = new LongRunningTask(false);
            queue.Enqueue(expected);

            var task = queue.Dequeue();

            Assert.That(task, Is.SameAs(expected));
        }
    }
}