using System;
using System.Collections.Generic;
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

        public void AttachTrigger(Trigger trigger)
        {
            EnsureNotNull(trigger);
            if (CollectionUtils.ThreadSafeAdd(ref _triggers, trigger))
            {
                trigger.Start(TriggerCallback);
                return;
            }
            var message = string.Format("Trigger named '{0}' already attached.", trigger.Name);
            throw new InvalidOperationException(message);
        }

        public void DetachTrigger(Trigger trigger)
        {
            EnsureNotNull(trigger);
            if (CollectionUtils.ThreadSafeRemove(ref _triggers, trigger))
            {
                trigger.Stop();
                return;
            }
            var message = string.Format("Trigger named '{0}' not found.", trigger.Name);
            throw new InvalidOperationException(message);
        }

        private static void EnsureNotNull(Trigger trigger)
        {
            if(trigger != null) return;
            throw new ArgumentNullException("trigger");
        }

        private void TriggerCallback(ITask task)
        {
            _executor.Execute(task);
        }

        public void Dispose()
        {
            foreach (var trigger in _triggers)
            {
                DetachTrigger(trigger);
            }
        }
    }
}