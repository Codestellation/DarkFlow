namespace Codestellation.DarkFlow.Execution
{
    public interface ITaskRouter
    {
        void Route(ExecutionEnvelope envelope);
    }
}