using System;

namespace Codestellation.DarkFlow.Triggers
{
    public class OneTimeTrigger : TimerBasedTrigger
    {
        private readonly DateTime _momentToFire;

        public OneTimeTrigger(string id, DateTime momentToFire) : base(id)
        {
            if (momentToFire.Kind == DateTimeKind.Unspecified)
            {
                throw new ArgumentException("Moment to fire should be specified in UTC or local. Unspecified datetime is not allowed");
            }
            _momentToFire = momentToFire;
        }

        protected override DateTime? FirstStart
        {
            get { return _momentToFire; }
        }

        protected override TimeSpan? Period
        {
            get { return null; }
        }
    }
}