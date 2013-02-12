using System;
using Codestellation.DarkFlow.Misc;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    internal class TaskExecutionWrap : ITask
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ITask _task;
        private readonly Action _afterExecute;

        public TaskExecutionWrap(ITask task, Action afterExecute)
        {
            Contract.Require(task != null, "task != null");
            Contract.Require(afterExecute != null, "afterExecute != null");

            _task = task;
            _afterExecute = afterExecute;
        }

        public void Execute()
        {
            try
            {
                _task.Execute();
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                {
                    Logger.ErrorException("Task failed.",ex);    
                }
            }
            finally
            {
                _afterExecute();
            }
        }
    }
}