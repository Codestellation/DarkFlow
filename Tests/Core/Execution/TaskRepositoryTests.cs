using System;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    public class PersistedTask : ITask
    {
        private readonly State _state;

        public PersistedTask(State state)
        {
            _state = state;
        }

        public void Execute()
        {
            Console.WriteLine(PersistentState);
        }

        public object PersistentState
        {
            get { return State; }
        }

        public State State
        {
            get { return _state; }
        }
    }
    
    public class State
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            var other = (State) obj;

            return other.Id.Equals(Id) && other.Name.Equals(Name);
        }
    }
}