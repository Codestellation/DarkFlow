using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Execution
{
    public class Executor : Disposable, IExecutor
    {
        private readonly ITaskRouter _router;
        private readonly TaskDispatcher _dispatcher;

        private readonly  Dictionary<string, ITaskQueue> _queues;

        public Executor(ITaskRouter router, TaskDispatcher dispatcher, IEnumerable<ITaskQueue> queues)
        {
            if (queues == null)
            {
                throw new ArgumentNullException("queues");
            }

            if (router == null)
            {
                throw new ArgumentNullException("router");
            }

            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            _router = router;
            _dispatcher = dispatcher;
            _queues = queues.ToDictionary(x => x.Name, x => x);
        }

        public void Execute(ITask task)
        {
            EnsureNotDisposed();

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
            //TODO: Cache this later
            var name = _router.ResolveQueueFor(task);
            var result = _queues[name];
            return result;
        }

        public void Execute(IPersistentTask task)
        {
            throw new NotImplementedException();
        }

        protected override void DisposeManaged()
        {
            _dispatcher.Dispose();
        }
    }
}