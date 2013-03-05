using System;
using System.Collections.Concurrent;
using System.Threading;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Execution
{
    public class TaskQueue : ITaskQueue, IExecutionQueue
    {
        private readonly IPersister _persister;
        private readonly ConcurrentQueue<ExecutionEnvelope> _queue;

        private readonly TaskQueueSettings _settings;
        private int _currentConcurrency;

        public TaskQueue(TaskQueueSettings settings, IPersister persister)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.Validate();

            _settings = settings;
            _persister = persister;
            
            _queue = new ConcurrentQueue<ExecutionEnvelope>();
        }

        public string Name
        {
            get { return _settings.Name; }
        }

        public void Enqueue(ExecutionEnvelope envelope)
        {
            Contract.Require(envelope != null, "envelope != null");
            Contract.Require(TaskCountChanged != null, "TaskCountChanged != null");

            _persister.Persist(new Identifier(envelope.Id, _settings.Region), envelope.Task);

            //TODO Consider reusing wraps to decrease workload on GC. 
            envelope.AfterExecute += AfterExecute;
            
            _queue.Enqueue(envelope);

            TaskCountChanged(1);
        }

        private void AfterExecute(ExecutionEnvelope envelope)
        {
            Interlocked.Decrement(ref _currentConcurrency);
            _persister.Delete(new Identifier(envelope.Id, _settings.Region));
        }

        public event Action<int> TaskCountChanged;

        public byte Priority
        {
            get { return _settings.Priority; }
        }

        public byte MaxConcurrency
        {
            get { return _settings.Priority; }
        }

        public ExecutionEnvelope Dequeue()
        {
            var totalReaders = Interlocked.Increment(ref _currentConcurrency);

            if (totalReaders > _settings.MaxConcurrency)
            {
                //Concurrency level reached. Do not return envelope from queue.
                Interlocked.Decrement(ref _currentConcurrency);
                return null;
            }
            
            ExecutionEnvelope result = null;
            
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