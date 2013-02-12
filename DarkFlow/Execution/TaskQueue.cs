using System;
using System.Collections.Concurrent;
using System.Threading;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Execution
{
    public delegate bool CanEnqueue(ITask task);
    
    public class TaskQueue : ITaskQueue, IExecutionQueue
    {
        private readonly byte _priority;
        private readonly ConcurrentQueue<ITask> _queue;
        private readonly byte _maxConcurrency;
        private readonly string _name;
        private int _currentConcurrency;
        
        public TaskQueue(string name, byte priority, byte maxConcurrency)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(string.Format("Expected not null not empty, but was '{0}'", name ?? "<null>"),"name");
            }
            
            _priority = priority;
            _maxConcurrency = maxConcurrency;
            _name = name;
            _queue = new ConcurrentQueue<ITask>();
        }

        public string Name
        {
            get { return _name; }
        }

        public void Enqueue(ITask task)
        {
            Contract.Require(task != null, "task != null");
            Contract.Require(TaskCountChanged != null, "TaskCountChanged != null");
            
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