using System;

namespace Codestellation.DarkFlow.Misc
{
    public class RealClock : IClock
    {
        public static readonly RealClock Instance  = new RealClock();
        
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }
    }
}