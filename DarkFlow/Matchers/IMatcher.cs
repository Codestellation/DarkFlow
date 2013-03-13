namespace Codestellation.DarkFlow.Matchers
{
    public interface IMatcher
    {
        string QueueName { get; }

        bool Match(ITask task);
    }
}