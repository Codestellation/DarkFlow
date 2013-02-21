using System;
using System.Collections;
using System.Collections.Generic;
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

        public override ITask Create(TaskData taskData)
        {
            var type = Type.GetType(taskData.TaskType);

            if (RegisteredAsSelf(type) || RegisteredAsIPersistentTask(type))
            {
                return (ITask)_kernel.Resolve(type, new Hashtable());
            }

            return base.Create(taskData);
        }

        private bool RegisteredAsIPersistentTask(Type type)
        {
            return _kernel.GetHandlers(typeof(ITask)).Any(x => x.ComponentModel.Implementation == type);
        }

        private bool RegisteredAsSelf(Type type)
        {
            var handlers = _kernel.GetHandlers(type);
            return  handlers != null && handlers.Length > 0;
        }

        public virtual TaskData GetTaskData(ITask task)
        {
            
            Type type = ProxyUtil.GetUnproxiedType(task);

            Logger.Debug("Task type is '{0}'. Real type is {1}", task.GetType(), type);

            var taskType = FormatType(type);

            PropertyValue[] properties = GetPersistedProperties(type, task);
            return new TaskData
                       {
                           TaskType = taskType,
                           Properties = properties
                       };
        }
    }
}