using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class AggregateMatcherBuilder : IMatcherBuilder
    {
        private readonly List<IMatcherBuilder> _builders;

        public AggregateMatcherBuilder()
        {
            _builders = new List<IMatcherBuilder>();
        }

        public NamespaceMatcherBuilder ByNamespace(string mask)
        {
            var matcherBuilder = new NamespaceMatcherBuilder {Mask = mask};
            return AddBuilder(matcherBuilder);
        }

        public AttributeMatcherBuilder MarkedWith(params Type[] attributeTypes)
        {
            var builder = new AttributeMatcherBuilder {AttributeTypes = attributeTypes};
            return AddBuilder(builder);
        }

        private TMatcherBuilder AddBuilder<TMatcherBuilder>(TMatcherBuilder builder) 
            where TMatcherBuilder : IMatcherBuilder
        {
            _builders.Add(builder);
            return builder;
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