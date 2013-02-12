using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    public class Executor : IExecutor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly  Dictionary<string, ITaskQueue> _queues;

        public Executor(IEnumerable<ITaskQueue> queues)
        {
            if (queues == null)
            {
                throw new ArgumentNullException("queues");
            }
            _queues = queues.ToDictionary(x => x.Name, x => x);
        }

        public void Execute(ITask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }
            
            var queue = FindQueue(task);

            if (queue == null)
            {
                Logger.Warn("Not found queue to dispatch task '{0}'", task);
            }
            else
            {
                queue.Enqueue(task);
            }
        }

        private ITaskQueue FindQueue(ITask task)
        {
            throw new NotImplementedException();
        }

        public void Execute(IPersistentTask task)
        {
            throw new NotImplementedException();
        }
    }
}