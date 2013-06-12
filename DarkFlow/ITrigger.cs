using System;
using System.Collections.Generic;

namespace Codestellation.DarkFlow
{
    public abstract class Trigger
    {
        /// <summary>
        /// Identifier of the trigger. Should be unique across the scheduler. 
        /// </summary>
        public abstract string Id { get; } 

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
        /// This delegate is called when trigger fires.
        /// </summary>
        protected internal abstract void Start(Action<ITask> triggerCallback);

        protected internal abstract void Stop();
    }
}