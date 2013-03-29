using System;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    public class DefaultReleaser : ITaskReleaser
    {
        protected readonly Logger Logger;

        private static readonly DefaultReleaser _instance = new DefaultReleaser();

        public DefaultReleaser()
        {
            Logger = LogManager.GetLogger(GetType().FullName);
        }

        public static ITaskReleaser Instance
        {
            get { return _instance; }
        }

        public virtual void Release(ITask task)
        {
            try
            {
                var disposable = task as IDisposable;

                if (disposable == null)
                {
                    return;
                }

                disposable.Dispose();

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Task {0} successfully disposed.");
                }
            }
            catch (Exception ex)
            {
                if (!Logger.IsErrorEnabled) return;

                var errorMessage = string.Format("Task {0} of type {1} throws exception on dispose.", task, task.GetType());
                Logger.ErrorException(errorMessage, ex);
            }
        }
    }
}