using System;

namespace Codestellation.DarkFlow.Execution
{
    public interface IExecutionQueue
    {
        event Action<IExecutionQueue> TaskAdded;
        
        byte Priority { get; }

        byte MaxConcurrency { get; }

        ExecutionEnvelope Dequeue();
    }
}