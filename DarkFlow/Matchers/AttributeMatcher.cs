using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Matchers
{
    public class AttributeMatcher : AbstractMatcher
    {
        private readonly Type[] _attributes;
        private Dictionary<Type, bool> _cache;

        public AttributeMatcher(string queueName, params Type[] attributes) :base(queueName)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }
            _attributes = attributes;
            _cache = new Dictionary<Type, bool>();
        }

        protected override bool Match(ITask task)
        {
            Contract.Require(task != null, "task != null");
            
            var type = task.GetType();
            
            bool result;
            
            if (_cache.TryGetValue(type, out result))
            {
                return result;
            }
            
            result = PerformAttributesMatch(type);

            CollectionUtils.ThreadSafeAdd(ref _cache, type, result);

            return result;
        }

        private bool PerformAttributesMatch(Type type)
        {
            var customAttributes = type.GetCustomAttributes(true);

            var result = customAttributes.Select(x => x.GetType()).Intersect(_attributes).Any();

            return result;
        }
    }
}