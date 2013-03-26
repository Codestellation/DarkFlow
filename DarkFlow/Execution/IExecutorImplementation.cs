namespace Codestellation.DarkFlow.Execution
{
    public interface IExecutorImplementation
    {
        string Name { get; }

        void Enqueue(ExecutionEnvelope envelope);
    }
}