using System;
using System.Threading;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Scheduling;
using Codestellation.DarkFlow.Triggers;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Triggers
{
    [TestFixture]
    public class PeriodicalTriggerTests
    {
        [Test]
        public void TEST()
        {
            var executor = new SynchronousExecutor();
            var task = new LongRunningTask(false) {Name = "Added task"};

            var scheduler = new Scheduler(executor);
            var trigger = new PeriodicalTrigger("Test", null, TimeSpan.FromSeconds(1));
            trigger.AttachTask(task);

            scheduler.AddTrigger(trigger);

            Thread.Sleep(10 *1000);

            scheduler.Dispose();


            Thread.Sleep(10 * 1000);
        }
    }
}