using Codestellation.DarkFlow.Bootstrap;
using Codestellation.DarkFlow.Database;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Bootstrap
{
    [TestFixture]
    public class BootstrapperTests
    {
        [SetUp]
        public void SetUp()
        {
            Utils.SafeDeleteDirectory(ManagedEsentDatabase.DefaultTaskFolder);
        }

        [Test]
        public void Creates_queued_executor()
        {
            var result = Create
                .QueuedExecutor()
                .UsingEsent()
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