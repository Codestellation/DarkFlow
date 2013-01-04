using System;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture(typeof(InMemoryDatabase))]
    [TestFixture(typeof(ManagedEsentDatabase))]
    public class DatabaseTests<TDatabase> where TDatabase : IDatabase
    {
        private TDatabase _dataBase;
        private string _original;

        [SetUp]
        public void Setup()
        {
            Utils.SafeDeleteDirectory(ManagedEsentDatabase.DefaultTaskFolder);
            _original = "Just test";
            _dataBase = Activator.CreateInstance<TDatabase>();
        }

        [TearDown]
        public void TearDown()
        {
            var disposabe = _dataBase as IDisposable;
            
            if(disposabe == null)return;
            disposabe.Dispose();
        }

        [Test]
        public void Can_persist_string_to_database()
        {
            var id = _dataBase.Persist(_original);
            var loaded = _dataBase.Get(id);

            Assert.That(id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(loaded, Is.EqualTo(_original));
        }

        [Test]
        public void Removes_string_from_database()
        {
            var id = _dataBase.Persist(_original);
            
            _dataBase.Remove(id);

            var all = _dataBase.GetAll();

            Assert.That(all, Is.Empty);
        }

        [Test]
        public void Does_not_throw_if_delete_absent_task()
        {
            _dataBase.Remove(Guid.NewGuid());
        }
    }
}