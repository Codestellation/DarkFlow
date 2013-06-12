using System;

namespace Codestellation.DarkFlow.Misc
{
    public class TestClock : IClock
    {
        private DateTime _now;

        public TestClock() : this(DateTime.Now)
        {
            
        }

        public TestClock(DateTime now)
        {
                
            Now = now;
        }

        public DateTime UtcNow
        {
            get { return _now.ToUniversalTime(); }
        }

        public DateTime Now
        {
            get { return _now; }
            set
            {
                if (value.Kind == DateTimeKind.Unspecified)
                {
                    throw new ArgumentException("DateTime kind must be UTC of local. Unspecified is prohibited.");
                }
                _now = value;
            }
        }
    }
}