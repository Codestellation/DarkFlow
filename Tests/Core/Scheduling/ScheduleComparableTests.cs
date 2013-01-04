using System;
using System.Collections.Generic;
using Codestellation.DarkFlow.Schedules;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Scheduling
{
    [TestFixture]
    public class ScheduleComparableTests
    {
        [Test, Ignore]
        public void Comparable_can_compare_items()
        {
            var mid = new OnceSchedule(DateTimeOffset.Now);
            var last = new OnceSchedule(DateTimeOffset.Now.AddDays(1));
            var first = new OnceSchedule(DateTimeOffset.Now.AddDays(-1));

            var sortedList = new List<Schedule>{ mid, last, first };
            
            sortedList.Sort();

            Assert.That(sortedList[0], Is.SameAs(first));
            Assert.That(sortedList[1], Is.SameAs(mid));
            Assert.That(sortedList[2], Is.SameAs(last));
        }
    }
}