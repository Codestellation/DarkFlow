using System;

namespace Codestellation.DarkFlow.Schedules
{
    [Serializable]
    public abstract class Schedule : IComparable<Schedule>, IComparable
    {
        protected Schedule()
        {
            
        }

        /// <summary>
        /// Get next start time. 
        /// <remarks>This property should not change state on an shedule.</remarks>
        /// </summary>
        public abstract DateTimeOffset StartAt { get; }

        /// <summary>
        /// Get next start time. 
        /// <remarks>Call to this method should change state of NextStartAt.</remarks>
        /// </summary>
        public abstract void StartedAt(DateTimeOffset startTime);

        public abstract bool SchedulingRequired { get; }

        public int CompareTo(Schedule other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            return StartAt.CompareTo(StartAt);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var other = obj as Schedule;

            if (other == null)
            {
                throw new ArgumentException(string.Format("Argument obj should be convertible to {0}.", GetType()), "obj");
            }

            return CompareTo(other);
        }
    }
}
