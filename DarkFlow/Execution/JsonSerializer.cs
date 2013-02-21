using System;
using System.Runtime.Serialization.Formatters;
using Codestellation.DarkFlow.Misc;
using NLog;
using Newtonsoft.Json;

namespace Codestellation.DarkFlow.Execution
{
    public class JsonSerializer : ISerializer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ITaskFactory _taskFactory;
        private readonly JsonSerializerSettings _settings;
        

        public JsonSerializer(ITaskFactory taskFactory)
        {
            if (taskFactory == null)
            {
                throw new ArgumentNullException("taskFactory");
            }
            
            _taskFactory = taskFactory;

            _settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto, 
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                };
        }

        public virtual string Serialize(ITask task)
        {
            Contract.Require(task != null, "task != null");

            var data = _taskFactory.GetTaskData(task);

            var serialized = JsonConvert.SerializeObject(data, _settings);
            
            Logger.Debug("Serialized task:{0}{1}", Environment.NewLine, serialized);
            
            return serialized;
        }

        public virtual ITask Deserialize(string serializedData)
        {
            Logger.Debug("Deserializing from:{0}{1}", Environment.NewLine, serializedData);

            var taskData = JsonConvert.DeserializeObject<TaskData>(serializedData, _settings);

            return _taskFactory.Create(taskData);
        }
    }
}