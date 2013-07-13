using System.Collections.Generic;
using Codestellation.DarkFlow.Database;

namespace Codestellation.DarkFlow.Execution
{
    public interface IPersister
    {
        /// <summary>
        /// Loads task by identifier. 
        /// </summary>
        /// <param name="identifier">Identifier of task</param>
        /// <returns>Loaded task.</returns>
        ITask Get(Identifier identifier);
        
        /// <summary>
        /// Probe the task for peristence. 
        /// </summary>
        /// <param name="task">Task to be persisted.</param>
        /// <returns>Returns true if task is a persistent task, false otherwise.</returns>
        bool IsPersistent(ITask task);

        /// <summary>
        /// Persists tasks.
        /// </summary>
        /// <param name="identifier">Task identifier.</param>
        /// <param name="task">Task to persist.</param>
        void Persist(Identifier identifier, ITask task);

        /// <summary>
        /// Removes task from persistent storage. 
        /// </summary>
        /// <param name="identifier">Identifier of task.</param>
        void Delete(Identifier identifier);

        /// <summary>
        /// Returns all persisted tasks with identifiers.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<Identifier, ITask>> LoadAll(Region region);
    }
}