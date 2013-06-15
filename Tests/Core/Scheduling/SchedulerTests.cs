using System;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Scheduling;
using Codestellation.DarkFlow.Triggers;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Scheduling
{
    [TestFixture]
    public class SchedulerTests
    {
        private Scheduler _scheduler;
        private OneTimeTrigger _trigger;

        [SetUp]
        public void SetUp()
        {
            _scheduler = new Scheduler(new SynchronousExecutor());
            _trigger = new OneTimeTrigger("Test", DateTime.Now.AddDays(10));
        }
        
        [Test]
        public void Can_attach_trigger()
        {
            _scheduler.AttachTrigger(_trigger);
            Assert.That(_scheduler.Triggers, Has.Member(_trigger));
            Assert.That(_trigger.Started, Is.True);
        }

        [Test]
        public void Cannot_attach_trigger_twice()
        {
            _scheduler.AttachTrigger(_trigger);
            Assert.Throws<InvalidOperationException>(() => _scheduler.AttachTrigger(_trigger));
        }

        [Test]
        public void Can_detach_trigger()
        {
            _scheduler.AttachTrigger(_trigger);
            _scheduler.DetachTrigger(_trigger);

            Assert.That(_scheduler.Triggers, Has.No.Member(_trigger));
            Assert.That(_trigger.Started, Is.False);
        }


        [Test]
        public void Cannot_detach_trigger_twice()
        {
            _scheduler.AttachTrigger(_trigger);
            _scheduler.DetachTrigger(_trigger);

            Assert.Throws<InvalidOperationException>(() => _scheduler.DetachTrigger(_trigger));
        }
    }
}