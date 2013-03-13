using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Misc;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Codestellation.DarkFlow.CastleWindsor.Impl
{
    public class WindsorPersister : IPersister
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDatabase _database;
        private readonly JsonSerializerSettings _settings;

        public WindsorPersister(IDatabase database, IKernel kernel)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }

            _database = database;
            var converter = new Converter(kernel);

            _settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.All, 
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                    Converters = new[] {converter}
                };
        }

        public ITask Get(Identifier identifier)
        {
            var serialized = _database.Get(identifier);
            var result = Deserialize(serialized);
            return result;
        }

        public virtual void Persist(Identifier identifier, ITask task)
        {
            Contract.Require(task != null, "task != null");
            Contract.Require(identifier.IsValid, "identifier.IsValid");

            var unproxiedTask = ProxyUtil.GetUnproxiedInstance(task);

            var serialized = JsonConvert.SerializeObject(unproxiedTask, _settings);

            _database.Persist(identifier, serialized);
            
            Logger.Debug("Serialized task:{0}{1}", Environment.NewLine, serialized);
        }

        public virtual void Delete(Identifier identifier)
        {
            Contract.Require(identifier.IsValid, "identifier.IsValid");

            _database.Remove(identifier);
        }

        public virtual IEnumerable<KeyValuePair<Identifier,ITask>> LoadAll(Region region)
        {
            Contract.Require(region.IsValid, "region.IsValid");

            var serialized = _database.GetAll(region);

            var results = serialized.Select(Deserialize);

            return results;
        }

        private ITask Deserialize(string serialized)
        {
            var type = ExtractType(serialized);
            var result = (ITask) JsonConvert.DeserializeObject(serialized, type, _settings);
            return result;
        }

        private static Type ExtractType(string serialized)
        {
            const string typebegin = "\"$type\": \"";
            var startIndex = serialized.IndexOf(typebegin, StringComparison.OrdinalIgnoreCase) + typebegin.Length;
            var endIndex = serialized.IndexOf("\",", startIndex, StringComparison.OrdinalIgnoreCase);

            var typename = serialized.Substring(startIndex, endIndex - startIndex);
            var type = Type.GetType(typename);
            return type;
        }

        private KeyValuePair<Identifier, ITask> Deserialize(KeyValuePair<Identifier, string> input)
        {
            var results = new KeyValuePair<Identifier, ITask>(input.Key, Deserialize(input.Value));
            return results;
        }
    }


    class Converter : JsonConverter
    {
        private readonly IKernel _kernel;

        public Converter(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            _kernel = kernel;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("CustomCreationConverter should only be used while deserializing.");
        }

        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var value = _kernel.Resolve(objectType);

            serializer.Populate(reader, value);
            return value;
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