using System;

namespace Codestellation.DarkFlow.Tests
{
    public class DisposableTask : ITask, IDisposable
    {
        private bool Throw { get; set; }

        public void Execute()
        {
            
        }

        public void Dispose()
        {
            if (Throw)
            {
                throw new InvalidOperationException();
            }
            Disposed = true;
        }

        public bool Disposed { get; private set; }

        public static ITask ThrowOnDispose()
        {
            return new DisposableTask {Throw = true};
        }

        public static DisposableTask DisposeNormally()
        {
            return new DisposableTask { Throw = false };
        }
    }
}