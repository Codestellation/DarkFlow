namespace Codestellation.DarkFlow.Execution
{
    /// <summary>
    /// Routes tasks to concreate executors using predefined rules. 
    /// </summary>
    public interface ITaskRouter
    {
        /// <summary>
        /// Routes enveloped task. 
        /// </summary>
        /// <param name="envelope">A task in envelope.</param>
        void Route(ExecutionEnvelope envelope);
    }
}