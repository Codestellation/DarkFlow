using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Codestellation.DarkFlow.Execution
{
    public class LimitedConcurrencyExecutor : BaseExecutor
    {
        private readonly int _maxThread;
        private readonly ConcurrentDictionary<Guid, ITask> _runningTasks;
        private int _runningThreads;
        private volatile AutoResetEvent _runningTasksFinished;

        public LimitedConcurrencyExecutor(ITaskRepository taskRepository, ITaskReleaser releaser, int maxThread) :base(taskRepository, releaser)
        {
            _maxThread = maxThread;
            _runningThreads = 0;
            _runningTasks = new ConcurrentDictionary<Guid, ITask>();
            _runningTasksFinished = new AutoResetEvent(true);
        }

        public LimitedConcurrencyExecutor(ITaskRepository taskRepository, ITaskReleaser releaser)
            : this(taskRepository, releaser, Environment.ProcessorCount)
        {
        }

        protected override void StartTask()
        {
            if(Started == false) return;

            var runningThreads = Interlocked.Increment(ref _runningThreads);
            if (runningThreads > _maxThread)
            {
                Interlocked.Decrement(ref _runningThreads);
                return;
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Running {0} of {1} threads. Starting new thread.", _runningThreads, _maxThread);
            }

            ThreadPool.QueueUserWorkItem(state => RunTasks());
        }

        protected override void PerformStart()
        {
            for (int i = 0; i < _maxThread; i++)
            {
                StartTask();
            }
        }

        protected override void PerformStop()
        {
            
        }

        private void WaitForStop()
        {
            var finished = _runningTasksFinished;

            if (finished == null) return;

            while (_runningThreads > 0)
            {
                finished.WaitOne();
            }
        }

        protected virtual void RunTasks()
        {
            try
            {
                while (IsNotDisposed && Started)
                {
                    var next = TaskRepository.TakeNext();

                    if (next == null)
                    {
                        return;
                    }
                    //needs to store somehow to ConcurrrentDictionary
                    var localId = Guid.NewGuid();

                    _runningTasksFinished.Reset();
                    _runningTasks.GetOrAdd(localId, next);

                    try
                    {
                        next.Execute();
                    }
                    finally
                    {
                        
                        ITask nomatter; // Used only to perform _runningTasks remove.
                        _runningTasks.TryRemove(localId, out nomatter);
                        Releaser.Release(next);
                    }
                }
            }
            finally
            {
                Interlocked.Decrement(ref _runningThreads);
                _runningTasksFinished.Set();
            }
        }


        protected override void ReleaseUnmanaged()
        {
            if (Disposed) return;

            WaitForStop();

            var finished = _runningTasksFinished;

            if(finished == null) return;
            
            finished.Dispose();
            _runningTasksFinished = null;
        }
    }
}