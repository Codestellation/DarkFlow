using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Execution
{
    public class NamespaceMatcher : IMatcher
    {
        private readonly string _ns;
        private readonly string _queueName;

        public NamespaceMatcher(string @namespace, string queueName)
        {
            Contract.Require(!string.IsNullOrWhiteSpace(@namespace), "!string.IsNullOrWhiteSpace(@namespace)");
            Contract.Require(!string.IsNullOrWhiteSpace(queueName), "!string.IsNullOrWhiteSpace(queueName)");
            _ns = @namespace;
            _queueName = queueName;
        }

        public string QueueName
        {
            get { return _queueName; }
        }

        public bool Match(ITask task)
        {
            Contract.Require(task != null, "task != null");

            return task.GetType().FullName.StartsWith(_ns);
        }
    }
}