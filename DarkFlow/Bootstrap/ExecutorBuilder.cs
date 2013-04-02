using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class ExecutorBuilder
    {
        private readonly List<QueuedExecutorSettings> _queuedExecutors;
        private IMatcherBuilder _routeMatcherBuilder;
        private IMatcherBuilder _persisterMatcherBuilder;
        private byte _maxConcurrency;
        private bool _built;
        private IPersister _persister;
        private QueuedExecutor[] _executors;
        private ITaskRouter _router;
        private TaskDispatcher _dispatcher;
        private Executor _executor;
        private DisposableContainer _container;
        private IDatabase _dataBase;

        public ExecutorBuilder()
        {
            _queuedExecutors = new List<QueuedExecutorSettings>();
        }

        public ExecutorBuilder WithQueuedExecutors(QueuedExecutorSettings queuedExecutorSettings)
        {
            _queuedExecutors.Add(queuedExecutorSettings);
            return this;
        }

        public ExecutorBuilder RouteTasks(Action<AggregateMatcherBuilder> buildAction)
        {
            var aggregateMatcherBuilder = new AggregateMatcherBuilder();
            _routeMatcherBuilder = aggregateMatcherBuilder;
            buildAction(aggregateMatcherBuilder);
            return this;
        }

        public ExecutorBuilder PersistTasks(Action<AggregateMatcherBuilder> buildAction)
        {
            var aggregateMatcherBuilder = new AggregateMatcherBuilder();
            _persisterMatcherBuilder = aggregateMatcherBuilder;
            buildAction(aggregateMatcherBuilder);
            return this;
        }

        public ExecutorBuilder MaxConcurrency(byte maxConcurrency)
        {
            _maxConcurrency = maxConcurrency;
            return this;
        }

        public ExecutorBuilder MaxConcurrency(int maxConcurrency)
        {
            return MaxConcurrency((byte) maxConcurrency);
        }

        public ExecutorBuilder UseDatabase(IDatabase database)
        {
            _dataBase = database;
            return this;
        }

        public ExecutorBuilder UseInMemoryDatabase()
        {
            _dataBase = new InMemoryDatabase();
            return this;
        }

        public ExecutorBuilder UseManagedEsentDatabase()
        {
            _dataBase = new ManagedEsentDatabase();
            return this;
        }

        public IExecutor Build()
        {
            if (_built)
            {
                throw new InvalidOperationException("Do not call Build more than once.");
            }
            _built = true;

            BuildPersister();

            BuildExecutors();

            BuildRouter();

            _dispatcher = new TaskDispatcher(_maxConcurrency, _executors);

            _executor = new Executor(_router, _dispatcher, DefaultReleaser.Instance);

            _container = new DisposableContainer(_executor, _executor, _dispatcher, _router, _executors, _persister, _dataBase);

            return _container;
        }

        private void BuildRouter()
        {
            var routerMatcher = _routeMatcherBuilder.Build();

            _router = new TaskRouter(routerMatcher, _executors);
        }

        private void BuildExecutors()
        {
            _executors = _queuedExecutors
                .Select(settings => new QueuedExecutor(settings, _persister))
                .ToArray() ;
        }

        private void BuildPersister()
        {
            _persister = NullPersister.Instance;

            if (_persisterMatcherBuilder == null || _dataBase == null) return;
            
            _persister = new Persister(_dataBase, _persisterMatcherBuilder.Build());
        }
    }
}