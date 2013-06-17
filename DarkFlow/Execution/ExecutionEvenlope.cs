using System;
using System.Diagnostics;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Misc;
using Codestellation.DarkFlow.Stat;
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
        private IMonitor _monitor;

        public ITask Task { get; private set; }

        public IMonitor Monitor
        {
            get { return _monitor ?? (_monitor = NullMonitor.Instance); }
            set { _monitor = value; }
        }

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
            Stopwatch stopwatch = null;
            try
            {
                if (Monitor.Enabled)
                {
                    stopwatch = new Stopwatch();
                    stopwatch.Start();
                }
                    Task.Execute();
                
                if (stopwatch != null)
                {
                    stopwatch.Stop();
                    Monitor.Increment(Task.GetType().FullName, stopwatch.Elapsed);
                }
                

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug(string.Format("Task {0} succeeded.", Task));
                }
            }
            catch (Exception ex)
            {
                if (stopwatch != null)
                {
                    stopwatch.Stop();
                    Monitor.Increment(Task.GetType().FullName, stopwatch.Elapsed);
                }

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