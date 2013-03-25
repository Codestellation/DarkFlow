using System;
using System.Collections.Generic;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Codestellation.DarkFlow.CastleWindsor.Impl;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Matchers;
using Codestellation.DarkFlow.Scheduling;

namespace Codestellation.DarkFlow.CastleWindsor
{
    /// <summary>
    /// </summary>
    public class DarkFlowFacility : AbstractFacility
    {
        private ComponentRegistration<IDatabase> _databaseRegistration;
        private List<IMatcher> _matchers;
        private List<TaskQueueSettings> _queues;

        public string PersistedTaskFolder { get; set; }

        public byte? MaxThreads { get; set; }

        protected override void Init()
        {
            _queues = new List<TaskQueueSettings>();
            _matchers = new List<IMatcher>();
            Kernel.AddHandlerSelector(new TaskHandlerSelector(Kernel));

            RegisterSharedServices();

            RegisterExecutor();

            RegisterScheduler();
        }

        private void RegisterExecutor()
        {
            var maxConcurrency = (byte) (MaxThreads ?? Environment.ProcessorCount);

            if (_queues.Count == 0)
            {
                var settings = new TaskQueueSettings { Name = "default" };
                _queues.Add(settings);
            }

            foreach (TaskQueueSettings settings in _queues)
            {
                Kernel.Register(
                    Component
                        .For<ITaskQueue, IExecutionQueue>()
                        .ImplementedBy<TaskQueue>()
                        .Named(settings.Name)
                        .DependsOn(new {settings})
                        .LifestyleSingleton()
                    );
            }

            Kernel.Register(Component
                                .For<IExecutor>()
                                .ImplementedBy<Executor>()
                                .LifestyleSingleton(),
                            Component
                                .For<IMatcher>()
                                .ImplementedBy<AggregateMatcher>()
                                .LifestyleSingleton(),
                            Component
                                .For<ITaskRouter>()
                                .ImplementedBy<TaskRouter>()
                                .LifestyleSingleton(),
                            Component
                                .For<TaskDispatcher>()
                                .DependsOn(new {maxConcurrency})
                                .LifestyleSingleton());
        }


        private void RegisterSharedServices()
        {
            string persistedFolder = PersistedTaskFolder ?? ManagedEsentDatabase.DefaultTaskFolder;

            Kernel.Register(
                Component
                    .For<ITaskReleaser>()
                    .ImplementedBy<WindsorReleaser>()
                    .LifestyleSingleton(),
                Component
                    .For<IPersister>()
                    .ImplementedBy<WindsorPersister>()
                    .LifestyleSingleton(),
                _databaseRegistration ?? Component
                                             .For<IDatabase>()
                                             .ImplementedBy<ManagedEsentDatabase>()
                                             .DependsOn(new {persistedFolder})
                                             .LifestyleSingleton()
                );
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
        ///     This is useful for testing purposes.
        /// </summary>
        public DarkFlowFacility UsingInMemoryPersistence()
        {
            _databaseRegistration = Component.For<IDatabase>()
                                             .ImplementedBy<InMemoryDatabase>()
                                             .LifestyleSingleton();
            return this;
        }

        public DarkFlowFacility WithQueue(TaskQueueSettings settings)
        {
            _queues.Add(settings);
            return this;
        }

        /// <summary>
        ///     Provides custom persistence implementation to register with executor.
        /// </summary>
        /// <param name="databaseRegistration"></param>
        public DarkFlowFacility UsingCustomPersistence(ComponentRegistration<IDatabase> databaseRegistration)
        {
            _databaseRegistration = databaseRegistration;
            return this;
        }
    }
}