using System;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Codestellation.DarkFlow.CastleWindsor.Impl;
using Codestellation.DarkFlow.Execution;
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

        public string PersistedTaskFolder { get; set; }

        public bool TestMode { get; set; }

        public int? MaxThreads { get; set; }

        protected override void Init()
        {
            int maxThread = MaxThreads ?? Environment.ProcessorCount;
            var persistedTaskFolder = PersistedTaskFolder ?? ManagedEsentDatabase.DefaultTaskFolder;
            Kernel.AddHandlerSelector(new TaskHandlerSelector(Kernel));
            
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

                _databaseRegistration ?? 
                Component
                    .For<IDatabase>()
                    .ImplementedBy<ManagedEsentDatabase>()
                    .DependsOn(new{persistedTaskFolder})
                    .LifestyleSingleton(),

                Component
                    .For<ITaskRepository>()
                    .ImplementedBy<TaskRepository>()
                    .LifestyleSingleton(),

                _executorRegistration ?? 
                Component
                    .For<IExecutor>()
                    .ImplementedBy<LimitedConcurrencyExecutor>()
                    .DependsOn(new {maxThread})
                    .LifestyleSingleton(),

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