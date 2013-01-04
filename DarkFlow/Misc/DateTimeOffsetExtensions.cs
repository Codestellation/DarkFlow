using System;

namespace Codestellation.DarkFlow.Misc
{
    public static class DateTimeOffsetExtensions
    {
        public static DateTimeOffset NextSecond(this DateTimeOffset self)
        {
            return new DateTimeOffset(self.Year, self.Month, self.Day, self.Hour, self.Minute, self.Second + 1, self.Offset);
        }

        public static DateTimeOffset NextMinute(this DateTimeOffset self)
        {
            return new DateTimeOffset(self.Year, self.Month, self.Day, self.Hour, self.Minute + 1, 0, self.Offset);
        }

        public static DateTimeOffset NextHour(this DateTimeOffset self)
        {
            return new DateTimeOffset(self.Year, self.Month, self.Day, self.Hour + 1, 0, 0, self.Offset);
        }

        public static DateTimeOffset LocalTime(this int self)
        {
            if (self < 0 || 23 < self)
            {
                throw new ArgumentOutOfRangeException("self", self, "Argument must be between 0 and 23.");
            }

            var offset = DateTimeOffset.Now.Offset;
            return new DateTimeOffset(2, 1, 1, 0, 0, 0, offset).AddHours(self);
        }

        public static DateTimeOffset LocalTime(this TimeSpan self)
        {
            if (self.TotalHours >= 24)
            {
                throw new ArgumentOutOfRangeException("self", self, "Total timespan must be not less 0 and less than 24 hours.");
            }

            var offset = DateTimeOffset.Now.Offset;
            return new DateTimeOffset(2, 1, 1, 0, 0, 0, offset).Add(self);
        }
    }
}