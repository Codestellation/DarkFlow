using System;
using System.Collections.Generic;
using Codestellation.DarkFlow.Misc;
using Microsoft.Isam.Esent.Collections.Generic;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    public class ManagedEsentDatabase : Disposable, IDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _persistFolder;
        private readonly PersistentDictionary<Guid, string> _database;
        
        public const string DefaultTaskFolder = "PersistedTasks";

        public ManagedEsentDatabase() :this(DefaultTaskFolder)
        {
            
        }

        public ManagedEsentDatabase(string persistFolder)
        {
            _persistFolder = persistFolder;
            _database = new PersistentDictionary<Guid, string>(persistFolder);
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Created managed esent task database at folder '{0}'", persistFolder);
            }
        }

        public string PersistFolder
        {
            get { return _persistFolder; }
        }

        public Guid Persist(string serializedTask)
        {
            var id = Guid.NewGuid();
            _database[id] = serializedTask;

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Persisted: id='{0}'; task='{1}'", id, serializedTask);
            }

            return id;
        }

        public string Get(Guid id)
        {
            string result;
            if (_database.TryGetValue(id, out result))
            {
                return result;
            }
            //TODO: Use own exception class
            throw new InvalidOperationException(string.Format("String with id='{0}' was not found. Possible concurrency issue.", id));
        }

        public void Remove(Guid id)
        {
            _database.Remove(id);
            
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Removed: id='{0}';", id);
            }
        }

        public IEnumerable<KeyValuePair<Guid, string>> GetAll()
        {
            return _database;
        }

        protected override IEnumerable<IDisposable> Disposables
        {
            get { return new[] {_database}; }
        }
    }
}