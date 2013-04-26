using System.Collections.Generic;
using Codestellation.DarkFlow.Database;

namespace Codestellation.DarkFlow.Execution
{
    public class NullPersister : IPersister
    {
        public static readonly NullPersister Instance = new NullPersister();

        private readonly KeyValuePair<Identifier, ITask>[] _empty;

        public NullPersister()
        {
            _empty = new KeyValuePair<Identifier, ITask>[0];
        }

        public ITask Get(Identifier identifier)
        {
            return null;
        }

        public bool IsPersistent(ITask task)
        {
            return false;
        }

        public void Persist(Identifier identifier, ITask task)
        {
            
        }

        public void Delete(Identifier identifier)
        {
            
        }

        public IEnumerable<KeyValuePair<Identifier, ITask>> LoadAll(Region region)
        {
            return _empty;
        }
    }
}