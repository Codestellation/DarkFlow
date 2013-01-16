using Castle.Windsor;
using Codestellation.DarkFlow.CastleWindsor;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Windsor
{
    [TestFixture]
    public class DarkFlowFacilityTests
    {
        private WindsorContainer _windsor;

        [SetUp]
        public void Setup()
        {
            _windsor = new WindsorContainer();
            _windsor.AddFacility<DarkFlowFacility>(x => x.UsingInMemoryPersistence());
        }

        [TearDown]
        public void TearDown()
        {
            _windsor.Dispose();
            _windsor = null;
        }

        [Test]
        public void Registers_executor_properly()
        {
            var executor = _windsor.Resolve<IExecutor>();

            Assert.That(executor, Is.InstanceOf<LimitedConcurrencyExecutor>());
            Assert.That(executor, Is.Not.InstanceOf<QueuedExecutor>());
        }

        [Test]
        public void Registers_queued_executor_properly()
        {
            var executor = _windsor.Resolve<IExecutor>("fiber");
            Assert.That(executor, Is.InstanceOf<QueuedExecutor>());
        }

        [Test]
        public void Registers_scheduler_properly()
        {
            Assert.DoesNotThrow(() => _windsor.Resolve<IScheduler>());
        }
    }
}