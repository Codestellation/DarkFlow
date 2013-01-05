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
            _executor.Start();
            RunAll();
            _tasks[0].Started.WaitOne(100);

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
            _executor.Start();
            RunAll();

            WaitAllTaskFinished(1000);

            int executedCount = _tasks.Count(x => x.Executed);

            Assert.That(executedCount, Is.EqualTo(10));
        }

        [Test]
        public void Shedules_not_more_than_specified_number_of_tasks_simultaneously()
        {
            _executor.Start();
            RunAll();

            _tasks[0].Started.WaitOne(100);
            _tasks[1].Started.WaitOne(100);

            int runningCount = _tasks.Count(x => x.Running);

            Assert.That(runningCount, Is.EqualTo(2));
        }

        [Test]
        public void Do_not_executes_tasks_until_start_is_called()
        {
            RunAll();
            var started = _tasks[0].Started.WaitOne(10);

            Assert.That(started, Is.False);
        }
        
        [Test]
        public void Do_not_executes_tasks_since_stopped()
        {
            _executor.Start();
            _executor.Stop();
            RunAll();
            var started = _tasks[0].Started.WaitOne(10);

            Assert.That(started, Is.False);
        }

        [Test]
        public void Do_not_executes_new_tasks_since_stopped()
        {
            _executor.Start();
            RunAll();
            _tasks[0].Started.WaitOne(10);
            _executor.Stop();
            _tasks[9].Finished.WaitOne(10000);

            var startedCount = _tasks.Count(x => x.Executed);

            Assert.That(startedCount, Is.LessThan(10));
        }

        private void WaitAllTaskFinished(int timeout = 100)
        {
            WaitHandle.WaitAll(_tasks.Select(x => x.Finished).Cast<WaitHandle>().ToArray(), timeout);
        }

        private void RunAll()
        {
            foreach (LongRunningTask task in _tasks)
            {
                _executor.Execute(task);
            }
        }
    }
}
