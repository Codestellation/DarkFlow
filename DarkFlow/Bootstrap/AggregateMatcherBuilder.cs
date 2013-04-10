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


        public AttributeMatcherBuilder MarkedWith<TAttribute>()
            where TAttribute : Attribute
        {
            return MarkedWith(typeof (TAttribute));
        }

        public AttributeMatcherBuilder MarkedWith<TAttribute1, TAttribute2>()
            where TAttribute1  : Attribute
            where TAttribute2 : Attribute
        {
            return MarkedWith(typeof(TAttribute1), typeof(TAttribute2));
        }

        public AttributeMatcherBuilder MarkedWith<TAttribute1, TAttribute2, TAttribute3>()
            where TAttribute1 : Attribute
            where TAttribute2 : Attribute
            where TAttribute3 : Attribute
        {
            return MarkedWith(typeof(TAttribute1), typeof(TAttribute2), typeof(TAttribute3));
        }

        public AttributeMatcherBuilder MarkedWith<TAttribute1, TAttribute2, TAttribute3, TAttribute4>()
            where TAttribute1 : Attribute
            where TAttribute2 : Attribute
            where TAttribute3 : Attribute
            where TAttribute4 : Attribute
        {
            return MarkedWith(typeof(TAttribute1), typeof(TAttribute2), typeof(TAttribute3), typeof(TAttribute3));
        }

        public AttributeMatcherBuilder MarkedWith(params Type[] attributeTypes)
        {
            var builder = new AttributeMatcherBuilder {AttributeTypes = attributeTypes};
            return AddBuilder(builder);
        }

        public TMatcherBuilder AddBuilder<TMatcherBuilder>(TMatcherBuilder builder) 
            where TMatcherBuilder : IMatcherBuilder
        {
            _builders.Add(builder);
            return builder;
        }

        public IMatcher Build()
        {
            if (_builders.Count == 1)
            {
                return _builders[0].Build();
            }

            var matchers = _builders.Select(x => x.Build()).ToArray();

            return new AggregateMatcher(matchers);
        }

        public ContentMatcherBuilder ByContent(string template)
        {
            return new ContentMatcherBuilder(template);
        }
    }
}