using Codestellation.DarkFlow.Schedules;

namespace Codestellation.DarkFlow
{
    public interface IScheduler
    {
        void Schedule(ITask task, Schedule schedule);
    }
}