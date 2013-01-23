using Castle.Facilities.Startable;
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

        public class ConstructorOrderedExecutorTester
        {
            public IExecutor OrderedExecutor { get; private set; }

            public ConstructorOrderedExecutorTester(IExecutor orderedExecutor)
            {
                OrderedExecutor = orderedExecutor;
            }
        }

        public class PropertyOrderedExecutorTester
        {
            public IExecutor OrderedExecutor { get; set; }
        }

        public class LimitConcurrencyExecutorTester
        {
            public IExecutor Executor { get; set; }

            public LimitConcurrencyExecutorTester(IExecutor executor)
            {
                Executor = executor;
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

            Assert.That(executor, Is.InstanceOf<LimitedConcurrencyExecutor>());
            Assert.That(executor, Is.Not.InstanceOf<OrderedExecutor>());
        }

        [Test]
        public void Registers_ordered_executor_properly()
        {
            var executor = _windsor.Resolve<IExecutor>("ordered");
            Assert.That(executor, Is.InstanceOf<OrderedExecutor>());
        }

        [Test]
        public void Registers_scheduler_properly()
        {
            Assert.DoesNotThrow(() => _windsor.Resolve<IScheduler>());
        }

        [Test]
        public void Resolves_ordered_if_parameter_named_ordered()
        {
            _windsor.Register(Component.For<ConstructorOrderedExecutorTester>());

            var tester = _windsor.Resolve<ConstructorOrderedExecutorTester>();

            Assert.That(tester.OrderedExecutor, Is.InstanceOf<OrderedExecutor>());
        }

        [Test]
        public void Resolves_ordered_if_property_named_ordered()
        {
            _windsor.Register(Component.For<PropertyOrderedExecutorTester>());

            var tester = _windsor.Resolve<PropertyOrderedExecutorTester>();

            Assert.That(tester.OrderedExecutor, Is.InstanceOf<OrderedExecutor>()); 
        }

        [Test]
        public void Resolves_limit_in_other_cases()
        {
            _windsor.Register(Component.For<LimitConcurrencyExecutorTester>());

            var tester = _windsor.Resolve<LimitConcurrencyExecutorTester>();

            Assert.That(tester.Executor, Is.Not.InstanceOf<OrderedExecutor>());
            Assert.That(tester.Executor, Is.InstanceOf<LimitedConcurrencyExecutor>());
        }
    }
}