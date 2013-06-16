using System;

namespace Codestellation.DarkFlow.Triggers
{
    public class OneTimeTrigger : TimerBasedTrigger
    {
        private readonly DateTime _momentToFire;

        public OneTimeTrigger(string name, DateTime momentToFire) : base(name)
        {
            if (momentToFire.Kind == DateTimeKind.Unspecified)
            {
                throw new ArgumentException("Moment to fire should be specified in UTC or local. Unspecified datetime is not allowed");
            }
            _momentToFire = momentToFire;
        }

        public override DateTime? FirstStart
        {
            get { return _momentToFire; }
        }

        public override TimeSpan? Period
        {
            get { return null; }
        }
    }
}