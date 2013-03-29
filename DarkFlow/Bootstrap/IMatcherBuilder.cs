using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Bootstrap
{
    public interface IMatcherBuilder
    {
        IMatcher ToMatcher();
    }
}