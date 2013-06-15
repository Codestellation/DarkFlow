using System;
using System.Collections.Generic;

namespace Codestellation.DarkFlow.Scheduling
{
    public partial class Scheduler
    {
        private class TriggerComparer : IEqualityComparer<Trigger>
        {
            public bool Equals(Trigger x, Trigger y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;

                return EqualityComparer<string>.Default.Equals(x.Name, y.Name);
            }

            public int GetHashCode(Trigger trigger)
            {
                if (trigger == null)
                {
                    throw new ArgumentNullException("trigger");
                }
                return trigger.Name.GetHashCode();
            }
        }
    }
}