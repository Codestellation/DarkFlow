using System;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Codestellation.DarkFlow.CastleWindsor;
using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Tests.Core.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Windsor
{
    //TODO: Remove duplication
    [TestFixture]
    public class WindsorPersisterTests
    {
        private WindsorContainer _windsor;
        private IPersister _persister;
        private Identifier _id;

        [SetUp]
        public void Setup()
        {
            _windsor = new WindsorContainer();
            

            var facility = new DarkFlowFacility()
                .PersistTasks(x =>
                    {
                        //TODO: Calls to To() are ugly, because they are completely ignored. Need more elegant solution to this.
                        x.ByNamespace("Codestellation.*").To("xx");
                        x.MarkedWith(typeof (MarkerAttribute)).To("xx");
                    });

            _windsor.AddFacility(facility);

            _windsor.Register(
                Component
                    .For<TaskInterceptor>()
                    .LifestyleTransient());

            _persister = _windsor.Resolve<IPersister>();
            _id = new Identifier(Guid.NewGuid(), new Region("test"));
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
                                  .LifestyleTransient());
        }

        [Test]
        public void Can_persist_and_restore_class_intercepted_tasks()
        {
            RegisterTaskAsSelf();

            var task = _windsor.Resolve<PersistentTaskWithDependency>();

            _persister.Persist(_id, task);

            var loaded = _persister.Get(_id);

            Assert.That(loaded, Is.InstanceOf<PersistentTaskWithDependency>());
        }

        [Test]
        public void Can_persist_and_restore_non_registered_task()
        {
            var task = new PersistentTask(1);

            _persister.Persist(_id, task);

            var loaded = _persister.Get(_id);

            Assert.That(loaded, Is.InstanceOf<PersistentTask>());
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