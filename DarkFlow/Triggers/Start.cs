using System;

namespace Codestellation.DarkFlow.Triggers
{
    public class Start
    {
        public static DateTime? Immediately = null;

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