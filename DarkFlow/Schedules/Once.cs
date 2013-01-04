using System;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Schedules
{
    public static class Once
    {
        public static IClock Clock = RealClock.Instance;

        public static OnceSchedule At(DateTimeOffset dateTime)
        {
            return new OnceSchedule(dateTime);
        }

        public static OnceSchedule After(TimeSpan timeSpan)
        {
            return new OnceSchedule(Clock.Now.Add(timeSpan));
        }
    }
}