using System;
using Codestellation.DarkFlow.Misc;
using Codestellation.DarkFlow.Schedules;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Scheduling
{
    [TestFixture]
    public class EveryTests
    {
        private TestClock _clock;

        [SetUp]
        public void Setup()
        {
            _clock = new TestClock();
            _clock.Now = new DateTimeOffset(2012,12,3,16,48,32, 300, TimeSpan.FromHours(3));

            Every.Clock = _clock;
        }

        [TearDown]
        public void TearDown()
        {
            Every.Clock = RealClock.Instance;
        }

        #region Daily
        
        [TestCase(16, 3)]
        [TestCase(14, 1)]
        public void Every_day_creates_dayly_schedule_stars_tomorrow(int hours, int offset)
        {
            var at = new DateTimeOffset(1, 1, 1, hours, 0, 0, TimeSpan.FromHours(offset));

            var schedule = Every.Day.At(at);

            Assert.That(schedule.StartAt, Is.EqualTo(Tomorrow16OClock));
        }

        [TestCase(18, 3)]
        [TestCase(16, 1)]
        public void Every_day_creates_dayly_schedule_stars_today(int hours, int offset)
        {
            var at = new DateTimeOffset(1, 1, 1, hours, 0, 0, TimeSpan.FromHours(offset));

            var schedule = Every.Day.At(at);

            Assert.That(schedule.StartAt, Is.EqualTo(Today18OClock));
        }
        #endregion

        [Test]
        public void Every_second_creates_schedule_starts_next_second()
        {
            var schedule = Every.Second;

            Assert.That(schedule.StartAt, Is.EqualTo(NextSecond));
        }

        [Test]
        public void Every_minute_creates_minutely_schedule_starts_next_minute()
        {
            var schedule = Every.Minute;

            Assert.That(schedule.StartAt, Is.EqualTo(NextMinute));
        }

        [Test]
        public void Every_hour_creates_minutely_schedule_starts_next_hour()
        {
            var schedule = Every.Hour;

            Assert.That(schedule.StartAt, Is.EqualTo(NextHour));
        }

        private DateTimeOffset NextSecond
        {
            get { return new DateTimeOffset(2012, 12, 3, 16, 48, 33, TimeSpan.FromHours(3)); }
        }

        private DateTimeOffset NextMinute
        {
            get { return new DateTimeOffset(2012, 12, 3, 16, 49, 0, TimeSpan.FromHours(3)); }
        }

        private DateTimeOffset NextHour
        {
            get { return new DateTimeOffset(2012, 12, 3, 17, 00, 00, TimeSpan.FromHours(3)); }
        }

        private DateTimeOffset Today18OClock
        {
            get
            {
                return new DateTimeOffset(_clock.Now.Date, TimeSpan.FromHours(3))
                    .Add(TimeSpan.FromHours(18));
            }
        }

        private DateTimeOffset Tomorrow16OClock
        {
            get
            {
                return new DateTimeOffset(_clock.Now.Date, TimeSpan.FromHours(3))
                    .AddDays(1)
                    .Add(TimeSpan.FromHours(16));
            }
        }



    }
}