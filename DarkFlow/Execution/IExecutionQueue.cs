using System;

namespace Codestellation.DarkFlow.Execution
{
    public interface IExecutionQueue
    {
        byte Priority { get; }

        ITask Dequeue();
    }
}