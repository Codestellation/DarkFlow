using System;
using Codestellation.DarkFlow.Execution;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class LimitedConcurrencyBuilder : ExecutorBuilder
    {
        private int _maxThreads = Environment.ProcessorCount;

        protected internal LimitedConcurrencyBuilder()
        {
            
        }

        protected override ExecutorContainer InstantiateExecutor()
        {
            Executor = new LimitedConcurrencyExecutor(Repository, Releaser, _maxThreads);
            return new ExecutorContainer(Executor, Disposables);
        }

        public LimitedConcurrencyBuilder Max(int threads)
        {
            _maxThreads = threads;
            return this;
        }
    }
}