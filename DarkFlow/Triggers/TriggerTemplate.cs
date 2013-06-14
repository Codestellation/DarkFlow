using System;
using System.Collections.Generic;
using System.Threading;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Triggers
{
    public abstract class TriggerTemplate : Trigger
    {
        private readonly string _name;
        private Action<ITask> _callback;
        private HashSet<ITask> _tasks;

        protected TriggerTemplate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name must be not null not empty string", "name");
            }

            _name = name;
            _tasks = new HashSet<ITask>();
        }

        public sealed override string Name
        {
            get { return _name; }
        }

        public sealed override IEnumerable<ITask> AttachedTasks
        {
            get { return _tasks; }
        }

        public sealed override void AttachTask(ITask task)
        {
            EnsureNotNull(task);
            if (!CollectionUtils.ThreadSafeAdd(ref _tasks, task)) return;

            string message = string.Format("Task {0} already added to trigger {1}", task, Name);
            throw new InvalidOperationException(message);
        }

        public sealed override void DetachTask(ITask task)
        {
            EnsureNotNull(task);
            if(!CollectionUtils.ThreadSafeRemove(ref _tasks, task)) return;

            string message = string.Format("Task {0} not found at trigger {1}", task, Name);
            throw new InvalidOperationException(message);
        }

        protected void ExecuteTasks()
        {
            foreach (ITask attachedTask in AttachedTasks)
            {
                _callback(attachedTask);
            }
        }

        protected sealed internal override void Start(Action<ITask> triggerCallback)
        {
            if (triggerCallback == null)
            {
                throw new ArgumentNullException("triggerCallback");
            }

            var beforeCas = _callback;
            Thread.MemoryBarrier();
            
            var afterCas = Interlocked.CompareExchange(ref _callback, triggerCallback, beforeCas);

            if (beforeCas != afterCas)
            {
                throw new InvalidOperationException("Method Start should be called once.");
            }

            OnStart();
        }

        protected abstract void OnStart();

        private static void EnsureNotNull(ITask task)
        {
            if (task != null) return;
            throw new ArgumentNullException("task");
        }
    }
}