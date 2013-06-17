using System;
using Codestellation.DarkFlow.Misc;
using Codestellation.DarkFlow.Stat;

namespace Codestellation.DarkFlow.Execution
{
    public class Executor : Disposable, IExecutor
    {
        private readonly ITaskReleaser _releaser;
        private readonly ITaskRouter _router;
        private IMonitor _monitor;

        public IMonitor Monitor
        {
            get { return _monitor ?? (_monitor = NullMonitor.Instance); }
            set { _monitor = value; }
        }

        public Executor(ITaskRouter router, TaskDispatcher dispatcher, ITaskReleaser releaser)
        {
            if (router == null)
            {
                throw new ArgumentNullException("router");
            }

            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            if (releaser == null)
            {
                throw new ArgumentNullException("releaser");
            }

            _router = router;
            _releaser = releaser;
        }

        public void Execute(ITask task)
        {
            EnsureNotDisposed();

            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            var envelope = new ExecutionEnvelope(task, _releaser);
            if (Monitor.Enabled)
            {
                envelope.Monitor = Monitor;
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Added '{0}'.", task);
            }

            _router.Route(envelope);
        }

        protected override void DisposeManaged()
        {

        }
    }
}