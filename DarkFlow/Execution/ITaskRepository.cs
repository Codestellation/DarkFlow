namespace Codestellation.DarkFlow.Execution
{
    public interface ITaskRepository
    {
        void Add(ITask task);

        void Add(IPersistentTask task);

        ITask TakeNext();
    }
}