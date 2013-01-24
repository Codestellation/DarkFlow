using Codestellation.DarkFlow.Database;

namespace Codestellation.DarkFlow.Execution
{
    public interface ITaskRepository
    {
        void SetRegion(Region region);

        void Add(ITask task);

        void Add(IPersistentTask task);

        ITask TakeNext();
    }
}