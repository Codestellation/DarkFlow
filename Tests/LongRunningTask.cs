using System;
using System.Threading;

namespace Codestellation.DarkFlow.Tests
{
    public class LongRunningTask : ITask
    {
        private int _runCount;
        private readonly ManualResetEventSlim _started;
        private readonly ManualResetEventSlim _finished;
        private readonly ManualResetEventSlim _canFinish;

        public LongRunningTask(bool manualFinish)
        {
            _started = new ManualResetEventSlim(false);
            _finished = new ManualResetEventSlim(false);
            _canFinish = new ManualResetEventSlim(!manualFinish);
        }

        public bool Running { get; private set; }

        public bool Executed { get; private set; }

        public string Name { get; set; }

        public int RunCount
        {
            get { return _runCount; }
        }

        public bool WaitForStart(int timeout = 10)
        {
            return _started.Wait(timeout);
        }

        public bool WaitForFinish(int timeout = 10)
        {
            return _finished.Wait(timeout);
        }

        public void Finilize()
        {
            _canFinish.Set();
        }

        public void Execute()
        {
            Running = true;
            _started.Set();
            
            Console.WriteLine("Running task {0}, at {1}. Thread {2}", Name, DateTime.Now, Thread.CurrentThread.ManagedThreadId);
            _canFinish.Wait();
            
            Running = false;
            Executed = true;
            Interlocked.Increment(ref _runCount);
            _finished.Set();
        }
    }

    public class PersistentLongRunningTask : LongRunningTask, IPersistentTask
    {
        public PersistentLongRunningTask(bool manualFinish) : base(manualFinish)
        {
        }

        public object PersistentState { get { return Name; } }
    }
}