using System;

namespace Codestellation.DarkFlow.Schedules
{
    [Serializable]
    public class PeriodicalSchedule : Schedule
    {
        private DateTimeOffset _startAt;
        private DateTimeOffset _firstRun;

        public DateTimeOffset FirstRun
        {
            get { return _firstRun; }
            internal set
            {
                _firstRun = value;
                _startAt = value;
            }
        }

        public TimeSpan Period { get; internal set; }


        public PeriodicalSchedule(DateTimeOffset firstRun, TimeSpan period)
        {
            FirstRun = firstRun;
            Period = period;
        }

        internal PeriodicalSchedule()
        {
            
        }

        public override DateTimeOffset StartAt
        {
            get { return _startAt; }
        }

        public override void StartedAt(DateTimeOffset startTime)
        {
            while (_startAt <= startTime)
            {
                _startAt = _startAt.Add(Period);
            }
        }

        public override bool SchedulingRequired
        {
            get { return true; }
        }
    }
}