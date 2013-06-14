using System;
using System.Threading;
using NLog;

namespace Codestellation.DarkFlow.Triggers
{
    public abstract class TimerBasedTrigger : TriggerTemplate, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof (TimerBasedTrigger).FullName);
        private bool _disposed;
        private Timer _timer;
        private string _message;

        protected TimerBasedTrigger(string name) : base(name)
        {
        }

        protected abstract DateTime? FirstStart { get; }

        protected abstract TimeSpan? Period { get; }

        private void OnTimerCallback(object state)
        {
            ExecuteTasks();
        }

        protected sealed override void OnStart()
        {
            Validate();
            
            var utcNow = DateTime.UtcNow;
           
            var dueTime = CalculateDueTime(utcNow);

            var period = CalculatePeriod();

            Thread.BeginCriticalRegion();

            _timer = new Timer(OnTimerCallback, null, dueTime, period);

            Thread.EndCriticalRegion();

            if (Logger.IsInfoEnabled)
            {
                Logger.Info(ToString);
            }
        }

        private void Validate()
        {
            if (FirstStart == null && Period == null)
            {
                throw new InvalidOperationException("At least one of properties FistStart or Period must be not null.");
            }
        }

        private TimeSpan CalculatePeriod()
        {
            var period = Period.HasValue ? Period.Value : TimeSpan.FromSeconds(0);
            return period;
        }

        private TimeSpan CalculateDueTime(DateTime utcNow)
        {
            var dueTime = FirstStart.HasValue ? utcNow - FirstStart.Value.ToUniversalTime() : TimeSpan.FromSeconds(0);

            dueTime = dueTime < TimeSpan.FromSeconds(0) ? TimeSpan.FromSeconds(0) : dueTime;
            return dueTime;
        }

        protected internal override void Stop()
        {
            Dispose();

            if (Logger.IsInfoEnabled)
            {
                Logger.Info("Trigger '{0}' stopped.", Name);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            Thread.BeginCriticalRegion();
            using (var timerStoppedEvent = new ManualResetEvent(false))
            {
                _timer.Dispose(timerStoppedEvent);
                timerStoppedEvent.WaitOne();
            }
            _disposed = true;
            Thread.EndCriticalRegion();
        }

        public override string ToString()
        {
            var startMessage = FirstStart.HasValue ? "at " + FirstStart.Value.ToString() : "immediately";
            var periodMessage = Period.HasValue ? " Period " + Period.Value : string.Empty;
            var message = string.Format("Trigger: '{0}'. Starts {1}.{2}", Name, startMessage, periodMessage);
            return _message ?? (_message = message);
        }
    }
}