using System;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class DefalutTaskFactoryTests
    {
        private State _state;
        private DefaultTaskFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _state = new State {Id = 1, Name = "Test"};
            _factory = new DefaultTaskFactory();
        }

        [Test]
        public void Creates_tasks_with_arguments_constructor()
        {
            var createdTask = _factory.Create(typeof (PersistentTaskWithConstructor).AssemblyQualifiedName, _state);

            Assert.That(createdTask.PersistentState, Is.EqualTo(_state));
        }

        [Test]
        public void Create_task_with_default_contructor_and_injects_property()
        {
            var createdTask = _factory.Create(typeof(PersistentTaskWithDefaultConstructorAndProperty).AssemblyQualifiedName, _state);

            Assert.That(createdTask.PersistentState, Is.EqualTo(_state));
        }

        [Test]
        public void Throws_if_cannot_create_task()
        {
            Assert.Throws<InvalidOperationException>(() => _factory.Create(typeof(NonSupportedTask).AssemblyQualifiedName, _state));
        }
    }

    public class PersistentTaskWithConstructor : IPersistentTask
    {
        private readonly State _state;

        public PersistentTaskWithConstructor(State state)
        {
            _state = state;
        }

        public void Execute()
        {
            
        }

        public object PersistentState
        {
            get { return _state; }
        }
        public Type GetRealType { get; private set; }
    }

    public class PersistentTaskWithDefaultConstructorAndProperty : IPersistentTask
    {
        public PersistentTaskWithDefaultConstructorAndProperty()
        {
        }

        public void Execute() {}

        public object PersistentState { get { return State; } }

        public State State { get; set; }

        public Type GetRealType { get; private set; }
    }

    public class NonSupportedTask : IPersistentTask
    {
        private State _state;

        public NonSupportedTask(int id, string name)
        {
            _state = new State {Id = id, Name = name};
        }

        public void Execute()
        {
            
        }

        public object PersistentState { get { return _state; } }

        public Type GetRealType { get; private set; }
    }
}