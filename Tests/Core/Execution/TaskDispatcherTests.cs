using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class TaskDispatcherTests
    {
        private TaskQueue _queue;
        private TaskDispatcher _pool;
        private List<LongRunningTask> _tasks;
        private TaskQueue _queue2;

        [SetUp]
        public void SetUp()
        {
            _tasks = new List<LongRunningTask>();
            _queue = new TaskQueue("test", 3, 1) ;
            _queue2 = new TaskQueue("test", 2, 1);
            _pool = new TaskDispatcher(2, new IExecutionQueue[] { _queue, _queue2 });
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var task in _tasks)
            {
                task.Finilize();
            }
        }

        [Test]
        public void Starts_thread_if_a_task_added()
        {
            var task = new LongRunningTask(false);

            _queue.Enqueue(task);

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

            shouldRun1.WaitForStart(1000);

            
            _queue.Enqueue(shouldRun2);
            _queue.Enqueue(shouldWait);
            
            shouldRun2.WaitForStart(100);

            Assert.That(shouldRun1.Running, Is.True);
            Assert.That(shouldRun2.Running, Is.True);
            Assert.That(shouldWait.Running, Is.False);
        }

        [Test]
        public void Does_not_exceed_max_concurrency_for_queue()
        {
            var shouldRun = CreateTask("Run");
            var shouldWait = CreateTask("Wait");

            _queue.Enqueue(shouldRun);
            _queue.Enqueue(shouldWait);

            shouldRun.WaitForStart(100);
            shouldWait.WaitForStart();

            Assert.That(shouldRun.Running, Is.True);
            Assert.That(shouldWait.Running, Is.False);
        }

        [Test]
        public void Dispose_not_returns_until_all_tasks_finished()
        {
            var task = CreateTask("Run");
            _queue.Enqueue(task);

            Assert.That(task.WaitForStart(), Is.True);

            Task.Factory.StartNew( () => 
            {
                while (_pool.DisposeInProgress == false)
                {
                    
                }
                task.Finilize();
            });

            _pool.Dispose();

            Assert.That(task.Executed, Is.True);
        }

        private  LongRunningTask CreateTask( string name, bool manualFinish = true)
        {
            var result = new LongRunningTask(manualFinish){Name = name};
            _tasks.Add(result);
            return result;
        }
    }
}