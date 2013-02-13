namespace Codestellation.DarkFlow.Execution
{
    public interface ITaskRouter
    {
        string ResolveQueueFor(ITask task);
    }
}