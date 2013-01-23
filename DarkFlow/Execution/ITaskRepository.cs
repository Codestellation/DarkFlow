using Codestellation.DarkFlow.Database;

namespace Codestellation.DarkFlow.Execution
{
    public interface ITaskRepository
    {
        void Add(ITask task);

        void Add(IPersistentTask task, Region region);

        ITask TakeNext();
    }
}