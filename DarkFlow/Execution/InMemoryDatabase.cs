using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    public class InMemoryDatabase : IDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<Guid, string> _tasks;

        public InMemoryDatabase()
        {
            _tasks = new ConcurrentDictionary<Guid, string>();
        }
        public Guid Persist(string serializedTask)
        {
            var id = Guid.NewGuid();
            _tasks[id] = serializedTask;
            
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Persisted: id='{0}'; task='{1}'", id, serializedTask);
            }

            return id;
        }

        public string Get(Guid id)
        {
            string result;
            if(_tasks.TryGetValue(id, out result))
            {
                return result;
            }
            //TODO: Use own exception class
            throw new InvalidOperationException(string.Format("String with id='{0}' was not found. Possible concurrency issue.", id));
        }

        public void Remove(Guid id)
        {
            string value;
            _tasks.TryRemove(id, out value);

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Removed: id='{0}'; task='{1}'", id, value);
            }
        }

        public IEnumerable<KeyValuePair<Guid, string>> GetAll()
        {
            return _tasks;
        }
    }
}