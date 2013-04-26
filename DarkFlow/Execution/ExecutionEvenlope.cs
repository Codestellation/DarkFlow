using System;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Misc;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    public class ExecutionEnvelope
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ITaskReleaser _releaser;
        private Identifier? _id;

        internal Region Region;
        internal bool Persistent;

        public ITask Task { get; private set; }

        public Identifier Id
        {
            get
            {
                if (_id.HasValue)
                {
                    return _id.Value;
                }

                Contract.Require(Region.IsValid, "_region.IsValid");

                var id = Region.NewIdentifier();
                _id = id;
                return id;
            }
        }

        internal event Action<ExecutionEnvelope> AfterExecute;

        public ExecutionEnvelope(ITask task, ITaskReleaser releaser) 
        {
            Contract.Require(task != null, "task != null");
            Contract.Require(releaser != null, "releaser != null");

            Task = task;
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