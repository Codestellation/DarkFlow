using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Bootstrap
{
    public interface IIMatcherBuilder
    {
        IMatcher ToMatcher();
    }
}