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
            Executor = new QueuedExecutor(Repository, Releaser);
            return new ExecutorContainer(Executor, Disposables);
        }
    }
}