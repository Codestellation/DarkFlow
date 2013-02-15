using System;
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
        private readonly object _latch;

        private class ExecutionInfo
        {
            public volatile Task TplTask;

            public volatile ExecutionEnvelope CurrentTask;

            public bool Free
            {
                get { return TplTask == null; }
            }

            public bool OwnedByCurrentTask
            {
                get
                {
                    var tplTask = TplTask;
                    if (tplTask == null) return false;
                    return tplTask.Id == Task.CurrentId;
                }
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

            _executionInfos =
                Enumerable.Range(0, _maxConcurrency)
                .Select(x => new ExecutionInfo())
                .ToArray();

            _latch = new object();

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
                var task = new Task(PerformTasks);
                
                //TODO Possible it better to do using Interlocked.Exchange.

                Monitor.Enter(_latch);
                var executionInfo = _executionInfos.First(x => x.Free);
                executionInfo.TplTask = task;
                Monitor.Exit(_latch);

                task.Start();
            }
            else
            {
                Interlocked.Decrement(ref _currentConcurrency);
            }
        }

        private void PerformTasks()
        {
            var executionInfo = _executionInfos.Single(x => x.OwnedByCurrentTask);

            while (true)
            {
                var envelope = TakeNextTask();
                
                executionInfo.CurrentTask = envelope;

                if (envelope == null)
                {
                    break;
                }

                envelope.ExecuteTask();

                if (DisposeInProgress || Disposed)
                {
                    break;
                }

            }

            executionInfo.CurrentTask = null;
            executionInfo.TplTask = null;

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
            foreach (var info in _executionInfos.Where(info => !info.Free))
            {
                info.TplTask.Wait();
            }
        }
    }
}