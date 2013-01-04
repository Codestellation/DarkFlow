using System;
using System.Linq;
using Codestellation.DarkFlow.Misc;
using Codestellation.DarkFlow.Schedules;
using NLog;

namespace Codestellation.DarkFlow.Scheduling
{
    public class Scheduler : Disposable, IScheduler 
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IExecutor _executor;
        private readonly IClock _clock;
        private readonly TaskSource _taskSource;
        private ITimer _timer;

        public Scheduler(IExecutor executor) : this(executor, RealClock.Instance, new SmartTimer(RealClock.Instance))
        {
            
        }
        public Scheduler(IExecutor executor, IClock clock, ITimer timer)
        {
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }

            if (clock == null)
            {
                throw new ArgumentNullException("clock");
            } 

            if (timer == null)
            {
                throw new ArgumentNullException("timer");
            } 

            _executor = executor;
            _clock = clock;
            _timer = timer;

            _timer.Callback = OnTimerCallback;
            _taskSource = new TaskSource(OnClosestChanged);
        }

        private void OnClosestChanged(DateTimeOffset startAt)
        {
            var now = _clock.Now;
            if (startAt <= now)
            {
                Logger.Debug("StartAt is {0}. Now is {1}. Executing without scheduling", startAt, now);
                OnTimerCallback(startAt);
                return;
            }
            
            Logger.Debug("StartAt is {0}. Now is {1}. Starting new timer", startAt, now);

            _timer.CallbackAt(startAt);
        }

        public void Schedule(ITask task, Schedule schedule)
        {
            if (ReferenceEquals(task, null))
            {
                throw new ArgumentNullException("task");
            }

            if (ReferenceEquals(schedule, null))
            {
                throw new ArgumentNullException("schedule");
            }

            _taskSource.AddTask(new[]{new ScheduledTask(schedule, task) });
        }

        private void OnTimerCallback(DateTimeOffset startAt)
        {
            Logger.Debug("TimerCallback at {0}", startAt);
            try
            {
                var tasksToRun = _taskSource.TakeOnTime(startAt);

                Logger.Debug("Found {0} tasks to run at {1}", tasksToRun.Count, startAt);

                var now = _clock.Now;
                Logger.Debug("Now is {0}", now);

                foreach (var scheduledTask in tasksToRun)
                {
                    _executor.ExecuteLater(scheduledTask.Task);
                    scheduledTask.Schedule.StartedAt(now);
                }

                var tasksToReschedule = tasksToRun.Where(x => x.Schedule.SchedulingRequired).ToArray();

                Logger.Debug("{0} tasks need rescheduling.", tasksToReschedule.Length);

                _taskSource.AddTask(tasksToReschedule);
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                {
                    Logger.Error(ex);
                }
            }
        }

        protected override void ReleaseUnmanaged()
        {
            var timer = _timer;
            if (timer == null) return;

            _timer.Dispose();
            _timer = null;
        }
    }
}