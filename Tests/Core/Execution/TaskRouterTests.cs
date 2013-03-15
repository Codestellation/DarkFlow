using System;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Matchers;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class TaskRouterTests
    {
        [Test]
        public void Returns_queue_name_for_matched_tasks()
        {
            var taskRouter = new TaskRouter(new NamespaceMatcher("OrderedQueue", "Codestellation.DarkFlow.Tests"));

            var actualQueue = taskRouter.ResolveQueueFor(new LongRunningTask(true));

            Assert.That(actualQueue, Is.EqualTo("OrderedQueue"));
        }

        [Test]
        public void Throws_if_no_one_matches()
        {
            var router = new TaskRouter(new NamespaceMatcher("OrderedQueue", "Codestellation.DarkFlow.Absent"));

            Assert.Throws<InvalidOperationException>(delegate { router.ResolveQueueFor(new LongRunningTask(false)); });
        }
    }
}