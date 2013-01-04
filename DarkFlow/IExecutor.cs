namespace Codestellation.DarkFlow
{
    public interface IExecutor
    {
        void ExecuteLater(ITask task);

        void ExecuteReliably(IPersistentTask task);
    }
}