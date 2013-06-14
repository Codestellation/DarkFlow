using System;
using System.Collections.Generic;

namespace Codestellation.DarkFlow
{
    public interface IScheduler
    {
        /// <summary>
        /// Returns all triggers that scheduler holds.
        /// </summary>
        IEnumerable<Trigger> Triggers { get; }
        
        /// <summary>
        /// Adds new trigger to scheduler.
        /// </summary>
        /// <param name="trigger"></param>
        /// <exception cref="InvalidOperationException">When trigger with same Name already added to scheduler.</exception>
        void AttachTrigger(Trigger trigger);

        /// <summary>
        /// Removes specified trigger. 
        /// </summary>
        /// <param name="trigger">Trigger to remove</param>
        /// <exception cref="InvalidOperationException">When trigger with same Name not found in scheduler.</exception>
        void DetachTrigger(Trigger trigger);
    }
}