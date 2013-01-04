namespace Codestellation.DarkFlow
{
    public interface IPersistentTask : ITask
    {
        object PersistentState { get; }
    }
}