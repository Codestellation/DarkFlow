namespace Codestellation.DarkFlow.Execution
{
    public class TaskQueueSettings
    {
        private readonly string _name;
        private readonly byte _priority;
        private readonly byte _maxConcurrency;

        public TaskQueueSettings(string name, byte priority, byte maxConcurrency)
        {
            _name = name;
            _priority = priority;
            _maxConcurrency = maxConcurrency;
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
    }
}