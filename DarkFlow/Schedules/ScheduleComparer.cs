using System.Collections.Generic;

namespace Codestellation.DarkFlow.Schedules
{
    public class ScheduleComparer : IComparer<Schedule>
    {
        public int Compare(Schedule x, Schedule y)
        {
            return x.CompareTo(y);
        }
    }
}