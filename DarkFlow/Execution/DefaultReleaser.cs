using System;
using System.Threading;
using NLog;

namespace Codestellation.DarkFlow.Execution
{
    public class DefaultReleaser : ITaskReleaser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Lazy<DefaultReleaser> LazyInstance = new Lazy<DefaultReleaser>(() => new DefaultReleaser());

        public static  ITaskReleaser Instance
        {
            get { return LazyInstance.Value; }
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