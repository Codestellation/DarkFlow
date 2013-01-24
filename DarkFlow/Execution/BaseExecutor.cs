using System;
using System.Threading;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Misc;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    public abstract class BaseExecutor : Disposable, IExecutor, ISupportStart
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskReleaser _releaser;
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private int _started;
        private readonly Region _region;

        public BaseExecutor(ITaskRepository taskRepository, ITaskReleaser releaser)
        {
            if (taskRepository == null)
            {
                throw new ArgumentNullException("taskRepository");
            }

            if (releaser == null)
            {
                throw new ArgumentNullException("releaser");
            }
            _region = new Region(GetType().Name);
            _taskRepository = taskRepository;
            _releaser = releaser;

            _taskRepository.SetRegion(_region);
        }

        public ITaskRepository TaskRepository
        {
            get { return _taskRepository; }
        }

        public ITaskReleaser Releaser
        {
            get { return _releaser; }
        }

        public bool Started
        {
            get { return _started == 1; }
        }

        public virtual void Execute(ITask task)
        {
            if (ReferenceEquals(task, null))
            {
                throw new ArgumentNullException("task");
            }

            EnsureNotDisposed();

            TaskRepository.Add(task);

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Enqueued {0}.", task);
            }

            StartTask();
        }

        public virtual void Execute(IPersistentTask task)
        {
            if (ReferenceEquals(task, null))
            {
                throw new ArgumentNullException("task");
            }

            EnsureNotDisposed();

            TaskRepository.Add(task);

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Enqueued Persistent {0}.", task);
            }

            if (!Started && Logger.IsWarnEnabled)
            {
                Logger.Warn("Enqueued task, but executor is not started.");
            }

            StartTask();
        }

        protected abstract void StartTask();

        public void Start()
        {
            EnsureNotDisposed();
           
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Executor is starting.");
            }

            var started = Interlocked.Increment(ref _started);

            if (started == 1)
            {
                PerformStart();
            }
            else
            {
                Interlocked.Decrement(ref _started);
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Executor has started.");
            }
        }

        protected abstract void PerformStart();

        public void Stop()
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Executor is stopping.");
            }

            var started = Interlocked.Decrement(ref _started);

            if (started == 0)
            {
                PerformStop();
            }
            else
            {
                Interlocked.Increment(ref _started);
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Executor has stopped.");
            }
        }

        protected abstract void PerformStop();
    }
}