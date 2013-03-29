using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.DarkFlow.Matchers;
using Codestellation.DarkFlow.Misc;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    //TODO This class should take care of pushing tasks to queues, or die otherwise.
    public class TaskRouter : ITaskRouter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IMatcher _matcher;
        private readonly Dictionary<string, IExecutorImplementation> _executors;

        public TaskRouter(IMatcher matcher, IEnumerable<IExecutorImplementation> taskQueues)
        {
            if (matcher == null)
            {
                throw new ArgumentNullException("matcher");
            }

            if (taskQueues == null)
            {
                throw new ArgumentNullException("taskQueues");
            }

            _matcher = matcher;
            _executors = taskQueues.ToDictionary(x => x.Name, x => x);
        }

        public void Route(ExecutionEnvelope task)
        {
            Contract.Require(task != null, "task != null");

            var result = _matcher.TryMatch(task.Task);


            if (!result)
            {
                throw new InvalidOperationException(string.Format("Task {0} does not match any executor", task));
            }

            IExecutorImplementation executor = null;

            var executorName = result.Value;

            if (_executors.TryGetValue(executorName, out executor))
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Task {0} routed to {1}", task, executor.Name);
                }

                executor.Enqueue(task);
            }
            else
            {
                var message = string.Format("Not found executor '{0}' for task {1}", executorName, task);
                throw new InvalidOperationException(message);
            }
        }
    }
}