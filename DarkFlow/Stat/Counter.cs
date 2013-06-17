using System;
using System.Threading;

namespace Codestellation.DarkFlow.Stat
{
    public class Counter
    {
        private readonly string _name;
        private long _entries;
        private long _totalDuration;
        private long _maxDuration;
        private long _minDuration;

        public Counter(string name)
        {
            _name = name;
            _minDuration = long.MaxValue;
        }

        public string Name
        {
            get { return _name; }
        }

        public long Entries
        {
            get { return _entries; }
        }

        public TimeSpan TotalDuration
        {
            get { return TimeSpan.FromTicks(_totalDuration); }
        }

        public TimeSpan MaxDuration
        {
            get { return TimeSpan.FromTicks(_maxDuration); }
        }

        public TimeSpan MinDuration
        {
            get { return TimeSpan.FromTicks(_minDuration); }
        }

        public TimeSpan AvgDuration
        {
            get
            {
                if (_entries == 0) return TimeSpan.FromSeconds(0);
                return TimeSpan.FromTicks(_totalDuration/_entries);
            }
        }

        public void Increment(TimeSpan duration)
        {
            Interlocked.Increment(ref _entries);
            
            var currentDuration = duration.Ticks;
            Interlocked.Add(ref _totalDuration, currentDuration);

            SetData(ref _minDuration, currentDuration, (min, cur) => min < cur);
            SetData(ref _maxDuration, currentDuration, (max, cur) => max > cur);
        }

        private void SetData(ref long location, long dataToSet, Func<long, long, bool> exitCase)
        {
            if(exitCase(location, dataToSet)) return;
            
            long beforeCas;
            long afterCas;
            do
            {
                beforeCas = Thread.VolatileRead(ref location);
                
                if (exitCase(beforeCas, dataToSet))return;

                afterCas = Interlocked.CompareExchange(ref location, dataToSet, beforeCas);
            } while (beforeCas != afterCas);
        }
    }
}