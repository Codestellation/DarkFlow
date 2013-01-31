using System;
using System.Linq;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    public class Executor : IExecutor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ITaskQueue[] _queues;

        public Executor(ITaskQueue[] queues)
        {
            if (queues == null)
            {
                throw new ArgumentNullException("queues");
            }
            _queues = queues;
        }

        public void Execute(ITask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }
            
            var queue = _queues.SingleOrDefault(x => x.CanEnqueue(task));

            if (queue == null)
            {
                Logger.Warn("Not found queue to dispatch task '{0}'", task);
            }
            else
            {
                queue.Enqueue(task);
            }
        }

        public void Execute(IPersistentTask task)
        {
            throw new NotImplementedException();
        }
    }
}