using System;
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

        public const string OrderedExecutorName = "ordered";
        public const string LimitedConcurrencyExecutorName = "limited";

        public string PersistedTaskFolder { get; set; }

        public int? MaxThreads { get; set; }

        protected override void Init()
        {
            Kernel.AddHandlerSelector(new TaskHandlerSelector(Kernel));
            Kernel.Resolver.AddSubResolver(new ExecutorResolver(Kernel));

            RegisterSharedServices();

            RegisterLimitedExecutor();

            RegisterQueuedExecutor();

            RegisterScheduler();
        }

        private void RegisterLimitedExecutor()
        {
            int maxThread = MaxThreads ?? Environment.ProcessorCount;

            Kernel.Register(
                _executorRegistration ??
                Component
                    .For<IExecutor>()
                    .ImplementedBy<LimitedConcurrencyExecutor>()
                    .Named(LimitedConcurrencyExecutorName)
                    .StartUsingMethod(c => ((ISupportStart)c).Start)
                    .StopUsingMethod(c => ((ISupportStart)c).Stop)
                    .DependsOn(new {maxThread})
                    .LifestyleSingleton());
        }

        private void RegisterQueuedExecutor()
        {
            Kernel.Register(
                Component
                    .For<IExecutor>()
                    .ImplementedBy<OrderedExecutor>()
                    .Named(OrderedExecutorName)
                    .StartUsingMethod(c => ((ISupportStart)c).Start)
                    .StopUsingMethod(c => ((ISupportStart)c).Stop)
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
                    .LifestyleSingleton(),

                Component
                    .For<ITaskRepository>()
                    .ImplementedBy<TaskRepository>()
                    .LifestyleBoundTo<IExecutor>()
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