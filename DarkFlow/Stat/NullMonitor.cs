using System;

namespace Codestellation.DarkFlow.Stat
{
    public class NullMonitor : IMonitor
    {
        public static NullMonitor Instance = new NullMonitor();

        public bool Enabled
        {
            get { return false; }
        }

        public void Increment(string counterName, TimeSpan duration)
        {
            throw new NotSupportedException();
        }
    }
}