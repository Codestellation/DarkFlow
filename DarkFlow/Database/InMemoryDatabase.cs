using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Codestellation.DarkFlow.Database
{
    public class InMemoryDatabase : IDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<Identifier, string> _tasks;

        public InMemoryDatabase()
        {
            _tasks = new ConcurrentDictionary<Identifier, string>();
        }
        public Identifier Persist(Region region, string serializedTask)
        {
            var id = region.NewIdentifier();
            _tasks[id] = serializedTask;
            
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Persisted: id='{0}'; task='{1}'", id, serializedTask);
            }

            return id;
        }

        public string Get(Identifier id)
        {
            string result;
            if(_tasks.TryGetValue(id, out result))
            {
                return result;
            }
            //TODO: Use own exception class
            throw new InvalidOperationException(string.Format("String with id='{0}' was not found. Possible concurrency issue.", id));
        }

        public void Remove(Identifier id)
        {
            string value;
            _tasks.TryRemove(id, out value);

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Removed: id='{0}'; task='{1}'", id, value);
            }
        }

        public IEnumerable<KeyValuePair<Identifier, string>> GetAll(Region region)
        {
            return _tasks.Where(x => x.Key.Region.Equals(region));
        }
    }
}