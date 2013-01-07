using System;
using Codestellation.DarkFlow.Schedules;
using Codestellation.DarkFlow.Scheduling;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Scheduling
{
    [TestFixture]
    public class TaskSourceTests
    {
        [Test]
        public void Can_add_new_tasks_to_source()
        {
            var mid = new OnceSchedule(DateTimeOffset.Now);
            var last = new OnceSchedule(DateTimeOffset.Now.AddDays(1));
            var first = new OnceSchedule(DateTimeOffset.Now.AddDays(-1));

            DateTimeOffset nextStart = DateTimeOffset.MaxValue;
            var source = new TaskSource(s => nextStart = s);


            source.AddTask(new[]
                {
                    new ScheduledTask(last, new LongRunningTask(false)),
                    new ScheduledTask(mid, new LongRunningTask(false)),
                });

            Assert.That(nextStart, Is.EqualTo(mid.StartAt));

            //source.AddTask(new LongRunningTask(), last);
            //source.AddTask(new LongRunningTask(), first);
        }
    }
}