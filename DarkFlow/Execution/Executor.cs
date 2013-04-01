using System;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Execution
{
    public class Executor : Disposable, IExecutor
    {
        private readonly ITaskReleaser _releaser;
        private readonly ITaskRouter _router;

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