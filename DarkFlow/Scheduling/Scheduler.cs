using System;
using System.Collections.Generic;
using System.Threading;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Scheduling
{
    public class Scheduler : IScheduler, IDisposable
    {
        private class TriggerComparer : IEqualityComparer<Trigger>
        {
            public bool Equals(Trigger x, Trigger y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;

                return EqualityComparer<string>.Default.Equals(x.Id, y.Id);
            }

            public int GetHashCode(Trigger trigger)
            {
                if (trigger == null)
                {
                    throw new ArgumentNullException("trigger");
                }
                return trigger.Id.GetHashCode();
            }
        }
        private readonly IExecutor _executor;
        private HashSet<Trigger> _triggers;

        public Scheduler(IExecutor executor)
        {
            _executor = executor;
            _triggers = new HashSet<Trigger>(new TriggerComparer());
        }

        public IEnumerable<Trigger> Triggers
        {
            get { return _triggers; }
        }

        public void AddTrigger(Trigger trigger)
        {
            if (CollectionUtils.ThreadSafeAdd(ref _triggers, trigger))
            {
                trigger.Start(TriggerCallback);
            }
        }

        private void TriggerCallback(ITask task)
        {
            _executor.Execute(task);
        }

        public void RemoveTrigger(Trigger trigger)
        {
            if (CollectionUtils.ThreadSafeRemove(ref _triggers, trigger))
            {
                trigger.Stop();
            }
        }

        public void Dispose()
        {
            foreach (var trigger in _triggers)
            {
                RemoveTrigger(trigger);
            }
        }
    }
}