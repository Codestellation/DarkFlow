using System;
using System.Threading;
using Codestellation.DarkFlow.Misc;
using Codestellation.DarkFlow.Scheduling;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Scheduling
{
    [TestFixture]
    public class SmartTimerTests
    {
        private ManualResetEventSlim _callbackInvoked;

        [Test]
        public void Should_callback_if_due_date_missed()
        {
            _callbackInvoked = new ManualResetEventSlim(false);

            Thread.MemoryBarrier();

            var now = new DateTimeOffset(2010, 01, 01, 10, 0, 0, TimeSpan.FromHours(1));

            var clock = new TestClock(now);
            var timer = new SmartTimer(clock) {Callback = OnTimerCallback};
            
            timer.CallbackAt(now.AddHours(-1));

            _callbackInvoked.Wait(TimeSpan.FromSeconds(5));

            Assert.That(_callbackInvoked.IsSet, Is.True, "Callback was not invoked");
        }


        [Test, Timeout(1000)]
        public void Should_not_hang_on_dispose()
        {
            var clock = RealClock.Instance;
            var timer = new SmartTimer(clock) { Callback = OnTimerCallback };

            for (int i = 1; i <= 100; i++)
            {
                timer.CallbackAt(DateTimeOffset.Now.AddHours(i));
            }

            timer.Dispose();
        }

        private void OnTimerCallback(DateTimeOffset obj)
        {
            _callbackInvoked.Set();
        }
    }
}