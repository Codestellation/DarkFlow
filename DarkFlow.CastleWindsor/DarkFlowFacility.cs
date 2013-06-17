using System;
using System.Collections.Generic;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Codestellation.DarkFlow.Bootstrap;
using Codestellation.DarkFlow.CastleWindsor.Impl;
using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Scheduling;
using Codestellation.DarkFlow.Stat;

namespace Codestellation.DarkFlow.CastleWindsor
{
    public class DarkFlowFacility : AbstractFacility
    {
        private ComponentRegistration<IDatabase> _databaseRegistration;
        private readonly List<QueuedExecutorSettings> _queuedExecutors;
        private byte? _maxConcurrency;
        private IMatcherBuilder _routerMatcherBuilder;
        private IMatcherBuilder _persisterMatchersBuilder;
        private bool _registerMonitor;

        public string PersistedTaskFolder { get; set; }

        public DarkFlowFacility()
        {
            _queuedExecutors = new List<QueuedExecutorSettings>();
        }

        protected override void Init()
        {
            Kernel.AddHandlerSelector(new TaskHandlerSelector(Kernel));

            RegisterPersistenceComponents();

            RegisterDispatcher();

            RegisterQueuedExecutors();

            RegisterRouter();

            RegisterExecutor();

            RegisterScheduler();

            RegisterMonitor();
        }

        private void RegisterMonitor()
        {
            if (_registerMonitor)
            {
                Kernel.Register(
                    Component
                        .For<IMonitor>()
                        .ImplementedBy<Monitor>()
                        .LifestyleSingleton()
                    );
            }

        }

        private void RegisterRouter()
        {
            if (_routerMatcherBuilder == null)
            {
                string message = "No route was registered. Use DarkFlowFacility.RouteTasks() to register at least one route.";
                throw new InvalidOperationException(message);
            }
            var matcher = _routerMatcherBuilder.Build();

            Kernel.Register(
                Component
                    .For<ITaskRouter>()
                    .ImplementedBy<TaskRouter>()
                    .DependsOn(new { matcher })
                    .LifestyleSingleton());
        }

        private void RegisterQueuedExecutors()
        {
            if (_queuedExecutors.Count == 0)
            {
                var settings = new QueuedExecutorSettings { Name = "default" };
                _queuedExecutors.Add(settings);
            }

            foreach (QueuedExecutorSettings settings in _queuedExecutors)
            {
                Kernel.Register(
                    Component
                        .For<IExecutorImplementation, IExecutionQueue>()
                        .ImplementedBy<QueuedExecutor>()
                        .Named(settings.Name)
                        .DependsOn(new { settings })
                        .LifestyleSingleton()
                    );
            }
        }

        private void RegisterDispatcher()
        {
            var maxConcurrency = (byte)(_maxConcurrency ?? Environment.ProcessorCount);

            Kernel.Register(
                Component
                    .For<TaskDispatcher>()
                    .DependsOn(new {maxConcurrency})
                    .LifestyleSingleton());
        }


        private void RegisterExecutor()
        {
            Kernel.Register(
                Component
                    .For<IExecutor>()
                    .ImplementedBy<Executor>()
                    .LifestyleSingleton(),
                Component
                    .For<ITaskReleaser>()
                    .ImplementedBy<WindsorReleaser>()
                    .LifestyleSingleton());
        }

        private void RegisterPersistenceComponents()
        {
            if (_persisterMatchersBuilder == null)
            {
                Kernel.Register(
                    Component
                        .For<IPersister>()
                        .Instance(NullPersister.Instance)
                        .LifestyleSingleton());
            }
            else
            {
                var matcher = _persisterMatchersBuilder.Build();

                var persisterRegistration = Component
                    .For<IPersister>()
                    .ImplementedBy<WindsorPersister>()
                    .DependsOn(new {matcher})
                    .LifestyleSingleton();

                string persistedFolder = PersistedTaskFolder ?? ManagedEsentDatabase.DefaultTaskFolder;

                var databaseRegistration = _databaseRegistration ?? Component
                                                                        .For<IDatabase>()
                                                                        .ImplementedBy<ManagedEsentDatabase>()
                                                                        .DependsOn(new {persistedFolder})
                                                                        .LifestyleSingleton();

                Kernel.Register(persisterRegistration, databaseRegistration);
            }
        }

        private void RegisterScheduler()
        {
            Kernel.Register(
                Component
                    .For<IScheduler>()
                    .ImplementedBy<Scheduler>()
                    .LifestyleSingleton()
                );
        }

        /// <summary>
        /// This is useful for testing purposes.
        /// </summary>
        public DarkFlowFacility UsingInMemoryPersistence()
        {
            _databaseRegistration = Component.For<IDatabase>()
                                             .ImplementedBy<InMemoryDatabase>()
                                             .LifestyleSingleton();
            return this;
        }

        public DarkFlowFacility WithQueuedExecutor(params QueuedExecutorSettings[] settings)
        {
            foreach (var setting in settings)
            {
                setting.Validate();
            }

            _queuedExecutors.AddRange(settings);
            return this;
        }

        /// <summary>
        /// Provides custom persistence implementation to register with executor.
        /// </summary>
        /// <param name="databaseRegistration"></param>
        public DarkFlowFacility UsingCustomPersistence(ComponentRegistration<IDatabase> databaseRegistration)
        {
            _databaseRegistration = databaseRegistration;
            return this;
        }

        public DarkFlowFacility MaxConcurrency(byte maxConcurrency)
        {
            _maxConcurrency = maxConcurrency;
            return this;
        }

        public DarkFlowFacility RouteTasks(Action<AggregateMatcherBuilder> buildAction)
        {
            var aggregateMatcherBuilder = new AggregateMatcherBuilder();
            _routerMatcherBuilder = aggregateMatcherBuilder;
            buildAction(aggregateMatcherBuilder);
            return this;
        }

        public DarkFlowFacility RouteTasks(IMatcherBuilder matcherBuilder)
        {
            _routerMatcherBuilder = matcherBuilder;
            return this;
        }

        public DarkFlowFacility PersistTasks(Action<AggregateMatcherBuilder> buildAction)
        {
            var builder = new AggregateMatcherBuilder();
            _persisterMatchersBuilder = builder;
            buildAction(builder);
            return this;
        }

        public DarkFlowFacility PersistTasks(IMatcherBuilder builder)
        {
            _persisterMatchersBuilder = builder;
            return this;
        }

        public DarkFlowFacility EnableMonitoring()
        {
            _registerMonitor = true;
            return this;
        }
    }
}