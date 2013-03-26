using System.Linq;
using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Matchers;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class ExecutorTests
    {
        [Test]
        public void Ordered_tasks_are_executed_serially()
        {
            var taskQueueSettings = new QueuedExecutorSettings{Name = "test"};
            var taskQueue = new QueuedExecutor(taskQueueSettings, NullPersister.Instance);
            var matcher = new NamespaceMatcher(taskQueue.Name, NamespaceMatcher.AnyWildCard);
            var taskRouter = new TaskRouter(matcher, new IExecutorImplementation[]{taskQueue });
            var dispatcher = new TaskDispatcher(2, new IExecutionQueue[] {taskQueue});
            var executor = new Executor(taskRouter, dispatcher, new DefaultReleaser());

            var tasks = Enumerable.Range(1, 2).Select(x => new LongRunningTask(true) {Name = "Task" + x}).ToArray();

            foreach (var task in tasks)
            {
                executor.Execute(task);
            }

            Assert.That(tasks[0].WaitForStart(), Is.True);

            Assert.That(tasks[1].WaitForStart(), Is.False);
        }
    }
}