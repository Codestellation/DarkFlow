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
        private readonly ITaskReleaser _releaser;
        private readonly Dictionary<string, ITaskQueue> _queues;

        public Executor(ITaskRouter router, TaskDispatcher dispatcher, ITaskReleaser releaser, IEnumerable<ITaskQueue> queues)
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

            if (releaser == null)
            {
                throw new ArgumentNullException("releaser");
            }

            _router = router;
            _dispatcher = dispatcher;
            _releaser = releaser;
            _queues = queues.ToDictionary(x => x.Name, x => x);
        }

        public void Execute(ITask task)
        {
            EnsureNotDisposed();

            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            var envelope = new ExecutionEnvelope(task, _releaser);

            var queue = FindQueue(envelope);

            if (queue == null)
            {
                Logger.Warn("Not found queue to dispatch task '{0}'", task);
            }
            else
            {
                queue.Enqueue(envelope);
            }
        }

        private ITaskQueue FindQueue(ExecutionEnvelope envelope)
        {
            //TODO: Cache this later
            var name = _router.ResolveQueueFor(envelope.Task);
            var result = _queues[name];
            return result;
        }

        protected override void DisposeManaged()
        {
            _dispatcher.Dispose();
        }
    }
}