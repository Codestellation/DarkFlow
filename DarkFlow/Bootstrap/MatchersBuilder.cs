using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class MatchersBuilder
    {
        private readonly List<IIMatcherBuilder> _builders;

        public MatchersBuilder()
        {
            _builders = new List<IIMatcherBuilder>();
        }

        public NamespaceMatcherBuilder ByNamespace(string mask)
        {
            var matcherBuilder = new NamespaceMatcherBuilder {Mask = mask};
            _builders.Add(matcherBuilder);
            return matcherBuilder;
        }

        public AttributeMatcherBuilder MarkedWith(params Type[] attributeTypes)
        {
            return new AttributeMatcherBuilder {AttributeTypes = attributeTypes};
        }

        public IMatcher ToMatcher()
        {
            if (_builders.Count == 1)
            {
                return _builders[0].ToMatcher();
            }

            var matchers = _builders.Select(x => x.ToMatcher()).ToArray();

            return new AggregateMatcher(matchers);
        }
    }
}