using System;
using Codestellation.DarkFlow.Schedules;

namespace Codestellation.DarkFlow.Scheduling
{
    public class ScheduledTask
    {
        private readonly Schedule _schedule;
        private readonly ITask _task;

        public ScheduledTask(Schedule schedule, ITask task)
        {
            if (schedule == null)
            {
                throw new ArgumentNullException("schedule");
            }

            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            _schedule = schedule;
            _task = task;
        }

        public Schedule Schedule
        {
            get { return _schedule; }
        }

        public ITask Task
        {
            get { return _task; }
        }
    }
}