using System;
using NLog;

namespace Codestellation.DarkFlow.Misc
{
    public abstract class Disposable : IDisposable
    {
        private volatile bool _disposeInProgress;
        private volatile bool _disposed;
        protected readonly Logger Logger;

        public Disposable()
        {
            Logger = LogManager.GetLogger(GetType().FullName);
        }

        public bool Disposed
        {
            get { return _disposed; }
        }

        public bool DisposeInProgress
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

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Dispose started.");
            }

            if (disposing)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Dispose managed resources started.");
                }

                DisposeManaged();

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Dispose managed resources finished.");
                }
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Dispose unmanaged resources started.");
            }

            ReleaseUnmanaged();

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Dispose unmanaged resources finished.");
            }

            _disposed = true;
        }

        protected abstract void DisposeManaged();

        protected virtual void ReleaseUnmanaged()
        {
            
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