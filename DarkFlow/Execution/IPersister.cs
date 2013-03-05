﻿using System.Collections.Generic;
using Codestellation.DarkFlow.Database;

namespace Codestellation.DarkFlow.Execution
{
    public interface IPersister
    {
        void Persist(Identifier identifier, ITask task);

        void Delete(Identifier identifier);

        IEnumerable<KeyValuePair<Identifier, ITask>> LoadAll(Region region);
    }
}