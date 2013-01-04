using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    public class DefaultTaskFactory : ITaskFactory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Type, string> _cachedTaskTypes;

        public DefaultTaskFactory()
        {
            _cachedTaskTypes = new Dictionary<Type, string>();
        }

        public virtual IPersistentTask Create(string taskType, object state)
        {
            
            if (taskType == null)
            {
                throw new ArgumentNullException("taskType");
            }

            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            var type = Type.GetType(taskType);
            
            var stateType = state.GetType();


            IPersistentTask result = CreateFromParametrizedConstructor(type, state, stateType);
            if (result != null)
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Task of type {0} created using parameterized constructor.");
                }
                return result;
            }

            var defaultConstructor = type.GetConstructor(new Type[0]);

            if (defaultConstructor == null)
            {
                var message = string.Format("In order to use DefaultTaskFactory task of type '{0}' should have constructor with the only argument of type '{1}' or default public constructor with public settably property of type '{2}'.",
                                            taskType, state.GetType(), state.GetType());
                throw new InvalidOperationException(message);
            }

            result = (IPersistentTask) defaultConstructor.Invoke(new object[0]);

            var stateProperty = type.GetProperties().Where(pi => pi.PropertyType == state.GetType()).FirstOrDefault(pi => pi.CanWrite);

            if (stateProperty == null)
            {
                return result;
            }

            stateProperty.SetValue(result, state, null);
            return result;
        }

        public virtual string GetRealType(IPersistentTask task)
        {
            var taskType = task.GetType();

            return FormatType(taskType);
        }

        protected virtual string FormatType(Type taskType)
        {
            string result;
            if (_cachedTaskTypes.TryGetValue(taskType, out result))
            {
                return result;
            }

            lock (_cachedTaskTypes)
            {
                if (_cachedTaskTypes.TryGetValue(taskType, out result))
                {
                    return result;
                }

                result = string.Format("{0}, {1}", taskType.FullName, taskType.Assembly.GetName().Name);
                _cachedTaskTypes[taskType] = result;
                return result;
            }
        }

        private static IPersistentTask CreateFromParametrizedConstructor(Type taskType, object state, Type stateType)
        {
            var constructor = taskType.GetConstructor(new[] {stateType});

            if (constructor != null)
            {
                return (IPersistentTask) constructor.Invoke(new[] {state});
            }
            return null;
        }
    }
}