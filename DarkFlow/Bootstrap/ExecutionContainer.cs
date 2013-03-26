using System;
using System.Collections;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class DisposableContainer : Disposable, IExecutor
    {
        private readonly IExecutor _executor;
        private readonly object[] _candidatesToDispose;

        public DisposableContainer(IExecutor executor, params object[] candidatesToDispose)
        {
            if (executor == null)
            {
                throw new ArgumentNullException("executor");
            }

            if (candidatesToDispose == null)
            {
                throw new ArgumentNullException("candidatesToDispose");
            }

            _executor = executor;
            _candidatesToDispose = candidatesToDispose;
        }

        protected override void DisposeManaged()
        {
            TryDispose(_candidatesToDispose);
        }

        private void TryDispose(object candidate)
        {
            var collection = candidate as IEnumerable;
            if (collection == null)
            {
                var disposable = candidate as IDisposable;
                if (disposable == null) return;
                disposable.Dispose();
            }
            else
            {
                foreach (var newCandidate in collection)
                {
                    TryDispose(newCandidate);
                }
            }
        }

        public void Execute(ITask task)
        {
            _executor.Execute(task);
        }
    }
}