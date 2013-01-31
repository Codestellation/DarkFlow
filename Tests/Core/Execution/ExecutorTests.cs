using System.Linq;
using Codestellation.DarkFlow.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Execution
{
    [TestFixture]
    public class ExecutorTests
    {
        [Test, Ignore("Not finished yet.")]
        public void Ordered_tasks_are_executed_serially()
        {
            var executor = new Executor(new ITaskQueue[]{new TaskQueue(x => true, () => { }, 3) });

            var tasks = Enumerable.Range(1, 2).Select(x => new LongRunningTask(true) {Name = "Task" + x}).ToArray();

            foreach (var task in tasks)
            {
                executor.Execute(task);
            }

            Assert.That(tasks[0].WaitForStart(), Is.True);

            Assert.That(tasks[0].WaitForStart(), Is.False);
        }
    }
}