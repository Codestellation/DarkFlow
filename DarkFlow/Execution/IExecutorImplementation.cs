using Codestellation.DarkFlow.Database;

namespace Codestellation.DarkFlow.Execution
{
    /// <summary>
    /// Compatible with <see cref="TaskRouter"/> executor implementation. 
    /// </summary>
    public interface IExecutorImplementation
    {
        /// <summary>
        /// Name of executor. Used for task routing. 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Region of persisted tasks. 
        /// </summary>
        Region Region { get; }

        /// <summary>
        /// Enqueues a task in envelope to executor. 
        /// </summary>
        /// <param name="envelope">An envelope with task</param>
        void Enqueue(ExecutionEnvelope envelope);
    }
}