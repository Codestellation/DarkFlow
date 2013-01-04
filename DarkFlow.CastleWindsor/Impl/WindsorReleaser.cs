using System;
using Castle.MicroKernel;
using Codestellation.DarkFlow.Execution;

namespace Codestellation.DarkFlow.CastleWindsor.Impl
{
    public class WindsorReleaser : DefaultReleaser, ITaskReleaser
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
            }
            else
            {
                base.Release(task);
            }
        }
    }
}