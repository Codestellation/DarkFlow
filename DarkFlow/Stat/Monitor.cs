using System;
using System.Collections.Generic;
using System.Threading;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Stat
{
    public class Monitor : IMonitor
    {
        private Dictionary<string, Counter> _counters;

        public Monitor()
        {
            _counters = new Dictionary<string, Counter>();
        }

        public bool Enabled
        {
            get { return true; }
        }

        public void Increment(string counterName, TimeSpan duration)
        {
            var counter = GetCounter(counterName);

            counter.Increment(duration);
        }

        public Counter GetCounter(string name)
        {
            Counter counter;
            if (_counters.TryGetValue(name, out counter))
            {
                return counter;
            }
            counter = new Counter(name);
            if(CollectionUtils.ThreadSafeAdd(ref _counters, name, counter))
            {
                return counter;
            }
            var counters = _counters;
            
            Thread.MemoryBarrier();

            if (counters.TryGetValue(name, out counter))
            {
                return counter;
            }
            throw new InvalidOperationException("Some ugly race condition. Take a closer look at monitor class.");
        }
    }
}