using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Codestellation.DarkFlow.Execution
{
    public delegate bool CanEnqueue(ITask task);
    
    public class TaskQueue : ITaskQueue, IExecutionQueue
    {
        private readonly CanEnqueue _canEnqueue;
        private readonly byte _priority;
        private readonly ConcurrentQueue<ITask> _queue;
        private readonly byte _maxConcurrency;
        private int _currentConcurrency;

        public TaskQueue(CanEnqueue canEnqueue, byte priority, byte maxConcurrency)
        {
            if (canEnqueue == null)
            {
                throw new ArgumentNullException("canEnqueue");
            }

            _canEnqueue = canEnqueue;
            _priority = priority;
            _maxConcurrency = maxConcurrency;
            _queue = new ConcurrentQueue<ITask>();
            TaskCountChanged = delegate { };
        }

        public bool CanEnqueue(ITask task)
        {
            return _canEnqueue(task);
        }

        public void Enqueue(ITask task)
        {
            //NOTE: I don't check for null here intentionally to improve performance. 
            // The only place where such check should have place is IExecutor
            //TODO Consider reusing wraps to decrease workload on GC. 
            var wrap = new TaskExecutionWrap(task, () => Interlocked.Decrement(ref _currentConcurrency));
            _queue.Enqueue(wrap);
            TaskCountChanged(1);
        }

        public event Action<int> TaskCountChanged;

        public byte Priority
        {
            get { return _priority; }
        }

        public byte MaxConcurrency
        {
            get { return _maxConcurrency; }
        }

        public ITask Dequeue()
        {
            var totalReaders = Interlocked.Increment(ref _currentConcurrency);

            if (totalReaders > _maxConcurrency)
            {
                //Concurrency level reached. Do not return task from queue.
                Interlocked.Decrement(ref _currentConcurrency);
                return null;
            }
            
            ITask result = null;
            
            _queue.TryDequeue(out result);
            
            if (result != null)
            {
                TaskCountChanged(-1);
            }
            else
            {
                Interlocked.Decrement(ref _currentConcurrency);
            }
            
            return result;
        }
    }
}