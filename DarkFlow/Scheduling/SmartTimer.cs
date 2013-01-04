using System;
using System.Collections.Concurrent;
using System.Threading;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Scheduling
{
    public class SmartTimer : ITimer
    {
        private readonly ConcurrentDictionary<DateTimeOffset, Timer> _timers;
        private IClock _clock;

        public SmartTimer(IClock clock)
        {
            _timers = new ConcurrentDictionary<DateTimeOffset, Timer>();
            _clock = clock;
        }

        public void Dispose()
        {
            using (var handle = new AutoResetEvent(false))
            {
                foreach (var timer in _timers)
                {
                    timer.Value.Dispose(handle);
                    handle.WaitOne();
                }
            }
        }

        public void CallbackAt(DateTimeOffset startAt)
        {
            _timers.GetOrAdd(startAt, BuildTimer);
        }

        public Action<DateTimeOffset> Callback { get; set; }

        private Timer BuildTimer(DateTimeOffset startAt)
        {
            var dueTime = startAt - _clock.Now;
            return new Timer(OnTimerCallback, startAt, Convert.ToInt64(dueTime.TotalMilliseconds), -1);
        }

        private void OnTimerCallback(object state)
        {
            var startAt = (DateTimeOffset) state;
            try
            {
                var callback = Callback;
                if (callback == null) return;
                callback(startAt);
            }
            finally
            {
                Timer timer;
                if (_timers.TryRemove(startAt, out timer))
                {
                    timer.Dispose();
                }
            }
        }
    }
}