namespace Codestellation.DarkFlow.Execution
{
    public interface IMatcher
    {
        string QueueName { get; }

        bool Match(ITask task);
    }
}