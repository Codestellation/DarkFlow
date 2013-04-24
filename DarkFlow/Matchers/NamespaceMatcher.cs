using System;
using System.Collections.Concurrent;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Matchers
{
    public class NamespaceMatcher : IMatcher
    {
        public const string AnyWildCard = "*";

        private readonly string _nameSpace;
        private readonly ConcurrentDictionary<Type, MatchResult> _cache;
        private readonly MatchResult _matchResult;

        public NamespaceMatcher(string queueName, string nameSpace) 
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("Should be not empty not null string.", "queueName");
            }

            if (string.IsNullOrWhiteSpace(nameSpace))
            {
                throw new ArgumentException("Should be not empty not null string.", "nameSpace");
            }

            _nameSpace = nameSpace.Replace(AnyWildCard, string.Empty);
            _cache = new ConcurrentDictionary<Type, MatchResult>();
            _matchResult = MatchResult.Matches(queueName);
        }

        public MatchResult TryMatch(ITask task)
        {
            Contract.Require(task != null, "task != null");
            
            var type = task.GetType();
            MatchResult result;
            if (_cache.TryGetValue(type, out result))
            {
                return result;
            }
            return _cache.GetOrAdd(type, ProbeNamespace);
        }

        private MatchResult ProbeNamespace(Type type)
        {
            var result = type.FullName.StartsWith(_nameSpace) || _nameSpace == AnyWildCard;
            
            return result ? _matchResult : MatchResult.NonMatched;
        }
    }
}