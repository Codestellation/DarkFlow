using System;

namespace Codestellation.DarkFlow.Triggers
{
    public class PeriodicalTrigger : TimerBasedTrigger
    {
        private readonly DateTime? _firstStart;
        private readonly TimeSpan _period;

        public PeriodicalTrigger(string name, DateTime? firstStart, TimeSpan period)
            : base(name)
        {
            if (firstStart.HasValue && firstStart.Value.Kind == DateTimeKind.Unspecified)
            {
                throw new ArgumentException("DateTimeKind should be Local or Utc. Unspecified is not supported.",  "firstStart");
            }
            if (period <= TimeSpan.FromSeconds(0))
            {
                throw new ArgumentException("Time interval should be positive.", "period");
            }

            _firstStart = firstStart;
            _period = period;
        }

        public override DateTime? FirstStart
        {
            get { return _firstStart; }
        }

        public override TimeSpan? Period
        {
            get { return _period; }
        }
    }

    
}