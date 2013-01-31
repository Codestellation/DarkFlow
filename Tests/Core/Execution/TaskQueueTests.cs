using System;
using System.Threading;
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
            var queue = new TaskQueue(task => canEnqueue, delegate { }, 2);

            Assert.That(queue.CanEnqueue(new LongRunningTask(false)), Is.EqualTo(canEnqueue));
        }


        [Test]
        public void Invoke_delegate_when_task_enqueued()
        {
            int enqueued = int.MinValue;
            Action<int> onEnqueued = arg => enqueued = arg;
            var queue = new TaskQueue(x => true, onEnqueued, 1);
            //TODO: this task should look like that new LongRunningTask(Finish.Auto | Finish.Manual)
            queue.Enqueue(new LongRunningTask(false));

            Assert.That(enqueued, Is.EqualTo(1));
        }

        [Test]
        public void Returns_null_if_no_tasks_added()
        {
            int? dequeued = null;

            var queue = new TaskQueue(x => true, x => dequeued = x, 1);

            var task = queue.Dequeue();

            Assert.That(task, Is.Null);
            Assert.That(dequeued, Is.Null); //Means not called at all
        }

        [Test]
        public void Returns_enqueued_task_if_added()
        {
            int dequeed = int.MinValue;
            var queue = new TaskQueue(x => true, x => dequeed = x, 1);
            var expected = new LongRunningTask(false);
            queue.Enqueue(expected);

            var task = queue.Dequeue();

            Assert.That(task, Is.SameAs(expected));
            Assert.That(dequeed, Is.EqualTo(-1));
        }
    }
}