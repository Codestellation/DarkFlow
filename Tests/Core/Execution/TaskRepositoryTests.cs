using System;
using System.Linq;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;
using System.Collections.Generic;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class TaskRepositoryTests
    {
        private ManagedEsentDatabase _database;
        public ITaskRepository Repository { get; set; }

        [SetUp]
        public void Setup()
        {
            Utils.SafeDeleteDirectory(ManagedEsentDatabase.DefaultTaskFolder);
            _database = new ManagedEsentDatabase();
            Repository = new TaskRepository(new JsonSerializer(new DefaultTaskFactory()),_database );
        }

        [TearDown]
        public void TearDown()
        {
            if (_database == null) return;
            _database.Dispose();
        }


        [Test]
        public void Should_return_added_task()
        {
            var task = new LongRunningTask(false);
            Repository.Add(task);

            var returnedTask = Repository.TakeNext();

            Assert.That(returnedTask, Is.EqualTo(task));
        }

        [Test]
        public void Should_return_null_if_no_task_added()
        {
            var result = Repository.TakeNext();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Should_reload_persisted_task()
        {
            var original = new State {Id = 1, Name = "Test"};
            var task = new PersistedTask(original);
            Repository.Add(task);
            _database.Dispose();
            _database = new ManagedEsentDatabase();

            Repository = new TaskRepository(new JsonSerializer(new DefaultTaskFactory()), _database);

            var reloaded = (PersistedTask)Repository.TakeNext();

            Assert.That(reloaded, Is.InstanceOf<PersistedTask>());
            Assert.That(reloaded.State, Is.EqualTo(original));
        }


        [Test, Explicit]
        public void Test_performance_for_add_method()
        {
            var tasks = new List<IPersistentTask>(10000);
            tasks.AddRange(Enumerable.Range(1, 10000).Select(x => new PersistedTask(new State { Id = x, Name = "Task" + x })));
            foreach (var task in tasks)
            {
                Repository.Add(task);
            }
        }
    }

    public class PersistedTask : IPersistentTask
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