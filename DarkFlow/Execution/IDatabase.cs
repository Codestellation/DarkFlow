using System;
using System.Collections.Generic;

namespace Codestellation.DarkFlow.Execution
{
    public interface IDatabase
    {
        Guid Persist(string serializedTask);

        string Get(Guid id);

        void Remove(Guid id);

        IEnumerable<KeyValuePair<Guid,string>> GetAll();
    }
}