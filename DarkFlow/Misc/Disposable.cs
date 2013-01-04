using System;
using System.Collections.Generic;
using System.Linq;

namespace Codestellation.DarkFlow.Misc
{
    public abstract class Disposable : IDisposable
    {
        private volatile bool _disposeInProgress;
        private volatile bool _disposed;


        public bool Disposed
        {
            get { return _disposed; }
        }

        protected bool DisposeInProgress
        {
            get { return _disposeInProgress; }
        }

        protected bool IsNotDisposed 
        {
            get { return DisposeInProgress == false && Disposed == false; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Disposable()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposeInProgress = true;

            if (disposing)
            {
                foreach (var disposable in Disposables)
                {
                    disposable.Dispose();
                }
            }

            ReleaseUnmanaged();

            _disposed = true;
        }

        protected virtual void ReleaseUnmanaged()
        {
            
        }

        protected virtual IEnumerable<IDisposable> Disposables
        {
            get { return Enumerable.Empty<IDisposable>(); }
        }

        protected void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().AssemblyQualifiedName);
            }
        }
    }
}