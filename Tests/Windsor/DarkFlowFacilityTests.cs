using Castle.Facilities.Startable;
using Castle.Windsor;
using Codestellation.DarkFlow.CastleWindsor;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Windsor
{
    [TestFixture, Ignore("Not fixed yet")]
    public class DarkFlowFacilityTests
    {
        private WindsorContainer _windsor;

        public class ConstructorOrderedExecutorTester
        {
            public IExecutor OrderedExecutor { get; private set; }

            public ConstructorOrderedExecutorTester(IExecutor orderedExecutor)
            {
                OrderedExecutor = orderedExecutor;
            }
        }

        [SetUp]
        public void Setup()
        {
            _windsor = new WindsorContainer();
            _windsor.AddFacility<StartableFacility>(x => x.DeferredStart());
            _windsor.AddFacility<DarkFlowFacility>();
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

            Assert.That(executor, Is.InstanceOf<Executor>());
        }

        [Test]
        public void Registers_scheduler_properly()
        {
            Assert.DoesNotThrow(() => _windsor.Resolve<IScheduler>());
        }
    }
}