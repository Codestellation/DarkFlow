using System;
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
                    Logger.ErrorException("Task throwed exception.",ex);    
                }
            }
            finally
            {
                _afterExecute();
            }
        }
    }
}