using System;
using System.Collections.Generic;
using System.Threading;
using Codestellation.DarkFlow.Misc;
using NLog;

namespace Codestellation.DarkFlow.Scheduling
{
    public class SmartTimer : ITimer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<DateTimeOffset, Timer> _timers;
        private IClock _clock;
        private Action<DateTimeOffset> _callback;

        public SmartTimer(IClock clock)
        {
            _timers = new Dictionary<DateTimeOffset, Timer>();
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
            var timer = BuildTimer(startAt);
            if (timer == null) return;
            lock (_timers)
            {
                _timers[startAt] = timer;
            }
        }

        public Action<DateTimeOffset> Callback
        {
            get { return _callback; }
            set
            {
                _callback = value;
                Thread.MemoryBarrier();
            }
        }

        private Timer BuildTimer(DateTimeOffset startAt)
        {
            var now = _clock.Now;
            var dueTime = startAt - now;
            var dueTimeInMilliseconds = Convert.ToInt64(dueTime.TotalMilliseconds);

            //TODO remove timer creation. Do call to callback immediately.
            if (dueTimeInMilliseconds < 0)
            {
                if (Logger.IsWarnEnabled)
                {
                    Logger.Warn("Attempt to start timer in past: now '{0}', StartAt '{1}', dueTime {2}", now, startAt, dueTime);
                }
                Callback(startAt);
                return null;
            }

            return new Timer(OnTimerCallback, startAt, dueTimeInMilliseconds, -1);
        }

        private void OnTimerCallback(object state)
        {
            var startAt = (DateTimeOffset)state;
            try
            {
                var callback = Callback;
                Thread.MemoryBarrier();
                if (callback == null) return;
                callback(startAt);
            }
            finally
            {
                Timer timer;
                lock (_timers)
                {
                    timer = _timers[startAt];
                    _timers.Remove(startAt);
                }
                timer.Dispose();
            }
        }
    }
}