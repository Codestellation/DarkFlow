namespace Codestellation.DarkFlow.Execution
{
    /// <summary>
    /// Disposes task after execution. 
    /// </summary>
    public interface ITaskReleaser
    {
        /// <summary>
        /// Dispose task after execution. 
        /// </summary>
        /// <param name="task">Task to release.</param>
        void Release(ITask task);
    }
}