using System;

namespace Codestellation.DarkFlow.Triggers
{
    public class Start
    {
        public static DateTime? Immediately = null;

        public static DateTime At(TimeSpan timeOfDay, DateTimeKind dateTimeKind)
        {
            if (timeOfDay < TimeSpan.FromSeconds(0) || TimeSpan.FromDays(1) < timeOfDay)
            {
                throw new ArgumentOutOfRangeException("timeOfDay", "Time of day should be in range from 0 up to 24 hours");
            }
            if (dateTimeKind == DateTimeKind.Unspecified)
            {
                throw new ArgumentOutOfRangeException("dateTimeKind", "DateTimeKind should be UTC or Local");
            }

            var currentDateTime = dateTimeKind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;

            var result = currentDateTime.Date.Add(timeOfDay);

            //Missed in current day. Plan it tomorrow
            if (currentDateTime.TimeOfDay > timeOfDay)
            {
                result = result.AddDays(1);
            }

            return result;
        }
    }

    public static class Repeat
    {
        public static class Every
        {
            public static readonly TimeSpan Second = TimeSpan.FromSeconds(1);
            
            public static readonly TimeSpan Minute = TimeSpan.FromMinutes(1);

            public static readonly TimeSpan Hour = TimeSpan.FromHours(1);

            public static readonly TimeSpan Day = TimeSpan.FromDays(1);
        }
    }
}