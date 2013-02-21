using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class DefalutTaskFactoryTests
    {
        private DefaultTaskFactory _factory;
        private PersistentTask _persistentTask;

        [SetUp]
        public void SetUp()
        {
            _factory = new DefaultTaskFactory();
            _persistentTask = new PersistentTask(1) { TotalCount = 2 };
        }

        [Test]
        public void Gets_task_data_properly()
        {
            var data = _factory.GetTaskData(_persistentTask);
            Assert.That(data.TaskType, Is.EqualTo("Codestellation.DarkFlow.Tests.Core.Execution.PersistentTask, Codestellation.DarkFlow.Tests"));
            Assert.That(data.Properties.Length, Is.EqualTo(2));
        }

        [Test]
        public void Creates_tasks_with_arguments_constructor()
        {
            var data = _factory.GetTaskData(_persistentTask);

            var reburnishedTask = (PersistentTask)_factory.Create(data);

            Assert.That(reburnishedTask, Is.Not.Null);
            Assert.That(reburnishedTask.Count, Is.EqualTo(_persistentTask.Count), "Ctor argument was not supplied");
            Assert.That(reburnishedTask.TotalCount, Is.EqualTo(_persistentTask.TotalCount), "Settable property was not set.");
        }
    }

    public class PersistentTask : ITask
    {
        private readonly int _count;

        public PersistentTask(int count)
        {
            _count = count;
        }

        public int Count
        {
            get { return _count; }
        }

        public int TotalCount { get; set; }
        
        public int IgnorantProperty
        {
            get { return Count + TotalCount; }
        }

        public void Execute()
        {
            
        }
    }
}