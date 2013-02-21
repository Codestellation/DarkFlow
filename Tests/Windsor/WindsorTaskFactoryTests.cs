using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Codestellation.DarkFlow.CastleWindsor;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Tests.Core.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Windsor
{
    //TODO: Remove duplication
    [TestFixture, Ignore]
    public class WindsorTaskFactoryTests
    {
        private WindsorContainer _windsor;
        private ISerializer _serializer;

        [SetUp]
        public void Setup()
        {
            _windsor = new WindsorContainer();
            _windsor.AddFacility<DarkFlowFacility>(x => x.UsingInMemoryPersistence());

            _windsor.Register(
                Component
                    .For<TaskInterceptor>()
                    .LifestyleTransient());

            _serializer = _windsor.Resolve<ISerializer>();
        }

        [TearDown]
        public void TearDown()
        {
            _windsor.Dispose();
        }

        private void RegisterTaskAsSelf()
        {
            _windsor.Register(Component
                                  .For<PersistedTask>()
                                  .Interceptors<TaskInterceptor>()
                                  .LifestyleTransient());
        }

        private void RegisterTaskAsIPersistentTask()
        {
            _windsor.Register(Component
                                  .For<ITask>()
                                  .ImplementedBy<PersistedTask>()
                                  .Interceptors<TaskInterceptor>()
                                  .LifestyleTransient());
        }

        [Test]
        public void Can_persist_and_restore_class_intercepted_tasks()
        {
            RegisterTaskAsSelf();
            
            var task = _windsor.Resolve<PersistedTask>(new{state = new State {Id = 1, Name = "Test"}});

            var serializedTask = _serializer.Serialize(task);

            var desialized = _serializer.Deserialize(serializedTask);

            Assert.That(desialized, Is.InstanceOf<IProxyTargetAccessor>());
        }

        [Test]
        public void Can_persist_and_restore_interface_intercepted_tasks()
        {
            RegisterTaskAsIPersistentTask();

            var task = _windsor.Resolve<ITask>(new { state = new State { Id = 1, Name = "Test" } });

            var serializedTask = _serializer.Serialize(task);

            var desialized = _serializer.Deserialize(serializedTask);

            Assert.That(desialized, Is.InstanceOf<IProxyTargetAccessor>());
        }

        [Test]
        public void Can_persist_and_restore_non_registered_task()
        {
            var task = new PersistedTask(new State { Id = 1, Name = "Test" });

            var serializedTask = _serializer.Serialize(task);

            var desialized = _serializer.Deserialize(serializedTask);

            Assert.That(desialized, Is.InstanceOf<PersistedTask>());
        }

    }

    public class TaskInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}