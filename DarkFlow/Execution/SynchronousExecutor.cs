namespace Codestellation.DarkFlow.Execution
{
    public class SynchronousExecutor : IExecutor
    {
        public void Execute(ITask task)
        {
            task.Execute();
        }
    }
}