using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class ContentMatcherBuilder : IMatcherBuilder
    {
        private readonly string _template;
        private bool _cacheEnvironments;

        public ContentMatcherBuilder(string template)
        {
            _template = template;
        }

        public IMatcher Build()
        {
            return new ContentMatcher(_template, _cacheEnvironments);
        }

        public ContentMatcherBuilder CacheEnvironments()
        {
            _cacheEnvironments = true;
            return this;
        }
    }
}