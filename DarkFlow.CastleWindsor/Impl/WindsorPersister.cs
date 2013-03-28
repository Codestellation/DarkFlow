using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Matchers;
using Codestellation.DarkFlow.Misc;
using NLog;
using Newtonsoft.Json;

namespace Codestellation.DarkFlow.CastleWindsor.Impl
{
    public class WindsorPersister : IPersister
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IDatabase _database;
        private readonly IMatcher _matcher;
        private readonly JsonSerializerSettings _settings;

        public WindsorPersister(IDatabase database, IKernel kernel, IMatcher matcher)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }

            if (matcher == null)
            {
                throw new ArgumentNullException("matcher");
            }

            _database = database;
            _matcher = matcher;
            var converter = new WindsorConverter(kernel);

            _settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                    Converters = new[] { converter }
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

            var matched = _matcher.TryMatch(task);

            if (matched)
            {
                var unproxiedTask = ProxyUtil.GetUnproxiedInstance(task);

                var serialized = JsonConvert.SerializeObject(unproxiedTask, _settings);

                _database.Persist(identifier, serialized);

                Logger.Debug("Serialized task:{0}{1}", Environment.NewLine, serialized);
            }
            else
            {
                Logger.Debug("{0} skipped");
            }
        }

        public virtual void Delete(Identifier identifier)
        {
            Contract.Require(identifier.IsValid, "identifier.IsValid");

            _database.Remove(identifier);
        }

        public virtual IEnumerable<KeyValuePair<Identifier, ITask>> LoadAll(Region region)
        {
            Contract.Require(region.IsValid, "region.IsValid");

            var serialized = _database.GetAll(region);

            var results = serialized.Select(Deserialize);

            return results;
        }

        private ITask Deserialize(string serialized)
        {
            var type = ExtractType(serialized);
            var result = (ITask)JsonConvert.DeserializeObject(serialized, type, _settings);
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
}