using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Bootstrap
{
    public class NamespaceMatcherBuilder : StaticExecutorNameBuilder
    {
        public override IMatcher Build()
        {
            return new NamespaceMatcher(ExecutorName, Mask);
        }

        public string Mask { get; set; }
    }
}