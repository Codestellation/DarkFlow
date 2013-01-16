using System;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;

namespace Codestellation.DarkFlow.CastleWindsor.Impl
{
    public class ExecutorResolver : ISubDependencyResolver
    {
        private readonly IKernel _kernel;

        public ExecutorResolver(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            _kernel = kernel;
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetItemType == typeof (IExecutor) && dependency.DependencyKey.Equals("fiber", StringComparison.InvariantCultureIgnoreCase);
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return _kernel.Resolve<IExecutor>("fiber");
        }
    }
}