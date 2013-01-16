using Castle.MicroKernel.Registration;
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

        public class CtorFiberTester
        {
            public IExecutor Fiber { get; private set; }

            public CtorFiberTester(IExecutor fiber)
            {
                Fiber = fiber;
            }
        }

        public class PropFiberTester
        {
            public IExecutor Fiber { get; set; }
        }

        public class LimitTester
        {
            public IExecutor Executor { get; set; }

            public LimitTester(IExecutor executor)
            {
                Executor = executor;
            }
        }


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

        [Test]
        public void Resolves_fiber_if_parameter_named_fiber()
        {
            _windsor.Register(Component.For<CtorFiberTester>());

            var tester = _windsor.Resolve<CtorFiberTester>();

            Assert.That(tester.Fiber, Is.InstanceOf<QueuedExecutor>());
        }

        [Test]
        public void Resolves_fiber_if_property_named_fiber()
        {
            _windsor.Register(Component.For<PropFiberTester>());

            var tester = _windsor.Resolve<PropFiberTester>();

            Assert.That(tester.Fiber, Is.InstanceOf<QueuedExecutor>());
        }

        [Test]
        public void Resolves_limit_in_other_cases()
        {
            _windsor.Register(Component.For<LimitTester>());

            var tester = _windsor.Resolve<LimitTester>();

            Assert.That(tester.Executor, Is.Not.InstanceOf<QueuedExecutor>());
            Assert.That(tester.Executor, Is.InstanceOf<LimitedConcurrencyExecutor>());
        }
    }
}