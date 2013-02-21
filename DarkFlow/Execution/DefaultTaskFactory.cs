using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Codestellation.DarkFlow.Misc;
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

        public virtual ITask Create(TaskData taskData)
        {
            Contract.Require(taskData != null, "taskData != null");

            var type = Type.GetType(taskData.TaskType);

            var constructor = GetTaskConstructor(type);

            var arguments = new List<object>();
            foreach (var parameterInfo in constructor.GetParameters())
            {
                arguments.Add(
                    taskData.Properties
                            .Single(x =>
                                    x.Name.Equals(parameterInfo.Name, StringComparison.OrdinalIgnoreCase) &&
                                    x.ValueType == parameterInfo.ParameterType)
                            .Value);

            }

            var settableProperties = GetSettableProperties(type);

            var result = (ITask) constructor.Invoke(arguments.ToArray());

            foreach (var settableProperty in settableProperties)
            {
                var data = taskData.Properties
                                   .Single(x => x.Name.Equals(settableProperty.Name, StringComparison.OrdinalIgnoreCase));

                settableProperty.SetValue(result, data.Value, null);
            }

            return result;
        }

        public virtual TaskData GetTaskData(ITask task)
        {
            var taskType = task.GetType();

            var type = FormatType(taskType);

            var properties = GetPersistedProperties(taskType, task);

            return new TaskData {TaskType = type, Properties = properties};
        }

        protected string FormatType(Type taskType)
        {
            string result;
            if (_cachedTaskTypes.TryGetValue(taskType, out result))
            {
                return result;
            }

            Monitor.Enter(_cachedTaskTypes);

            if (_cachedTaskTypes.TryGetValue(taskType, out result))
            {
                return result;
            }

            result = string.Format("{0}, {1}", taskType.FullName, taskType.Assembly.GetName().Name);
            _cachedTaskTypes[taskType] = result;

            Monitor.Exit(_cachedTaskTypes);

            return result;
        }

        protected PropertyValue[] GetPersistedProperties(Type taskType, ITask task)
        {
            var settableProperties = GetSettableProperties(taskType);
            
            //takes greedy constructor
            var constructor = GetTaskConstructor(taskType);
            
            if (constructor == null)
            {
                throw new InvalidOperationException(string.Format("Not found appropriate constructor for type {0}.", taskType));
            }

            var ctorInjectableProperties = CtorInjectableProperties(taskType, constructor);


            var persistentProperties = settableProperties.Union(ctorInjectableProperties);

            var list = new List<PropertyValue>();
            foreach (var persistentProperty in persistentProperties)
            {
                var value = persistentProperty.GetValue(task, null);
                list.Add(new PropertyValue{Name = persistentProperty.Name, Value = value, ValueType = persistentProperty.PropertyType});
            }
            return list.ToArray();
        }

        private static IEnumerable<PropertyInfo> GetSettableProperties(Type taskType)
        {
            var settableProperties = taskType
                .GetProperties()
                .Where(x => x.CanWrite);
            return settableProperties;
        }

        private static IEnumerable<PropertyInfo> CtorInjectableProperties(Type taskType, ConstructorInfo constructor)
        {
            var ctorInjectableProperties = taskType
                .GetProperties()
                .Where(
                    prop =>
                    constructor.GetParameters()
                               .Any(
                                   parm =>
                                   parm.ParameterType == prop.PropertyType &&
                                   parm.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)));
            return ctorInjectableProperties;
        }

        private static ConstructorInfo GetTaskConstructor(Type taskType)
        {
            var constructor = taskType
                .GetConstructors()
                .OrderByDescending(x => x.GetParameters().Length)
                .SingleOrDefault();
            return constructor;
        }
    }
}