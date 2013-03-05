using System;
using Castle.MicroKernel;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    public class PersistentTaskWithDependency : PersistentTask
    {
        private readonly IKernel _kernel;

        protected PersistentTaskWithDependency() : base(0)
        {
            
        }

        public PersistentTaskWithDependency(int count, IKernel kernel) : base(count)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            _kernel = kernel;
        }

        public IKernel Kernel
        {
            get { return _kernel; }
        }
    }
}