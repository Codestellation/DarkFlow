using System;
using Codestellation.DarkFlow.Schedules;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Scheduling
{
    [TestFixture]
    public class OnceScheduleTests
    {
        private DateTime _today;
        private DateTimeOffset _firstRun;
        private OnceSchedule _schedule;

        [SetUp]
        public void Setup()
        {
            _today = DateTime.Today;
            _firstRun = new DateTimeOffset(_today.Year, _today.Month, _today.Day, 12, 20, 30, TimeSpan.FromSeconds(0));
            _schedule = new OnceSchedule(_firstRun);
        }

        [Test]
        public void On_scheduled_should_set_next_run_to_null()
        {
            _schedule.StartedAt(_firstRun.AddSeconds(3));
            Assert.That(_schedule.StartAt, Is.EqualTo(_firstRun));
            Assert.That(_schedule.SchedulingRequired, Is.False);
        }

        [Test]
        public void Before_scheduled_next_start_is_equal_to_first_run()
        {
            Assert.That(_schedule.StartAt, Is.EqualTo(_firstRun));
            Assert.That(_schedule.SchedulingRequired, Is.True);
        }
    }
}