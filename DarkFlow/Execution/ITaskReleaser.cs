namespace Codestellation.DarkFlow.Execution
{
    public interface ITaskReleaser
    {
        void Release(ITask task);
    }
}