using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codestellation.DarkFlow.Database;
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
                var task = new LongRunningTask(true);
                _tasks.Add(task);
            }
        }

        [TearDown]
        public void TearDown()
        {
            FinalizeAll();
            _executor.Dispose();
        }

        private void FinalizeAll()
        {
            foreach (var longRunningTask in _tasks)
            {
                longRunningTask.Finilize();
            }
        }

        private LimitedConcurrencyExecutor _executor;
        private List<LongRunningTask> _tasks;


        [Test]
        public void Do_not_exit_dispose_until_running_task_executed()
        {
            _executor.Start();
            RunAll();
            _tasks[0].WaitForStart(100);

            ThreadPool.QueueUserWorkItem(state =>
                {
                    while (_executor.DisposeInProgress == false)
                    {

                    }
                    FinalizeAll();
                });

            _executor.Dispose();

            _tasks[0].WaitForFinish(100);
            _tasks[1].WaitForFinish(100);

            WaitAllTaskFinished(200, false);

            int executedCount = _tasks.Count(x => x.Executed);

            Assert.That(executedCount, Is.EqualTo(2));
        }

        [Test]
        public void Shedules_all_tasks()
        {
            _executor.Start();
            RunAll(autofinish: true);

            WaitAllTaskFinished();

            int executedCount = _tasks.Count(x => x.Executed);

            Assert.That(executedCount, Is.EqualTo(10));
        }

        [Test]
        public void Shedules_not_more_than_specified_number_of_tasks_simultaneously()
        {
            _executor.Start();
            RunAll();

            _tasks[0].WaitForStart(100);
            _tasks[1].WaitForStart(100);

            int runningCount = _tasks.Count(x => x.Running);

            Assert.That(runningCount, Is.EqualTo(2));
        }

        [Test]
        public void Do_not_executes_tasks_until_start_is_called()
        {
            RunAll();
            var started = _tasks[0].WaitForStart();

            Assert.That(started, Is.False);
        }

        [Test]
        public void Do_not_executes_tasks_since_stopped()
        {
            _executor.Start();
            _executor.Stop();
            RunAll();
            var started = _tasks[0].WaitForStart();

            Assert.That(started, Is.False);
        }

        [Test]
        public void Do_not_executes_new_tasks_since_stopped()
        {  
            _executor.Start();
            RunAll();
            _tasks[0].WaitForStart(100);
            _tasks[1].WaitForStart(100);
            _executor.Stop();
            FinalizeAll();

            var startedCount = _tasks.Count(x => x.Executed);

            Assert.That(startedCount, Is.EqualTo(2));
        }

        private void WaitAllTaskFinished(int timeout = 100, bool ensure = true)
        {
            var finished = _tasks.Select(x => x.WaitForFinish(timeout)).Aggregate((a, b) => a && b);
            if (ensure)
            {
                Assert.That(finished, Is.True, string.Format("At least one task was not finished within {0} ms.", timeout));
            }

        }

        private void RunAll(bool autofinish = false)
        {
            foreach (LongRunningTask task in _tasks)
            {
                _executor.Execute(task);
                if (autofinish)
                {
                    task.Finilize();
                }
            }
        }
    }
}
