using System;
using System.Linq;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Codestellation.DarkFlow.Execution;
using NLog;

namespace Codestellation.DarkFlow.CastleWindsor.Impl
{
    public class WindsorTaskFactory : DefaultTaskFactory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IKernel _kernel;

        public WindsorTaskFactory(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            _kernel = kernel;
        }

        public override IPersistentTask Create(string taskType, object state)
        {
            var type = Type.GetType(taskType);

            if (RegistertedAsSelf(type) || RegisteredAsIPersistentTask(type))
            {
                return (IPersistentTask)_kernel.Resolve(type, new { state });
            }

            return base.Create(taskType, state);
        }

        private bool RegisteredAsIPersistentTask(Type type)
        {
            return _kernel.GetHandlers(typeof(IPersistentTask)).Any(x => x.ComponentModel.Implementation == type);
        }

        private bool RegistertedAsSelf(Type type)
        {
            var handlers = _kernel.GetHandlers(type);
            return  handlers != null && handlers.Length > 0;
        }

        public override string GetRealType(IPersistentTask task)
        {
            Type type = ProxyUtil.GetUnproxiedType(task);
            
            Logger.Debug("Task type is '{0}'. Real type is {1}", task.GetType(), type);

            return FormatType(type);
        }
    }
}