using System;
using System.Collections.Generic;
using System.Threading;
using SimpleConfig;

namespace Codestellation.DarkFlow.Config
{
    public class DarkFlowConfiguration
    {
        private static readonly Lazy<DarkFlowConfiguration> _instance = new Lazy<DarkFlowConfiguration>(() => Configuration.Load<DarkFlowConfiguration>("darkFlow"), LazyThreadSafetyMode.ExecutionAndPublication);

        public static DarkFlowConfiguration Instance
        {
            get { return _instance.Value; }
        }

        public DispatcherSettings Dispatcher { get; set; }

        public List<QueuedExecutorSettings> Executors { get; set; }

        public List<MatcherSettings> Routes { get; set; }

        public PersistenceSettings Persistence { get; set; }
    }
}