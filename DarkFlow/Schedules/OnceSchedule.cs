using System;

namespace Codestellation.DarkFlow.Schedules
{
    [Serializable]
    public class OnceSchedule : Schedule
    {
        private readonly DateTimeOffset _startAt;
        private volatile bool _started;

        public OnceSchedule(DateTimeOffset startAt)
        {
            _startAt = startAt;
        }

        public override DateTimeOffset StartAt
        {
            get { return _startAt; }
        }

        public override void StartedAt(DateTimeOffset startTime)
        {
            _started = true;
        }

        public override bool SchedulingRequired
        {
            get { return _started == false; }
        }

        public override string ToString()
        {
            return string.Format("Once: {0}", _startAt);
        }
    }
}