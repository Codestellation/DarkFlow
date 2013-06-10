using System.Collections.Generic;

namespace Codestellation.DarkFlow
{
    public interface ITrigger
    {
        /// <summary>
        /// Identifier of the trigger. Should be unique across the scheduler. 
        /// </summary>
        string Id { get; } 

        /// <summary>
        /// Returns all attached tasks. 
        /// </summary>
        IEnumerable<ITask> AttachedTasks { get; }

        /// <summary>
        /// Attached task to trigger. 
        /// </summary>
        /// <param name="task">A task to be detached.</param>
        void AttachTask(ITask task);

        /// <summary>
        /// Detaches task from trigger.
        /// </summary>
        /// <param name="task"></param>
        void DetachTask(ITask task);
    }
}