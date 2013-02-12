namespace Codestellation.DarkFlow.Execution
{
    public interface ITaskQueue
    {
        string Name { get; }

        void Enqueue(ITask task);
    }
}