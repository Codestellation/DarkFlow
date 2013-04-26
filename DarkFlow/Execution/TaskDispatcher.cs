using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Execution
{
    public partial class TaskDispatcher : Disposable
    {
        private readonly byte _maxConcurrency;
        private readonly IExecutionQueue[] _executionQueues;
        private int _currentConcurrency;
        private readonly ExecutionInfo[] _executionInfos;
        private string[] _threadNames;

        public TaskDispatcher(byte maxConcurrency, IExecutionQueue[] executionQueues)
        {
            if (maxConcurrency == 0)
            {
                throw new ArgumentOutOfRangeException("maxConcurrency", "MaxConcurrency should be greater than zero.");
            }

            if (executionQueues == null)
            {
                throw new ArgumentNullException("executionQueues");
            }

            if (executionQueues.Length == 0)
            {
                throw new ArgumentOutOfRangeException("executionQueues", "List of execution queues cannot be empty.");
            }

            if (executionQueues.Any(x => x == null))
            {
                throw new ArgumentOutOfRangeException("executionQueues", "Execution queues cannot contain null values.");
            }

            var queueTotalConcurrency = (byte)executionQueues.Sum(x => x.MaxConcurrency);

            _maxConcurrency = Math.Min(maxConcurrency, queueTotalConcurrency);
            _executionQueues = executionQueues.OrderByDescending(x => x.Priority).ToArray();

            _executionInfos = new ExecutionInfo[_maxConcurrency];
            _threadNames = Enumerable.Range(0, _maxConcurrency).Select(x => "DarkFlow#" + x).ToArray();

            foreach (var executionQueue in executionQueues)
            {
                executionQueue.TaskAdded += OnTaskAdded;
            }

            Thread.MemoryBarrier();
        }

        private void OnTaskAdded(IExecutionQueue executor)
        {
            Contract.Require(executor != null, "executor != null");

            TryStartNewThread(executor);
        }

        private void TryStartNewThread(IExecutionQueue executor)
        {
         BEGIN:
            if (_disposed) return;

            //Second check to ensure concurrency threshold would not be exceeded.
            var currentConcurrency = Interlocked.Increment(ref _currentConcurrency);

            if (currentConcurrency > _maxConcurrency)
            {
                currentConcurrency = Interlocked.Decrement(ref _currentConcurrency);

                if (currentConcurrency == 0)
                {
                    goto BEGIN;
                }

                return;
            }
            ExecutionEnvelope envelope;
            if (executor == null)
            {
                envelope = TakeNextTask();
            }
            else
            {
                envelope = executor.Dequeue();
            }
            
            
            if (envelope == null)
            {
                Interlocked.Decrement(ref _currentConcurrency);
                return;
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Starting new execution thread.");
            }

            var executionInfo = new ExecutionInfo { CurrentTask = envelope };

            var tplTask = new Task(PerformTasks, executionInfo);

            executionInfo.TakeFreeCell(_executionInfos, _threadNames);

            executionInfo.TplTask = tplTask;

            tplTask.Start();
        }

        private void PerformTasks(object state)
        {
            var executionInfo = (ExecutionInfo)state;

            //note: thread name would be cleaned up by CLR ThreadPool when thread returns there, so it safe to name it.
            Thread.CurrentThread.Name = executionInfo.ThreadName; 

            while (!_disposed)
            {
                var envelope = executionInfo.CurrentTask;

                Contract.Require(envelope != null, "envelope != null");

                if (envelope == null)
                {
                    break;
                }
                executionInfo.CurrentTask = envelope;
                envelope.ExecuteTask();

                executionInfo.CurrentTask = TakeNextTask();
            }

            executionInfo.Release();

            var currentConcurrency = Interlocked.Decrement(ref _currentConcurrency);

            //This prevents situation with a task hanging in a queue in very specific conditions 
            if (currentConcurrency == 0)
            {
                TryStartNewThread(null);
            }
        }

        //note: this methods suppose that execution queues already sorted, so priority already  applied.
        //TODO: Implement round robin to prevent starvation for queues with the same priority.
        private ExecutionEnvelope TakeNextTask()
        {
            ExecutionEnvelope result = null;

            for (int i = 0; i < _executionQueues.Length; i++)
            {
                result = _executionQueues[i].Dequeue();
                if (result != null)
                {
                    break;
                }
            }
            return result;
        }
        protected override void DisposeManaged()
        {
            for (int i = 0; i < _executionInfos.Length; i++)
            {
                var executionInfo = _executionInfos[i];

                Thread.MemoryBarrier();

                if (executionInfo != null)
                {
                    executionInfo.WaitTaskFinished();
                }
            }
        }
    }
}