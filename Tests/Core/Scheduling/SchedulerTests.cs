using System;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Misc;
using Codestellation.DarkFlow.Schedules;
using Codestellation.DarkFlow.Scheduling;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Scheduling
{
    [TestFixture]
    public class SchedulerTests
    {
        private LongRunningTask _task;
        private Scheduler _scheduler;
        private TestClock _clock;
        private TestTimer _testTimer;

        [SetUp]
        public void Setup()
        {
            _clock = new TestClock {Now = Constants.FrozenNow};
            _task = new LongRunningTask{Name = "PeriodicalTask", SleepTime = 0};
            _testTimer = new TestTimer();
            _scheduler = new Scheduler(new SynchronousExecutor(), _clock, _testTimer);
        }

        [TearDown]
        public void TearDown()
        {
            _scheduler.Dispose();
        }

        [Test]
        public void Should_schedule_task()
        {
            var nextSecond = _clock.Now.NextSecond();
            _scheduler.Schedule(_task, Once.At(nextSecond));
            
            ChangeTimeAndFireCallback(nextSecond);

            Assert.That(_testTimer.ContainsTimepoint(nextSecond));
            Assert.That(_task.Executed, Is.True);
        }

        [Test]
        public void Should_schedule_periodical_task()
        {
            var firstStart = _clock.Now.NextSecond();
            var secondStart = firstStart.AddSeconds(2);
            var third = secondStart.AddSeconds(2);

            _scheduler.Schedule(_task, new PeriodicalSchedule(firstStart, TimeSpan.FromSeconds(2)));

            
            ChangeTimeAndFireCallback(firstStart);
            Assert.That(_testTimer.ContainsTimepoint(firstStart));

            ChangeTimeAndFireCallback(secondStart);
            Assert.That(_testTimer.ContainsTimepoint(secondStart));

            ChangeTimeAndFireCallback(third);
            Assert.That(_testTimer.ContainsTimepoint(third));
        }

        [Test]
        public void Should_schedule_two_once_tasks()
        {
            var nextSecond = _clock.Now.NextSecond();
            var fiveSecondsLater = _clock.Now.AddSeconds(5);

            _scheduler.Schedule(_task, Once.At(nextSecond));
            _scheduler.Schedule(_task, Once.At(fiveSecondsLater));

            ChangeTimeAndFireCallback(nextSecond);

            Assert.That(_testTimer.ContainsTimepoint(fiveSecondsLater));
        }

        private void ChangeTimeAndFireCallback(DateTimeOffset timePoint)
        {
            _clock.Now = timePoint;
            _testTimer.FireCallback(timePoint);
        }
    }
}