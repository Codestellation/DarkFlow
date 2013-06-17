using System;

namespace Codestellation.DarkFlow.Stat
{
    public interface IMonitor
    {
        bool Enabled { get; }
        void Increment(string counterName, TimeSpan duration);
    }
}