using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Codestellation.DarkFlow.Execution
{
    public class TaskDispatcher
    {
        private readonly byte _maxConcurrency;
        private readonly IExecutionQueue[] _executionQueues;
        private int _taskPending;
        private int _currentConcurrency;

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
            
            _maxConcurrency = maxConcurrency;
            _executionQueues = executionQueues.OrderByDescending(x => x.Priority).ToArray();

            foreach (var executionQueue in executionQueues)
            {
                executionQueue.TaskCountChanged += OnTaskCountChanged;
            }
        }

        private void OnTaskCountChanged(int change)
        {
            var pendingsNow = Interlocked.Add(ref _taskPending, change);

            if (pendingsNow > 0 && _currentConcurrency < _maxConcurrency)
            {
                StartNewThread();
            }
        }

        private void StartNewThread()
        {
            Interlocked.Increment(ref _currentConcurrency);
            
            //TODO Save task so that dispose can wait for it to finish.
            Task.Factory
                .StartNew(PerformTasks)
                .ContinueWith(DecrementCurrentConcurrency);
        }

        private void DecrementCurrentConcurrency(Task obj)
        {
            Interlocked.Decrement(ref _currentConcurrency);
        }

        private void PerformTasks()
        {
            while (true)
            {
                var task = TakeNextTask();
                
                if (task == null)
                {
                    break;
                }

                //TODO: try catch needed. 
                task.Execute();
            }
        }

        //note: this methods suppose that execution queues already sorted, so priority already  applied.
        private ITask TakeNextTask()
        {
            ITask result = null;
            
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
    }
}