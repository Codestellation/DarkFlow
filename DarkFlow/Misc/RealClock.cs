using System;

namespace Codestellation.DarkFlow.Misc
{
    public class RealClock : IClock
    {
        public static readonly RealClock Instance  = new RealClock();
        
        public DateTimeOffset Now
        {
            get { return DateTimeOffset.Now; }
        }
    }
}