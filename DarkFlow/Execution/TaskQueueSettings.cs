using System;
using Codestellation.DarkFlow.Database;

namespace Codestellation.DarkFlow.Execution
{
    public class TaskQueueSettings
    {
        private readonly string _name;
        private readonly byte _priority;
        private readonly byte _maxConcurrency;
        private readonly Region _region;

        public TaskQueueSettings(string name, byte priority, byte maxConcurrency)
        {
            _name = name;
            _priority = priority;
            _maxConcurrency = maxConcurrency;
            _region = new Region(name);
        }

        public string Name
        {
            get { return _name; }
        }

        public byte Priority
        {
            get { return _priority; }
        }

        public byte MaxConcurrency
        {
            get { return _maxConcurrency; }
        }

        public Region Region
        {
            get { return _region; }
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException(string.Format("Expected not null not empty, but was '{0}'", Name ?? "<null>"), "name");
            }
        }
    }
}