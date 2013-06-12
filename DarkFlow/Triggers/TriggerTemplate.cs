using System;
using System.Collections.Generic;
using System.Threading;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Triggers
{
    public abstract class TriggerTemplate : Trigger
    {
        private readonly string _id;
        private Action<ITask> _callback;
        private HashSet<ITask> _tasks;

        protected TriggerTemplate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id must be not null not empty string", "id");
            }

            _id = id;
            _tasks = new HashSet<ITask>();
        }

        public sealed override string Id
        {
            get { return _id; }
        }

        public sealed override IEnumerable<ITask> AttachedTasks
        {
            get { return _tasks; }
        }

        public sealed override void AttachTask(ITask task)
        {
            ModifyTasks(SafeAttach, task);
        }

        public sealed override void DetachTask(ITask task)
        {
            ModifyTasks(SafeDetach, task);
        }

        private HashSet<ITask> SafeDetach(HashSet<ITask> original, ITask task)
        {
            var newTasks = new HashSet<ITask>(original);
            if (!newTasks.Remove(task))
            {
                string message = string.Format("Task {0} not found at trigger {1}", task, Id);
                throw new InvalidOperationException(message);
            }
            return newTasks;
        }

        private HashSet<ITask> SafeAttach(HashSet<ITask> original, ITask task)
        {
            if (original.Contains(task))
            {
                string message = string.Format("Task {0} already added to trigger {1}", task, Id);
                throw new InvalidOperationException(message);
            }

            var newTasks = new HashSet<ITask>(original) { task };
            return newTasks;
        }

        private void ModifyTasks(Func<HashSet<ITask>, ITask, HashSet<ITask>> modifier, ITask task)
        {
            HashSet<ITask> afterCas;
            HashSet<ITask> beforeCas;
            do
            {
                beforeCas = _tasks;
                Thread.MemoryBarrier();

                var newTasks = modifier(beforeCas, task);

                afterCas = Interlocked.CompareExchange(ref _tasks, newTasks, beforeCas);
            } while (beforeCas != afterCas);
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
            Contract.Require(triggerCallback != null, "triggerCallback != null");
            if (_callback != null)
            {
                throw new InvalidOperationException("Method Start should be called once.");
            }
            Interlocked.Exchange(ref _callback, triggerCallback);
            OnStart();
        }

        protected abstract void OnStart();
    }
}