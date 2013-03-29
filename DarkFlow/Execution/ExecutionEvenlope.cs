using System;
using Codestellation.DarkFlow.Misc;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    public class ExecutionEnvelope
    {
        private readonly ITaskReleaser _releaser;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ITask Task { get; private set; }
        
        public Guid Id { get; private set; }

        internal event Action<ExecutionEnvelope> AfterExecute;

        public ExecutionEnvelope(ITask task, ITaskReleaser releaser) : this(Guid.NewGuid(), task, releaser)
        {
            
        }

        internal ExecutionEnvelope(Guid id, ITask task, ITaskReleaser releaser)
        {
            Contract.Require(task != null, "task != null");
            Contract.Require(id != Guid.Empty, "id != Guid.Empty");
            Contract.Require(releaser != null, "releaser != null");

            Task = task;
            Id = id;
            _releaser = releaser;
        }

        public void ExecuteTask()
        {
            Contract.Require(AfterExecute != null, "AfterExecute != null");
            try
            {
                Task.Execute();
                
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug(string.Format("Task {0} succeeded.", Task));
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                {
                    var message = string.Format("Task {0} failed.", Task);
                    Logger.ErrorException(message, ex);
                }
            }
            finally
            {
                _releaser.Release(Task);
                AfterExecute(this);
            }
        }
    }
}