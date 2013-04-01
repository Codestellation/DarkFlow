using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Codestellation.DarkFlow.Execution
{
    public class ExplicitExecutor : IExecutor
    {
        private ConcurrentQueue<ITask> _queue;

        public ExplicitExecutor()
        {
            _queue = new ConcurrentQueue<ITask>();
        }
        
        public void Execute(ITask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }
            _queue.Enqueue(task);
        }

        public IEnumerable<ITask> EnqueuedTasks
        {
            get { return _queue; }
        }

        public IEnumerable<TTask> GetEnqueuedTasks<TTask>() where TTask : ITask
        {
            return _queue.OfType<TTask>();
        }

        public ITask FirstAdded
        {
            get
            {
                ITask result;
                _queue.TryPeek(out result);
                return result;
            }
        }

        public void ExecuteNext()
        {
            ITask task;
            if (_queue.TryDequeue(out task))
            {
                task.Execute();
            }
        }
    }
}