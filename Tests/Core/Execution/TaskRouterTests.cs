using System;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Matchers;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class TaskRouterTests
    {
        private TaskQueue _queue;
        private LongRunningTask _task;
        private ExecutionEnvelope _executionEnvelope;
        private TaskQueueSettings _taskQueueSettings;

        [SetUp]
        public void Setup()
        {
            _taskQueueSettings = new TaskQueueSettings("test", 1, 1);
            _queue = new TaskQueue(_taskQueueSettings, NullPersister.Instance);
            _queue.TaskCountChanged += delegate {  };
            _task = new LongRunningTask(true);
            _executionEnvelope = new ExecutionEnvelope(_task, DefaultReleaser.Instance);
        }

        [Test]
        public void Returns_queue_name_for_matched_tasks()
        {
            var taskRouter = new TaskRouter(new NamespaceMatcher("test", "Codestellation.DarkFlow.Tests"), new[] {_queue});
            
            taskRouter.Route(_executionEnvelope);

            Assert.That(_queue.Count, Is.EqualTo(1));
        }

        [Test]
        public void Throws_if_no_one_matches()
        {
            var router = new TaskRouter(new NamespaceMatcher("OrderedQueue", "Codestellation.DarkFlow.Absent"), new[] {_queue});

            Assert.Throws<InvalidOperationException>(delegate { router.Route(_executionEnvelope); });
        }
    }
}