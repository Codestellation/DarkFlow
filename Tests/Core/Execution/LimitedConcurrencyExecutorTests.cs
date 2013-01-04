using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class LimitedConcurrencyExecutorTests
    {
        [SetUp]
        public void Setup()
        {
            _executor = new LimitedConcurrencyExecutor(new TaskRepository(new JsonSerializer(new DefaultTaskFactory()), new InMemoryDatabase()), DefaultReleaser.Instance, 2);
            _tasks = new List<LongRunningTask>(10);
            for (int i = 0; i < 10; i++)
            {
                var task = new LongRunningTask();
                _tasks.Add(task);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _executor.Dispose();
        }

        private LimitedConcurrencyExecutor _executor;
        private List<LongRunningTask> _tasks;


        [Test]
        public void Do_not_exit_dispose_until_running_task_executed()
        {
            RunAll();
            _tasks[0].Started.WaitOne();

            _executor.Dispose();

            _tasks[0].Finished.WaitOne(100);
            _tasks[1].Finished.WaitOne(100);
            
            WaitAllTaskFinished(200);

            int executedCount = _tasks.Count(x => x.Executed);

            Assert.That(executedCount, Is.EqualTo(2));
        }

        [Test]
        public void Shedules_all_tasks()
        {
            RunAll();

            WaitAllTaskFinished(1000);

            int executedCount = _tasks.Count(x => x.Executed);

            Assert.That(executedCount, Is.EqualTo(10));
        }

        private void RunAll()
        {
            foreach (LongRunningTask task in _tasks)
            {
                _executor.Execute(task);
            }
        }

        [Test]
        public void Shedules_not_more_than_specified_number_of_tasks_simultaneously()
        {
            RunAll();

            _tasks[0].Started.WaitOne();
            _tasks[1].Started.WaitOne();

            int runningCount = _tasks.Count(x => x.Running);

            Assert.That(runningCount, Is.EqualTo(2));
        }

        private void WaitAllTaskFinished(int timeout = 100)
        {
            WaitHandle.WaitAll(_tasks.Select(x => x.Finished).Cast<WaitHandle>().ToArray(), timeout);
        }
    }
}