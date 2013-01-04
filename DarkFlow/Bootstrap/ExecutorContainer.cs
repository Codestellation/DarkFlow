using System;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class ExecutorContainer : Disposable, IExecutor
    {
        private readonly IExecutor _executor;
        private readonly IDisposable[] _disposables;

        public ExecutorContainer(IExecutor executor, IDisposable[] disposables)
        {
            _executor = executor;
            _disposables = disposables;
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }

            if (disposables == null)
            {
                throw new ArgumentNullException("disposables");
            }
        }
        protected override System.Collections.Generic.IEnumerable<IDisposable> Disposables
        {
            get { return _disposables; }
        }
        public void ExecuteLater(ITask task)
        {
            _executor.ExecuteLater(task);
        }

        public void ExecuteReliably(IPersistentTask task)
        {
            _executor.ExecuteLater(task);
        }
    }
}