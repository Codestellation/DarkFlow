using System;
using Codestellation.DarkFlow.Schedules;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Scheduling
{
    [TestFixture]
    public class PeriodicalScheduleTests
    {
        private DateTime _today;
        private DateTimeOffset _firstRun;
        private TimeSpan _period;
        private PeriodicalSchedule _schedule;

        [SetUp]
        public void Setup()
        {
            _today = DateTime.Today;
            _firstRun = new DateTimeOffset(_today.Year, _today.Month, _today.Day, 12, 20, 30, TimeSpan.FromSeconds(0));
            _period = TimeSpan.FromSeconds(30);
            _schedule = new PeriodicalSchedule(_firstRun, _period);
        }

        [TestCase(0)]
        [TestCase(3)]
        public void On_scheduled_should_set_next_run_time(int startedAt)
        {
            var nextRun = _firstRun.AddSeconds(30);
            _schedule.StartedAt(_firstRun.AddSeconds(startedAt));
            Assert.That(_schedule.StartAt, Is.EqualTo(nextRun));
        }
        
        [Test]
        public void Before_scheduled_next_start_is_equal_to_first_run()
        {
            Assert.That(_schedule.StartAt, Is.EqualTo(_firstRun));
        }
    }
}