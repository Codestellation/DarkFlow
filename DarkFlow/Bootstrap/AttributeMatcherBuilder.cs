using System;
using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class AttributeMatcherBuilder : StaticExecutorNameBuilder
    {
        public override IMatcher ToMatcher()
        {
            return new AttributeMatcher(ExecutorName, AttributeTypes);
        }

        public Type[] AttributeTypes { get; set; }
    }
}