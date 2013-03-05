using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Codestellation.DarkFlow.CastleWindsor;
using Codestellation.DarkFlow.Tests.Core.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Windsor
{
    //TODO: Remove duplication
    [TestFixture]
    public class WindsorTaskFactoryTests
    {
        private WindsorContainer _windsor;

        [SetUp]
        public void Setup()
        {
            _windsor = new WindsorContainer();
            _windsor.AddFacility<DarkFlowFacility>(x => x.UsingInMemoryPersistence());

            _windsor.Register(
                Component
                    .For<TaskInterceptor>()
                    .LifestyleTransient());

        }

        [TearDown]
        public void TearDown()
        {
            _windsor.Dispose();
        }

        private void RegisterTaskAsSelf()
        {
            _windsor.Register(Component
                                  .For<PersistentTaskWithDependency>()
                                  .Interceptors<TaskInterceptor>()
                                  .LifestyleTransient());
        }

        private void RegisterTaskAsIPersistentTask()
        {
            _windsor.Register(Component
                                  .For<ITask>()
                                  .ImplementedBy<PersistentTaskWithDependency>()
                                  .Interceptors<TaskInterceptor>()
                                  .LifestyleTransient());
        }

        [Test]
        public void Can_persist_and_restore_class_intercepted_tasks()
        {
            RegisterTaskAsSelf();
            
            var task = _windsor.Resolve<PersistentTaskWithDependency>(new{count = 1});

            //var serializedTask = _serialize.Serialize(task);

            //var desialized = _serialize.Deserialize(serializedTask);

            //Assert.That(desialized, Is.InstanceOf<IProxyTargetAccessor>());
        }

        [Test]
        public void Can_persist_and_restore_interface_intercepted_tasks()
        {
            RegisterTaskAsIPersistentTask();

            var task = _windsor.Resolve<ITask>(new { count = 1 });

            //var serializedTask = _serialize.Serialize(task);

            //var desialized = _serialize.Deserialize(serializedTask);

            //Assert.That(desialized, Is.InstanceOf<IProxyTargetAccessor>());
        }

        [Test]
        public void Can_persist_and_restore_non_registered_task()
        {
            var task = new PersistentTask(1);

            //var serializedTask = _serialize.Serialize(task);

            //var desialized = _serialize.Deserialize(serializedTask);

            //Assert.That(desialized, Is.InstanceOf<PersistentTask>());
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