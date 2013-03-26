using System;
using Codestellation.DarkFlow.Database;

namespace Codestellation.DarkFlow.Config
{
    public class QueuedExecutorSettings
    {
        private string _name;
        private  Region _region;
        private byte _maxConcurrency;

        public QueuedExecutorSettings()
        {
            MaxConcurrency = 1;
            Priority = 5;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _region = new Region(value);
            }
        }

        internal Region Region
        {
            get { return _region; }
        }

        public byte Priority { get; set; }

        public byte MaxConcurrency
        {
            get { return _maxConcurrency; }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The value should be greater than 0.");
                }
                _maxConcurrency = value;
            }
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentException(string.Format("Expected not null not empty, but was '{0}'", Name ?? "<null>"), "Name");
            }
        }
    }
}