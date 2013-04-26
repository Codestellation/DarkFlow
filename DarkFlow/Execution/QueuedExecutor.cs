using System;
using System.Collections.Concurrent;
using System.Threading;
using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Execution
{
    public class QueuedExecutor : IExecutorImplementation, IExecutionQueue
    {
        private readonly IPersister _persister;
        private readonly ConcurrentQueue<ExecutionEnvelope> _queue;
        private readonly QueuedExecutorSettings _settings;
        private int _currentConcurrency;
        private readonly Region _region;

        public QueuedExecutor(QueuedExecutorSettings settings, IPersister persister)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.Validate();

            _settings = settings;
            _persister = persister;
            _region = new Region(_settings.Name);
            _queue = new ConcurrentQueue<ExecutionEnvelope>();
        }

        public string Name
        {
            get { return _settings.Name; }
        }

        public Region Region
        {
            get { return _region; }
        }

        public void Enqueue(ExecutionEnvelope envelope)
        {
            Contract.Require(envelope != null, "envelope != null");
            Contract.Require(TaskAdded != null, "TaskAdded != null");
            if (_persister.IsPersistent(envelope.Task))
            {
                envelope.Persistent = true;
                _persister.Persist(envelope.Id, envelope.Task);
            }

            //TODO Consider reusing wraps to decrease workload on GC. 
            envelope.AfterExecute += AfterExecute;
            
            _queue.Enqueue(envelope);
            if(_currentConcurrency > _settings.MaxConcurrency) return;
            TaskAdded(this);
        }

        private void AfterExecute(ExecutionEnvelope envelope)
        {
            Interlocked.Decrement(ref _currentConcurrency);

            if (envelope.Persistent)
            {
                _persister.Delete(envelope.Id);
            }
            
        }

        public event Action<IExecutionQueue> TaskAdded;

        public byte Priority
        {
            get { return _settings.Priority; }
        }

        public byte MaxConcurrency
        {
            get { return _settings.Priority; }
        }

        public int Count
        {
            get { return _queue.Count; }
        }

        public ExecutionEnvelope Dequeue()
        {
            BEGIN:
            var totalReaders = Interlocked.Increment(ref _currentConcurrency);

            if (totalReaders > _settings.MaxConcurrency)
            {
                //Concurrency level reached. Do not return envelope from queue.
                totalReaders = Interlocked.Decrement(ref _currentConcurrency);
                //It's possible, that previous line, and line 100 will execute close to simultaneous, and the task in queue will not be return.
                //This check prevents hanging in such situation. 
                if (totalReaders == 0)
                {
                    goto BEGIN;
                }
                return null;
            }
            
            ExecutionEnvelope result = null;
            
            _queue.TryDequeue(out result);
            
            if (result == null)
            {
                Interlocked.Decrement(ref _currentConcurrency);
            }
            
            return result;
        }
    }
}