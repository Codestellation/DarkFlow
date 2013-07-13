using System;

namespace Codestellation.DarkFlow.Execution
{
    /// <summary>
    /// Implements execution queue that compatible with <see cref="TaskDispatcher"/>
    /// </summary>
    public interface IExecutionQueue
    {
        /// <summary>
        /// Event raise when new task added to queue. 
        /// </summary>
        event Action<IExecutionQueue> TaskAdded;
        
        /// <summary>
        /// Priority of the queue. 
        /// </summary>
        byte Priority { get; }

        /// <summary>
        /// Max tasks concurrency for execution queue. 
        /// </summary>
        byte MaxConcurrency { get; }

        /// <summary>
        /// Extracts task from the queue. 
        /// </summary>
        /// <returns>Returns null if max concurrency level for queue reached or no tasks in queue. Execution envelope otherwise.</returns>
        ExecutionEnvelope Dequeue();
    }
}