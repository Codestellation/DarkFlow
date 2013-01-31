namespace Codestellation.DarkFlow.Execution
{
    public interface ITaskQueue
    {
        bool CanEnqueue(ITask task);

        void Enqueue(ITask task);
    }
}