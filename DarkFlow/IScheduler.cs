using System;
using System.Collections.Generic;
using Codestellation.DarkFlow.Schedules;

namespace Codestellation.DarkFlow
{
    public interface IScheduler
    {
        /// <summary>
        /// Returns all triggers that scheduler holds.
        /// </summary>
        IEnumerable<ITrigger> Triggers { get; }
        
        /// <summary>
        /// Adds new trigger to scheduler.
        /// </summary>
        /// <param name="trigger"></param>
        void AddTrigger(ITrigger trigger);

        /// <summary>
        /// Removes specified trigger. 
        /// </summary>
        /// <param name="trigger">Trigger to remove</param>
        void RemoveTrigger(ITrigger trigger);

        /// <summary>
        /// Removes trigger with specified identifier. 
        /// </summary>
        /// <param name="triggerId">Id of trigger that should be removed.</param>
        void RemoveTrigger(string triggerId);
        
        [Obsolete("Look forward for the new architecture of trigger based scheduler.", false)]
        void Schedule(ITask task, Schedule schedule);
    }
}