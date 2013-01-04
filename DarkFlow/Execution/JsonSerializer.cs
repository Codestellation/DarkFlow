using System;
using System.Runtime.Serialization.Formatters;
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
                    //Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto, 
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                };
        }

        public virtual string Serialize(IPersistentTask task)
        {
            var envelope = new PersistedEnvelope
                {
                    TaskType = _taskFactory.GetRealType(task),
                    State = task.PersistentState
                };

            var serialized = JsonConvert.SerializeObject(envelope, _settings);
            
            Logger.Debug("Serialized task:{0}{1}", Environment.NewLine, serialized);
            
            return serialized;
        }

        public virtual IPersistentTask Deserialize(string serializedEnvelope)
        {
            Logger.Debug("Deserializing from:{0}{1}", Environment.NewLine, serializedEnvelope);

            var envelope = JsonConvert.DeserializeObject<PersistedEnvelope>(serializedEnvelope, _settings);

            return _taskFactory.Create(envelope.TaskType, envelope.State);
        }

        protected class PersistedEnvelope
        {
            public string TaskType { get; set; }

            public object State { get; set; }
        }
    }
}