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
            _windsor.AddFacility<ThreadingFacility>(x => x.UsingInMemoryPersistence());

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