using System;

namespace Codestellation.DarkFlow.Misc
{
    public class FactoryTask<TTask> : ITask
        where TTask : ITask
    {
        private readonly Func<TTask> _taskFactory;

        public FactoryTask(Func<TTask> taskFactory)
        {
            _taskFactory = taskFactory;
        }

        public void Execute()
        {
            var task = _taskFactory();
            task.Execute();
        }
    }
}