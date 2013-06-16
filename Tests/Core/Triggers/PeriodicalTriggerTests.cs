using System;
using Codestellation.DarkFlow.Triggers;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Triggers
{
    [TestFixture]
    public class PeriodicalTriggerTests : ITask
    {
        private PeriodicalTrigger _trigger;

        [SetUp]
        public void Setup()
        {
            _trigger = new PeriodicalTrigger("Test", null, TimeSpan.FromMilliseconds(10));
        }

        [Test]
        public void Attaches_tasks_properly()
        {
            _trigger.AttachTask(this);
            Assert.That(_trigger.AttachedTasks, Has.Member(this));
        }

        [Test]
        public void Does_not_allow_to_attach_task_twice()
        {
            _trigger.AttachTask(this);

            Assert.Throws<InvalidOperationException>(() => _trigger.AttachTask(this));
        }

        [Test]
        public void Detaches_task_properly()
        {
            _trigger.AttachTask(this);
            _trigger.DetachTask(this);

            Assert.That(_trigger.AttachedTasks, Has.No.Member(this));
        }
        

        [Test]
        public void Throws_if_removes_non_attached_task()
        {
            Assert.Throws<InvalidOperationException>(() => _trigger.DetachTask(this));
        }

        [Test]
        public void Throws_if_removes_attached_task_twice()
        {
            _trigger.AttachTask(this);
            _trigger.DetachTask(this);
            Assert.Throws<InvalidOperationException>(() => _trigger.DetachTask(this));
        }

        [Test]
        public void Build_extensions_tests()
        {
            var trigger = new PeriodicalTrigger("xxx", Start.Immediately, Repeat.Every.Minute);
                
            Assert.That(trigger.FirstStart, Is.EqualTo(null));
            Assert.That(trigger.Period, Is.EqualTo(TimeSpan.FromMinutes(1)));
        }


        public void Execute()
        {

        }
    }
}