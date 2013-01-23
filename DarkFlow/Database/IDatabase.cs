using System.Collections.Generic;

namespace Codestellation.DarkFlow.Database
{
    public interface IDatabase
    {
        Identifier Persist(Region region, string serializedTask);

        string Get(Identifier id);

        void Remove(Identifier id);

        IEnumerable<KeyValuePair<Identifier, string>> GetAll();
    }
}