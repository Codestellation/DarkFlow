using System.Collections.Generic;
using Codestellation.DarkFlow.Execution;

namespace Codestellation.DarkFlow.Config
{
    public class DarkFlowConfiguration
    {
        public DispatcherSettings Dispatcher { get; set; }

        public List<QueuedExecutorSettings> Executors { get; set; }

        public List<MatcherSettings> Routes { get; set; }

        public PersistenceSettings Persistence { get; set; }
    }
}