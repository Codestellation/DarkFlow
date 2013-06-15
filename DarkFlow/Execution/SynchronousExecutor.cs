using System;
using Codestellation.DarkFlow.Database;

namespace Codestellation.DarkFlow.Execution
{
    public class SynchronousExecutor : IExecutor, IExecutorImplementation
    {
        private readonly string _name;
        private readonly Region _region;
        public const string DefaultName = "Sync";

        public SynchronousExecutor() : this(DefaultName)
        {
        }

        public SynchronousExecutor(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Argument name should be not null not empty string", "name");
            }
            _name = name;
            _region = new Region(name);
        }

        public void Execute(ITask task)
        {
            task.Execute();
        }

        public string Name
        {
            get { return _name; }
        }

        public Region Region
        {
            get { return _region; }
        }

        public void Enqueue(ExecutionEnvelope envelope)
        {
            Execute(envelope.Task);
        }
    }
}