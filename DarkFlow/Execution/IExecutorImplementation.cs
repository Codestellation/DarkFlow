using Codestellation.DarkFlow.Database;

namespace Codestellation.DarkFlow.Execution
{
    public interface IExecutorImplementation
    {
        string Name { get; }

        Region Region { get; }

        void Enqueue(ExecutionEnvelope envelope);
    }
}