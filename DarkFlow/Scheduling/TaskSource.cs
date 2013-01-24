using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Codestellation.DarkFlow.Scheduling
{
    public class TaskSource
    {
        private readonly Action<DateTimeOffset> _onClosestChanged;
        private readonly Dictionary<DateTimeOffset, List<ScheduledTask>> _tasks;
        private DateTimeOffset _closestStart;

        public TaskSource(Action<DateTimeOffset> onClosestChanged)
        {
            if (onClosestChanged == null)
            {
                throw new ArgumentNullException("onClosestChanged");
            }

            _onClosestChanged = onClosestChanged;
            _tasks = new Dictionary<DateTimeOffset, List<ScheduledTask>>();
            _closestStart = DateTimeOffset.MaxValue;
        }

        public void AddTask(IEnumerable<ScheduledTask> tasks)
        {
            var groupedTasks = tasks.GroupBy(x => x.Schedule.StartAt).ToDictionary(x => x.Key, x => x.ToList());

            var localClosestStartAt = DateTimeOffset.MaxValue;

            Monitor.Enter(_tasks);

            foreach (var task in groupedTasks)
            {
                List<ScheduledTask> tasksOnTime;

                if (_tasks.TryGetValue(task.Key, out tasksOnTime) == false)
                {
                    tasksOnTime = new List<ScheduledTask>();
                    _tasks[task.Key] = tasksOnTime;
                }

                tasksOnTime.AddRange(task.Value);

                if (task.Key < localClosestStartAt)
                {
                    localClosestStartAt = task.Key;
                }
            }

            if (localClosestStartAt < _closestStart)
            {
                SetClosest(localClosestStartAt);    
            }

            Monitor.Exit(_tasks);
        }

        public ICollection<ScheduledTask> TakeOnTime(DateTimeOffset onTime)
        {
            List<ScheduledTask> results;

            Monitor.Enter(_tasks);
         
            if (_tasks.TryGetValue(onTime, out results))
            {
                _tasks.Remove(onTime);
            }

            if (onTime.Equals(_closestStart) && _tasks.Count > 0)
            {
                SetClosest(_tasks.Min(x => x.Key));
            }
            else
            {
                _closestStart = DateTimeOffset.MaxValue;
            }

            Monitor.Exit(_tasks);

            return results ?? new List<ScheduledTask>(0);
        }

        private void SetClosest(DateTimeOffset localClosestStartAt)
        {
            _closestStart = localClosestStartAt;
            _onClosestChanged(_closestStart);
        }
    }
}