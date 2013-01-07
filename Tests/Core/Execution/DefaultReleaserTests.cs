using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class DefaultReleaserTests
    {
        private DefaultReleaser _releaser;

        [SetUp]
        public void SetUp()
        {
            _releaser = new DefaultReleaser();
        }
        [Test]
        public void Do_no_throw_if_task_is_no_disposable()
        {
            Assert.DoesNotThrow(() => _releaser.Release(new LongRunningTask(false)));
        }   
        
        [Test]
        public void Do_no_throw_if_task_dispose_throws()
        {
            Assert.DoesNotThrow(() => _releaser.Release(DisposableTask.ThrowOnDispose()));
        }

        [Test]
        public void Execute_dispose_on_task()
        {
            var task = DisposableTask.DisposeNormally();

            _releaser.Release(task);

            Assert.That(task.Disposed, Is.True);
        }
    }
}