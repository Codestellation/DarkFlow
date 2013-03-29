using System;
using Castle.MicroKernel;
using NLog;
using Newtonsoft.Json;

namespace Codestellation.DarkFlow.CastleWindsor.Impl
{
    internal class WindsorConverter : JsonConverter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IKernel _kernel;

        public WindsorConverter(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            _kernel = kernel;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("WindsorConverter should only be used while deserializing.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var task = _kernel.Resolve(objectType);

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Populating task {0}", task);
            }

            serializer.Populate(reader, task);
            return task;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterface(typeof(ITask).Name) != null && _kernel.HasComponent(objectType);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}