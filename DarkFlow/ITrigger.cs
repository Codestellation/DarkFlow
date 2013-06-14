using System;
using System.Collections.Generic;

namespace Codestellation.DarkFlow
{
    public abstract class Trigger
    {
        /// <summary>
        /// Identifier of the trigger. Should be unique across the scheduler. 
        /// </summary>
        public abstract string Name { get; } 

        /// <summary>
        /// Returns all attached tasks. 
        /// </summary>
        public abstract IEnumerable<ITask> AttachedTasks { get; }

        /// <summary>
        /// Attached task to trigger. 
        /// </summary>
        /// <param name="task">A task to be detached.</param>
        public abstract void AttachTask(ITask task);

        /// <summary>
        /// Detaches task from trigger.
        /// </summary>
        /// <param name="task"></param>
        public abstract void DetachTask(ITask task);

        /// <summary>
        /// Instructs trigger to begin scheduling tasks with supplied callback. 
        /// </summary>
        protected internal abstract void Start(Action<ITask> triggerCallback);

        /// <summary>
        /// Intructs trigger to stop scheduling tasks. 
        /// </summary>
        protected internal abstract void Stop();
    }
}