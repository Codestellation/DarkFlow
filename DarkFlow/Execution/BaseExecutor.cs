using System;
using System.Threading;
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

            _taskRepository = taskRepository;
            _releaser = releaser;
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

            StartTask();
        }

        protected abstract void StartTask();

        public void Start()
        {
            var started = Interlocked.Increment(ref _started);

            if (started == 1)
            {
                PerfomStart();
            }
            else
            {
                Interlocked.Decrement(ref _started);
            }

        }

        protected abstract void PerfomStart();

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}