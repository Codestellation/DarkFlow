using System;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class TaskRouterTests
    {
        [Test]
        public void Returns_queue_name_for_matched_tasks()
        {
            var taskRouter = new TaskRouter(new[] {new NamespaceMatcher("Codestellation.DarkFlow.Tests", "OrderedQueue")});

            var actualQueue = taskRouter.ResolveQueueFor(new LongRunningTask(true));

            Assert.That(actualQueue, Is.EqualTo("OrderedQueue"));
        }

        [Test]
        public void Throws_if_more_than_one_matches()
        {
            var router = new TaskRouter(new[]
                                            {
                                                new NamespaceMatcher("Codestellation.DarkFlow.Tests", "OrderedQueue"),
                                                new NamespaceMatcher("Codestellation.DarkFlow.Tests", "OrderedQueue")
                                            });
            Assert.Throws<InvalidOperationException>(delegate { router.ResolveQueueFor(new LongRunningTask(false)); });
        }

        [Test]
        public void Throws_if_no_one_matches()
        {
            var router = new TaskRouter(new[] { new NamespaceMatcher("Codestellation.DarkFlow.Absent", "OrderedQueue")});

            Assert.Throws<InvalidOperationException>(delegate { router.ResolveQueueFor(new LongRunningTask(false)); });
        }
    }
}