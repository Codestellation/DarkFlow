using System;
using System.Threading;

namespace Codestellation.DarkFlow.Tests
{
    public class LongRunningTask : ITask
    {
        private int _runCount;
        private readonly ManualResetEvent _started;
        private readonly ManualResetEvent _finished;
        private readonly ManualResetEvent _canFinish;

        public LongRunningTask(bool manualFinish)
        {
            _started = new ManualResetEvent(false);
            _finished = new ManualResetEvent(false);
            _canFinish = new ManualResetEvent(!manualFinish);
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
            return _started.WaitOne(timeout);
        }

        public bool WaitForFinish(int timeout = 10)
        {
            return _finished.WaitOne(timeout);
        }

        public void Finilize()
        {
            _canFinish.Set();
        }

        public void Execute()
        {
            Running = true;
            _started.Set();
            
            Console.WriteLine("Running task {0}, at {1}", Name, DateTime.Now);
            _canFinish.WaitOne();
            
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