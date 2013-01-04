using System;
using System.Collections.Generic;

namespace Codestellation.DarkFlow.Scheduling
{
    public class TestTimer : ITimer
    {
        private readonly List<DateTimeOffset> _callbackTimePoints;

        public TestTimer()
        {
            _callbackTimePoints = new List<DateTimeOffset>();
        }

        public void Dispose()
        {

        }

        public void CallbackAt(DateTimeOffset startAt)
        {
            _callbackTimePoints.Add(startAt);
        }

        public void FireCallback(DateTimeOffset at)
        {
            Callback(at);

        }

        public Action<DateTimeOffset> Callback { get; set; }

        public IEnumerable<DateTimeOffset> CallbackTimePoints
        {
            get { return _callbackTimePoints; }
        }

        public bool ContainsTimepoint(DateTimeOffset timePoint)
        {
            return _callbackTimePoints.Contains(timePoint);
        }
    }
}