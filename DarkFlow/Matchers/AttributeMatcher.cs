using System;
using System.Collections.Concurrent;
using System.Linq;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Matchers
{
    public class AttributeMatcher : AbstractMatcher
    {
        private readonly Type[] _attributes;
        private ConcurrentDictionary<Type, bool> _cache;

        public AttributeMatcher(string queueName, params Type[] attributes) :base(queueName)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }
            _attributes = attributes;
            _cache = new ConcurrentDictionary<Type, bool>();
        }

        protected override bool Match(ITask task)
        {
            Contract.Require(task != null, "task != null");
            
            var type = task.GetType();
            var result = false;
            
            if (_cache.TryGetValue(type, out result))
            {
                return result;
            }
            
            result = PerformAttributesMatch(type);
            return _cache.GetOrAdd(type, result);
        }

        private bool PerformAttributesMatch(Type type)
        {
            var customAttributes = type.GetCustomAttributes(true);

            var result = customAttributes.Select(x => x.GetType()).Intersect(_attributes).Any();

            return result;
        }
    }
}