using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Misc;
using NLog;
using Newtonsoft.Json;

namespace Codestellation.DarkFlow.Execution
{
    public class Persister : IPersister
    {
        private readonly IDatabase _database;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly JsonSerializerSettings _settings;

        public Persister(IDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            
            _database = database;

            _settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.All, 
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                };
        }

        public ITask Get(Identifier identifier)
        {
            var serialized = _database.Get(identifier);

            var result =  (ITask)JsonConvert.DeserializeObject(serialized, _settings);

            return result;
        }

        public virtual void Persist(Identifier identifier, ITask task)
        {
            Contract.Require(task != null, "task != null");
            Contract.Require(identifier.IsValid, "identifier.IsValid");

            var serialized = JsonConvert.SerializeObject(task, _settings);

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

        private KeyValuePair<Identifier, ITask> Deserialize(KeyValuePair<Identifier, string> input)
        {
            var deserialized = JsonConvert.DeserializeObject(input.Value, _settings);
            return new KeyValuePair<Identifier, ITask>(input.Key, (ITask) deserialized);
        }
    }
}