namespace Codestellation.DarkFlow.Execution
{
    public class OrderedExecutor : LimitedConcurrencyExecutor
    {
        public OrderedExecutor(ITaskRepository taskRepository, ITaskReleaser releaser)
            : base(taskRepository, releaser ,1)
        {
            
        }
    }
}