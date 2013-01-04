using System;

namespace Codestellation.DarkFlow.Misc
{
    public class TestClock : IClock
    {
        public TestClock() : this(DateTimeOffset.Now)
        {
        }

        public TestClock(DateTimeOffset now)
        {
            Now = now;
        }

        public DateTimeOffset Now { get; set; }
    }
}