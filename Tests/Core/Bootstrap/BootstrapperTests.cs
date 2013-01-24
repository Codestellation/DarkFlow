using Codestellation.DarkFlow.Bootstrap;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Bootstrap
{
    [TestFixture]
    public class BootstrapperTests
    {
        [Test]
        public void Creates_queued_executor()
        {
            var result = Create
                .QueuedExecutor()
                .UsingInMemoryPersistence()
                .Build();

            Assert.That(result, Is.Not.Null);
            result.Dispose();
        }

        [Test]
        public void Creates_limited_concurrency_executor()
        {
            var result = Create
                .LimitedConcurrencyExecutor()
                .Max(4.Threads())
                .UsingInMemoryPersistence()
                .Build();

            Assert.That(result, Is.Not.Null);
        }
    }
}