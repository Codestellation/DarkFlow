using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Execution
{
    public class TaskDispatcher : Disposable
    {
        private readonly byte _maxConcurrency;
        private readonly IExecutionQueue[] _executionQueues;
        private int _taskPending;
        private int _currentConcurrency;
        private readonly ExecutionInfo[] _executionInfos;

        private class ExecutionInfo
        {
            //TODO I guess more sophisticated sync could be involved.
            public volatile Task TplTask;
            public volatile ExecutionEnvelope CurrentTask;
            public volatile int OwnedCellIndex;

            public ExecutionInfo()
            {
                OwnedCellIndex = -1;
            }

            public void WaitTaskFinished()
            {
                var tplTask = TplTask;

                if (tplTask != null)
                {
                    TplTask.Wait();
                }
            }

            public void Release()
            {
                OwnedCellIndex = -1;
                CurrentTask = null;
            }
        }

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

            var queueTotalConcurrency = (byte)executionQueues.Sum(x => x.MaxConcurrency);

            _maxConcurrency = Math.Min(maxConcurrency, queueTotalConcurrency);
            _executionQueues = executionQueues.OrderByDescending(x => x.Priority).ToArray();

            _executionInfos = new ExecutionInfo[_maxConcurrency];

            foreach (var executionQueue in executionQueues)
            {
                executionQueue.TaskCountChanged += OnTaskCountChanged;
            }
        }

        private void OnTaskCountChanged(int change)
        {
            Contract.Require(change == -1 || change == 1, "change == -1 || change == 1");

            if (DisposeInProgress || Disposed)
            {
                return;
            }
            
            var pendingsNow = Interlocked.Add(ref _taskPending, change);

            if (pendingsNow <= 0 || _currentConcurrency >= _maxConcurrency) return;
            
            //Second check to ensure concurrency threshold would not be exceeded.
            var currentConcurrency = Interlocked.Increment(ref _currentConcurrency);
                
            if (currentConcurrency <= _maxConcurrency)
            {
                var executionInfo = new ExecutionInfo();
                var task = new Task(PerformTasks, executionInfo);

                for (int i = 0; i < _executionInfos.Length; i++)
                {
                    var originalValue =  Interlocked.CompareExchange(ref _executionInfos[i], executionInfo, null);
                    var cellAlreadyOwned = originalValue != null;

                    if (cellAlreadyOwned) continue;
                    
                    executionInfo.OwnedCellIndex = i;
                    executionInfo.TplTask = task;
                    
                    break;
                }
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

            while (true)
            {
                if (DisposeInProgress || Disposed)
                {
                    break;
                }
                
                var envelope = TakeNextTask();
                
                executionInfo.CurrentTask = envelope;

                if (envelope == null)
                {
                    break;
                }

                envelope.ExecuteTask();
            }

            //ExecutionInfo.Release() drops the index. So it preserved.
            var index = executionInfo.OwnedCellIndex;

            executionInfo.Release();

            _executionQueues[index] = null;
             

            Interlocked.Decrement(ref _currentConcurrency);
        }

        //note: this methods suppose that execution queues already sorted, so priority already  applied.
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