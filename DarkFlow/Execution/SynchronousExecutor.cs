namespace Codestellation.DarkFlow.Execution
{
    public class SynchronousExecutor : IExecutor
    {
        public void ExecuteLater(ITask task)
        {
            task.Execute();
        }

        public void ExecuteReliably(IPersistentTask task)
        {
            task.Execute();
        }
    }
}