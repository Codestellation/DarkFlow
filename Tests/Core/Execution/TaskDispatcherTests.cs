using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class TaskDispatcherTests
    {
        private QueuedExecutor _queue;
        private TaskDispatcher _pool;
        private List<LongRunningTask> _tasks;
        private QueuedExecutor _queue2;

        [SetUp]
        public void SetUp()
        {
            _tasks = new List<LongRunningTask>();
            var settings = new QueuedExecutorSettings{ Name = "test" };
            _queue = new QueuedExecutor(settings, NullPersister.Instance);
            
            var settings2 = new QueuedExecutorSettings{ Name = "test" };
            _queue2 = new QueuedExecutor(settings2, NullPersister.Instance);

            _pool = new TaskDispatcher(2, new IExecutionQueue[] { _queue, _queue2 });
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var task in _tasks)
            {
                task.Finilize();
            }
            _tasks = null;
        }

        [Test]
        public void Starts_thread_if_a_task_added()
        {
            var task = new LongRunningTask(false);
            var envelope = new ExecutionEnvelope(task, DefaultReleaser.Instance);
            _queue.Enqueue(envelope);

            task.WaitForFinish();

            Assert.That(task.Executed, Is.True);
        }

        [Test]
        public void Does_not_exceed_max_concurrency()
        {
            Console.WriteLine("Current thread is {0}", Thread.CurrentThread.ManagedThreadId);
            var shouldRun1 = CreateTask("Run1");
            var shouldRun2 = CreateTask("Run2");
            var shouldWait = CreateTask("Wait");

            _queue2.Enqueue(shouldRun1);

            ((LongRunningTask)shouldRun1.Task).WaitForStart(1000);

            
            _queue.Enqueue(shouldRun2);
            _queue.Enqueue(shouldWait);
            
            ((LongRunningTask)shouldRun2.Task).WaitForStart(100);

            Assert.That(((LongRunningTask)shouldRun1.Task).Running, Is.True, "First task did not started");
            Assert.That(((LongRunningTask)shouldRun2.Task).Running, Is.True, "Second task did not started");
            Assert.That(((LongRunningTask)shouldWait.Task).Running, Is.False, "Third task started, exceed concurrency");
        }

        [Test]
        public void Does_not_exceed_max_concurrency_for_queue()
        {
            var shouldRun = CreateTask("Run");
            var shouldWait = CreateTask("Wait");

            _queue.Enqueue(shouldRun);
            _queue.Enqueue(shouldWait);

            ((LongRunningTask)shouldRun.Task).WaitForStart(10000);
            ((LongRunningTask)shouldWait.Task).WaitForStart();

            Assert.That(((LongRunningTask)shouldRun.Task).Running, Is.True, "First task did not started.");
            Assert.That(((LongRunningTask)shouldWait.Task).Running, Is.False, "Second task started. Concurrency limit exceeded");
        }

        [Test]
        public void Dispose_not_returns_until_all_tasks_finished()
        {
            var envelope = CreateTask("Run");
            _queue.Enqueue(envelope);

            Assert.That(((LongRunningTask)envelope.Task).WaitForStart(), Is.True);

            Task.Factory.StartNew( () => 
            {
                while (_pool.Disposed == false)
                {
                    
                }
                ((LongRunningTask)envelope.Task).Finilize();
            });

            _pool.Dispose();

            Assert.That(((LongRunningTask)envelope.Task).Executed, Is.True);
        }

        private  ExecutionEnvelope CreateTask( string name, bool manualFinish = true)
        {
            var result = new LongRunningTask(manualFinish){Name = name};
            _tasks.Add(result);

            return new ExecutionEnvelope(result, DefaultReleaser.Instance);
        }
    }
}