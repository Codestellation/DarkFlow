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
        private readonly IClock _clock;
        private Action<DateTimeOffset> _callback;
        private volatile bool _disposed;

        public SmartTimer(IClock clock)
        {
            _timers = new Dictionary<DateTimeOffset, Timer>();
            _clock = clock;
        }

        public void Dispose()
        {
            _disposed = true;
            CountdownEvent handle = null;

            lock (_timers)
            {
                if (_timers.Count > 0)
                {
                    handle = new CountdownEvent(_timers.Count);
                    foreach (var timer in _timers)
                    {
                        timer.Value.Dispose(handle.WaitHandle);
                    }
                }
            }

            if (handle != null)
            {
                handle.Wait();
            }
        }

        public void CallbackAt(DateTimeOffset startAt)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            
            lock (_timers)
            {
                Timer timer;
                if (!_timers.TryGetValue(startAt, out timer))
                {
                    timer = BuildTimer(startAt);
                    if (timer == null) return;
                }
                _timers[startAt] = timer;
            }
        }

        public Action<DateTimeOffset> Callback
        {
            get { return _callback; }
            set { Interlocked.Exchange(ref _callback, value); }
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
                    Logger.Warn("Attempt to start timer in past: now '{0:yyyy.MM.dd HH:mm:ss.ffff K}', StartAt '{1:yyyy.MM.dd HH:mm:ss.ffff K}', dueTime {2:yyyy.MM.dd HH:mm:ss.ffff K}", now, startAt, dueTime);
                }
                Callback(startAt);
                return null;
            }

            return new Timer(OnTimerCallback, startAt, dueTimeInMilliseconds, -1);
        }

        private void OnTimerCallback(object state)
        {
            var startAt = (DateTimeOffset)state;

            Timer timer;

            lock (_timers)
            {
                if (_timers.TryGetValue(startAt, out timer))
                {
                    _timers.Remove(startAt);
                }
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Callback for timer fired at {0:yyyy.MM.dd HH:mm:ss.ffff K}", startAt);
            }

            try
            {
                var callback = Callback;
                if (callback == null) return;
                callback(startAt);
            }
            finally
            {
                if (timer != null)
                {
                    timer.Dispose();
                }
            }
        }
    }
}