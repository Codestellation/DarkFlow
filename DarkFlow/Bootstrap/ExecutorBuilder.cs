using System;
using System.Linq;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;

namespace Codestellation.DarkFlow.Bootstrap
{
    public abstract class ExecutorBuilder
    {
        protected IDatabase Database;
        protected ITaskReleaser Releaser;
        protected IExecutor Executor;

        public ExecutorBuilder UsingEsent()
        {
            Database = new ManagedEsentDatabase();
            return this;
        }

        public virtual ExecutorBuilder UsingInMemoryPersistence()
        {
            Database = new InMemoryDatabase();
            return this;
        }

        public ExecutorContainer Build()
        {
            FillDefaults();
            return InstantiateExecutor();
        }

        protected abstract ExecutorContainer InstantiateExecutor();

        protected virtual void FillDefaults()
        {
            Releaser = Releaser ?? new DefaultReleaser();
        }

        protected virtual IDisposable[] Disposables
        {
            get
            {
                var candidates = new object[] {Executor, Releaser, Database};
                return candidates.OfType<IDisposable>().ToArray();
            }
        }
    }
}