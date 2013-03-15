using System;
using System.Linq;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class PersisterTests
    {
        private Persister _persister;
        private PersistentTask _original;
        private Identifier _id;

        [SetUp]
        public void SetUp()
        {
            _id = new Identifier(Guid.NewGuid(), new Region("test"));
            _persister = new Persister(new InMemoryDatabase());
            _original = new PersistentTask(1) { TotalCount = 2 };
        }

        [Test]
        public void Can_save_task()
        {
            _persister.Persist(_id, _original);
        }

        [Test]
        public void Can_load_persisted_tasks()
        {
            _persister.Persist(_id, _original);

            var loaded = (PersistentTask)_persister.LoadAll(_id.Region).Single().Value;

            Assert.That(loaded.Count, Is.EqualTo(_original.Count));
            Assert.That(loaded.TotalCount, Is.EqualTo(_original.TotalCount));
        }

        [Test]
        public void Can_delete_persisted_task()
        {
            _persister.Persist(_id, _original);
            
            _persister.Delete(_id);

            var loaded = _persister.LoadAll(_id.Region);

            Assert.That(loaded, Is.Empty);
        }
    }

    public class PersistentTask : ITask
    {
        public static readonly PersistentTask Instance = new PersistentTask(10);
        
        private readonly int _count;
        private int _count2;

        public PersistentTask(int count)
        {
            _count = count;
            _count2 = 1;
        }

        public int Count
        {
            get { return _count; }
        }

        public int TotalCount { get; set; }

        internal int IgnorantProperty
        {
            get { return Count + TotalCount; }
        }

        public void Execute()
        {
            
        }
    }
}