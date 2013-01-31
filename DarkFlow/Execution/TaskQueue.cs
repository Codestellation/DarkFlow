﻿using System;
using System.Collections.Concurrent;

namespace Codestellation.DarkFlow.Execution
{
    public delegate bool CanEnqueue(ITask task);
    
    public class TaskQueue : ITaskQueue, IExecutionQueue
    {
        private readonly CanEnqueue _canEnqueue;
        private readonly Action _onTaskEnqueued;
        private readonly byte _priority;
        private readonly ConcurrentQueue<ITask> _queue;

        public TaskQueue(CanEnqueue canEnqueue, Action onTaskEnqueued, byte priority)
        {
            _canEnqueue = canEnqueue;
            _onTaskEnqueued = onTaskEnqueued;
            _priority = priority;
            if (canEnqueue == null)
            {
                throw new ArgumentNullException("canEnqueue");
            }
            _queue = new ConcurrentQueue<ITask>();
        }

        public bool CanEnqueue(ITask task)
        {
            return _canEnqueue(task);
        }

        public void Enqueue(ITask task)
        {
            //NOTE: I don't check for null here intentionally to improve performance. 
            // The only place where such check should have place is IExecutor
            _queue.Enqueue(task);
            _onTaskEnqueued();
        }

        public byte Priority
        {
            get { return _priority; }
        }

        public ITask Dequeue()
        {
            ITask result = null;
            
            _queue.TryDequeue(out result);
            
            return result;
        }
    }
}