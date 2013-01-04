namespace Codestellation.DarkFlow.Execution
{
    public interface ITaskFactory
    {
        IPersistentTask Create(string taskType, object state);

        string GetRealType(IPersistentTask task);
    }
}