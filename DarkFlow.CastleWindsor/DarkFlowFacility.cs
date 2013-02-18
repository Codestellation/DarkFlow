using System;
using System.Collections.Generic;
using Castle.Facilities.Startable;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Codestellation.DarkFlow.CastleWindsor.Impl;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Misc;
using Codestellation.DarkFlow.Scheduling;

namespace Codestellation.DarkFlow.CastleWindsor
{
    /// <summary>
    /// 
    /// </summary>
    public class DarkFlowFacility : AbstractFacility
    {
        private ComponentRegistration<IDatabase> _databaseRegistration;
        private ComponentRegistration<IExecutor> _executorRegistration;
        private List<TaskQueue> _queues;
        private List<IMatcher> _matchers;

        public string PersistedTaskFolder { get; set; }

        public byte? MaxThreads { get; set; }

        protected override void Init()
        {
            _queues = new List<TaskQueue>();
            _matchers = new List<IMatcher>();
            Kernel.AddHandlerSelector(new TaskHandlerSelector(Kernel));

            RegisterSharedServices();

            RegisterExecutor();

            RegisterScheduler();
        }

        private void RegisterExecutor()
        {
            byte maxConcurrency = (byte) (MaxThreads ?? Environment.ProcessorCount);

            if (_queues.Count == 0)
            {
                var settings = new TaskQueueSettings("default", 1, 1);
                _queues.Add(new TaskQueue(settings));
            }

            Kernel.Register(Component
                .For<IExecutor>()
                .ImplementedBy<Executor>()
                .DependsOn(new { queues = _queues })
                .LifestyleSingleton(),
                
                Component
                .For<ITaskRouter>()
                .ImplementedBy<TaskRouter>()
                .DependsOn(new {matchers = _matchers})
                .LifestyleSingleton(),
                
                Component
                .For<TaskDispatcher>()
                .DependsOn(new { maxConcurrency, executionQueues = _queues.ToArray()})
                .LifestyleSingleton());
        }


        private void RegisterSharedServices()
        {
            var persistedFolder = PersistedTaskFolder ?? ManagedEsentDatabase.DefaultTaskFolder;

            Kernel.Register(
                Component
                    .For<ITaskReleaser>()
                    .ImplementedBy<WindsorReleaser>()
                    .LifestyleSingleton(),

                Component
                    .For<ITaskFactory>()
                    .ImplementedBy<WindsorTaskFactory>()
                    .LifestyleSingleton(),

                Component
                    .For<ISerializer>()
                    .ImplementedBy<JsonSerializer>()
                    .LifestyleSingleton(),

                _databaseRegistration ?? Component
                    .For<IDatabase>()
                    .ImplementedBy<ManagedEsentDatabase>()
                    .DependsOn(new { persistedFolder })
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
        /// This is useful for testing purposes.
        /// </summary>
        public DarkFlowFacility UsingInMemoryPersistence()
        {
            _databaseRegistration = Component.For<IDatabase>()
                                             .ImplementedBy<InMemoryDatabase>()
                                             .LifestyleBoundTo<IExecutor>();
            return this;
        }

        public DarkFlowFacility WithQueue(TaskQueueSettings settings)
        {
            _queues.Add(new TaskQueue(settings));
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

        public DarkFlowFacility ExecuteSynchronously()
        {
            _executorRegistration = Component
                .For<IExecutor>()
                .ImplementedBy<SynchronousExecutor>()
                .LifestyleSingleton();
            return this;
        }
    }
}