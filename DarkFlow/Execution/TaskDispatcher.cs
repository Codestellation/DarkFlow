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
        private int _taskPending;
        private int _currentConcurrency;
        private readonly ExecutionInfo[] _executionInfos;



        public TaskDispatcher(byte maxConcurrency, IExecutionQueue[] executionQueues)
        {
            if(maxConcurrency == 0)
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

            foreach (var executionQueue in executionQueues)
            {
                executionQueue.TaskCountChanged += OnTaskCountChanged;
            }

            Thread.MemoryBarrier();
        }

        private void OnTaskCountChanged(int change)
        {
            Contract.Require(change == -1 || change == 1, "change == -1 || change == 1");

            if (Disposed) return;
            
            
            var pendingsNow = Interlocked.Add(ref _taskPending, change);

            if (pendingsNow <= 0) return;
            
            TryStartNewThread();
        }

        private void TryStartNewThread()
        {
            if(Disposed) return;
            if (_currentConcurrency >= _maxConcurrency) return;

            //Second check to ensure concurrency threshold would not be exceeded.
            var currentConcurrency = Interlocked.Increment(ref _currentConcurrency);

            if (currentConcurrency <= _maxConcurrency)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Starting new execution thread.");
                }

                var executionInfo = new ExecutionInfo();
                var task = new Task(PerformTasks, executionInfo);


                executionInfo.TakeFreeCell(_executionInfos);

                
                executionInfo.TplTask = task;

                task.Start();
            }
            else
            {
                Interlocked.Decrement(ref _currentConcurrency);
            }
        }

        private void PerformTasks(object state)
        {
            var executionInfo = (ExecutionInfo) state;

            //note: thread name would be cleaned up by CLR ThreadPool when thread returns there, so it safe to name it.
            Thread.CurrentThread.Name = "DarkFlow#" + executionInfo.OwnedCellIndex;

            while (!Disposed)
            {
                var envelope = TakeNextTask();
                
                executionInfo.CurrentTask = envelope;

                if (envelope == null)
                {
                    break;
                }

                envelope.ExecuteTask();
            }

            executionInfo.Release();

            var currentConcurrency = Interlocked.Decrement(ref _currentConcurrency);

            //This prevents situation with a task hanging in a queue in very specific conditions 
            if (currentConcurrency == 0 && _taskPending > 0)
            {
                TryStartNewThread();
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