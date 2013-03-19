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
        private readonly IMatcher _matcher;
        private readonly Dictionary<string, ITaskQueue> _taskQueues;

        public TaskRouter(IMatcher matcher, IEnumerable<ITaskQueue> taskQueues)
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
            _taskQueues = taskQueues.ToDictionary(x => x.Name, x => x);
        }

        public void Route(ExecutionEnvelope task)
        {
            Contract.Require(task != null, "task != null");

            var result = _matcher.TryMatch(task.Task);

            if (result)
            {
                var queue = _taskQueues[result.Value];
                
                queue.Enqueue(task);
            }
            else
            {
                throw new InvalidOperationException(string.Format("Task {0} does not match any queue", task));
            }
        }
    }
}