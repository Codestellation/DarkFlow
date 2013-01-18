using Codestellation.DarkFlow.Execution;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class QueuedExecutorBuilder : ExecutorBuilder
    {
        protected internal QueuedExecutorBuilder()
        {
            
        }

        protected override ExecutorContainer InstantiateExecutor()
        {
            Executor = new OrderedExecutor(Repository, Releaser);
            return new ExecutorContainer(Executor, Disposables);
        }
    }
}