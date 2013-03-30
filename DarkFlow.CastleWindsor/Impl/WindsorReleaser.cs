using System;
using Castle.MicroKernel;
using Codestellation.DarkFlow.Execution;

namespace Codestellation.DarkFlow.CastleWindsor.Impl
{
    public class WindsorReleaser : DefaultReleaser
    {
        private readonly IKernel _kernel;

        public WindsorReleaser(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }

            _kernel = kernel;
        }

        public override void Release(ITask task)
        {
            if (_kernel.ReleasePolicy.HasTrack(task))
            {
                _kernel.ReleaseComponent(task);

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Task {0} was tracked. Released.", task);
                }
            }
            else
            {
                base.Release(task);
                
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Task {0} was not tracked. Disposed.", task);
                }
            }
        }
    }
}