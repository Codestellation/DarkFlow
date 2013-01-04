namespace Codestellation.DarkFlow.Execution
{
    public class QueuedExecutor : LimitedConcurrencyExecutor
    {
        public QueuedExecutor(ITaskRepository taskRepository, ITaskReleaser releaser)
            : base(taskRepository, releaser ,1)
        {
            
        }
    }
}