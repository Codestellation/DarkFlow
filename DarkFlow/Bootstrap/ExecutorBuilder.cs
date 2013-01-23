using System;
using System.Linq;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;

namespace Codestellation.DarkFlow.Bootstrap
{
    public abstract class ExecutorBuilder
    {
        protected IDatabase Database;
        protected ITaskFactory TaskFactory;
        protected ISerializer Serializer;
        protected ITaskRepository Repository;
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
            TaskFactory = TaskFactory ?? new DefaultTaskFactory();
            Serializer = Serializer ?? new JsonSerializer(TaskFactory);
            Repository = Repository ?? new TaskRepository(Serializer, Database);
            Releaser = Releaser ?? new DefaultReleaser();
        }

        protected virtual IDisposable[] Disposables
        {
            get
            {
                var candidates = new object[] {Executor, Releaser, Repository, Database, Serializer, TaskFactory};
                return candidates.OfType<IDisposable>().ToArray();
            }
        }
    }
}