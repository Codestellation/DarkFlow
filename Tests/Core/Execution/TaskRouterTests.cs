using System;
using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Matchers;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class TaskRouterTests
    {
        private QueuedExecutor _queue;
        private LongRunningTask _task;
        private ExecutionEnvelope _executionEnvelope;
        private QueuedExecutorSettings _queuedExecutorSettings;

        [SetUp]
        public void Setup()
        {
            _queuedExecutorSettings = new QueuedExecutorSettings{Name = "test"};
            _queue = new QueuedExecutor(_queuedExecutorSettings, NullPersister.Instance);
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