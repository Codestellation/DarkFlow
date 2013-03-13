using System;
using Castle.MicroKernel;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    public class PersistentTaskWithDependency : ITask
    {
        private readonly IKernel _kernel;

        public PersistentTaskWithDependency(IKernel kernel) 
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            _kernel = kernel;
        }

        public int Count { get; set; }
        
        public void Execute()
        {
            
        }
    }
}