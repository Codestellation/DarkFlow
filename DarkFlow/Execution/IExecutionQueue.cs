using System;

namespace Codestellation.DarkFlow.Execution
{
    public interface IExecutionQueue
    {
        event Action<int> TaskCountChanged;
        
        byte Priority { get; }

        byte MaxConcurrency { get; }

        ExecutionEnvelope Dequeue();
    }
}