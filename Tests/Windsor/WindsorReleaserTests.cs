using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Codestellation.DarkFlow.CastleWindsor;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Windsor
{
    [TestFixture]
    public class WindsorReleaserTests
    {
        private WindsorContainer _windsor;
        private ITaskReleaser _releaser;

        [SetUp]
        public void Setup()
        {
            _windsor = new WindsorContainer();

            var facility = new DarkFlowFacility()
                .RouteTasks(x => x.ByNamespace("Codestellation.*").To("someExecutor"))
                .PersistTasks(x =>
                {
                    //TODO: Calls to To() are ugly, because they are completely ignored. Need more elegant solution to this.
                    x.ByNamespace("Codestellation.*").To("xx");
                    x.MarkedWith(typeof(MarkerAttribute)).To("xx");
                });

            _windsor.AddFacility(facility);

            _windsor.Register(
                Component
                    .For<TaskInterceptor>()
                    .LifestyleTransient());

            _releaser = _windsor.Resolve<ITaskReleaser>();
        }

        [TearDown]
        public void TearDown()
        {
            _windsor.Dispose();
        }

        [Test]
        public void Should_dispose_tracked_tasks()
        {
            _windsor.Register(
                Component
                    .For<DisposableTask>()
                    .LifestyleTransient());

            var task = _windsor.Resolve<DisposableTask>();

            _releaser.Release(task);

            Assert.That(task.Disposed, Is.True);
        }


        [Test]
        public void Should_dispose_non_tracked_tasks()
        {
            var task = new DisposableTask();

            _releaser.Release(task);

            Assert.That(task.Disposed, Is.True);
        }

    }
}