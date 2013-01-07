using System;
using System.Threading;

namespace Codestellation.DarkFlow.Tests
{
    //TODO: Remove thread sleep completely. Make more intention revealing interface.
    public class LongRunningTask : ITask
    {
        private int _runCount;
        public int SleepTime = 50;
        public LongRunningTask()
        {
            Started = new ManualResetEvent(false);
            Finished = new ManualResetEvent(false);
        }

        public ManualResetEvent Started { get; set; }

        public ManualResetEvent Finished { get; set; }

        public bool Running { get; set; }

        public bool Executed { get; set; }

        public string Name { get; set; }

        public int RunCount
        {
            get { return _runCount; }
        }

        public void Execute()
        {
            Running = true;
            Started.Set();
            if (SleepTime > 0)
            {
                Thread.Sleep(SleepTime);
            }

            Console.WriteLine("Running task {0}, at {1}", Name, DateTime.Now);
            Running = false;
            Executed = true;
            Interlocked.Increment(ref _runCount);
            Finished.Set();
        }
    }

    public class PersistentLongRunningTask : LongRunningTask, IPersistentTask
    {
        public object PersistentState { get { return Name; } }
    }
}