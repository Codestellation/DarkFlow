namespace Codestellation.DarkFlow
{
    /// <summary>
    /// Executes tasks using preconfigured rules. 
    /// </summary>
    public interface IExecutor
    {
        /// <summary>
        /// Puts task to executoion pipeline. 
        /// </summary>
        /// <param name="task">Task to be executed.</param>
        void Execute(ITask task);
    }
}