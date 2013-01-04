using System;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Schedules
{
    public static class Every
    {
        public static IClock Clock = RealClock.Instance;

        public class DayScheduleDescriptor
        {
            public PeriodicalSchedule At(DateTimeOffset timeOffset)
            {
                var period = TimeSpan.FromDays(1);

                var now = Clock.Now.ToUniversalTime();

                var localTimeOffset = timeOffset.ToUniversalTime();

                var firstStartOffset = localTimeOffset.TimeOfDay - now.TimeOfDay;
                
                if (firstStartOffset < TimeSpan.Zero)
                {
                    firstStartOffset = firstStartOffset.Add(period);
                }

                var firstStartAt = now.Add(firstStartOffset);
                
                return new PeriodicalSchedule(firstStartAt, period);
            }

        }

        public static PeriodicalSchedule Second
        {
            get { return new PeriodicalSchedule(Clock.Now.NextSecond(), TimeSpan.FromSeconds(1)); }
        }

        public static PeriodicalSchedule Minute
        {
            get { return new PeriodicalSchedule(Clock.Now.NextMinute(), TimeSpan.FromMinutes(1)); }
        }

        public static PeriodicalSchedule Hour
        {
            get { return new PeriodicalSchedule(Clock.Now.NextHour(), TimeSpan.FromHours(1)); }
        }

        public static DayScheduleDescriptor Day
        {
            get { return new DayScheduleDescriptor(); }
        }
    }
}