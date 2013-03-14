using System;
using System.Collections.Generic;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Matchers
{
    public class NamespaceMatcher : IMatcher
    {
        private readonly string _ns;
        private readonly string _queueName;
        private readonly Dictionary<Type, bool> _matchedCache;
        public const string All = "*";

        public NamespaceMatcher(string @namespace, string queueName)
        {
            Contract.Require(!string.IsNullOrWhiteSpace(@namespace), "!string.IsNullOrWhiteSpace(@namespace)");
            Contract.Require(!string.IsNullOrWhiteSpace(queueName), "!string.IsNullOrWhiteSpace(queueName)");
            
            _ns = @namespace.Replace(All, string.Empty);
            _queueName = queueName;
            _matchedCache = new Dictionary<Type, bool>();
        }

        public string QueueName
        {
            get { return _queueName; }
        }

        public bool Match(ITask task)
        {
            Contract.Require(task != null, "task != null");

            var type = task.GetType();
            var result = _matchedCache.GetOrAddThreadSafe(type, () => Match(type));
            return result;
        }

        private bool Match(Type type)
        {
            return type.FullName.StartsWith(_ns) || _ns == All;
        }
    }
}