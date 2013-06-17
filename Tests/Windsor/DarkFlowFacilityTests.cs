using Castle.Facilities.Startable;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Codestellation.DarkFlow.CastleWindsor;
using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Stat;
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
            _windsor.Kernel.Resolver.AddSubResolver(new CollectionResolver(_windsor.Kernel));
            _windsor.AddFacility<StartableFacility>(x => x.DeferredStart());

            
        }

        private void AddCodedFacility()
        {
            var facility = new DarkFlowFacility()
                .EnableMonitoring()
                .MaxConcurrency(4)
                .UsingInMemoryPersistence()
                .WithQueuedExecutor(new QueuedExecutorSettings {Name = "someExecutor"})
                .RouteTasks(x =>
                    {
                        x.ByNamespace("Codestellation.*").To("someExecutor");
                        x.MarkedWith<MarkerAttribute>().To("someExecutor");
                        x.ByContent("executor.{Name}").CacheEnvironments();
                    })
                .PersistTasks(x =>
                    {
                        //TODO: Calls to To() are ugly, because they are completely ignored. Need more elegant solution to this.
                        x.ByNamespace("Codestellation.*").To("xx");
                        x.MarkedWith<MarkerAttribute>().To("xx");
                    });

            _windsor.AddFacility(facility);
        }

        private void AddXmlConfiguredFacility()
        {
            var facility = new DarkFlowFacility().ConfigureFromXml();
            _windsor.AddFacility(facility);
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
            AddCodedFacility();

            var executor = _windsor.Resolve<IExecutor>();

            Assert.That(executor, Is.InstanceOf<Executor>());
        }

        [Test]
        public void Registers_executor_properly_using_xml_config()
        {
            AddXmlConfiguredFacility();

            var executor = _windsor.Resolve<IExecutor>();

            Assert.That(executor, Is.InstanceOf<Executor>());
        }


        [Test]
        public void Registers_scheduler_properly()
        {
            AddCodedFacility();
            Assert.DoesNotThrow(() => _windsor.Resolve<IScheduler>());
        }

        [Test]
        public void Registers_monitor()
        {
            AddCodedFacility();
            var executor = (Executor)_windsor.Resolve<IExecutor>();

            Assert.That(executor.Monitor, Is.InstanceOf<Monitor>());
        }
    }
}