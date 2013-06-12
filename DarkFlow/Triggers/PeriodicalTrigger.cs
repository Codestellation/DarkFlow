﻿using System;

namespace Codestellation.DarkFlow.Triggers
{
    public class PeriodicalTrigger : TimerBasedTrigger
    {
        private readonly DateTime? _firstStart;
        private readonly TimeSpan _period;

        public PeriodicalTrigger(string id, DateTime? firstStart, TimeSpan period)
            : base(id)
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

        protected override DateTime? FirstStart
        {
            get { return _firstStart; }
        }

        protected override TimeSpan? Period
        {
            get { return _period; }
        }
    }
}