using System;
using Codestellation.DarkFlow.Stat;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core
{
    [TestFixture]
    public class MonitorTests
    {
        [Test]
        public void Can_monitor_statistical_data()
        {
            var monitor = new Monitor();
            
            monitor.Increment("Test", TimeSpan.FromSeconds(2));
            monitor.Increment("Test", TimeSpan.FromSeconds(2));
            monitor.Increment("Test", TimeSpan.FromSeconds(5));

            var counter = monitor.GetCounter("Test");

            Assert.That(counter.Entries, Is.EqualTo(3));
            Assert.That(counter.TotalDuration, Is.EqualTo(TimeSpan.FromSeconds(9)));
            Assert.That(counter.MaxDuration, Is.EqualTo(TimeSpan.FromSeconds(5)));
            Assert.That(counter.MinDuration, Is.EqualTo(TimeSpan.FromSeconds(2)));
            Assert.That(counter.AvgDuration, Is.EqualTo(TimeSpan.FromSeconds(3)));
        }
    }
}