using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Bootstrap
{
    public abstract class StaticExecutorNameBuilder : IMatcherBuilder
    {
        public string ExecutorName { get; set; }

        public abstract IMatcher ToMatcher();

        public void To(string executorName)
        {
            ExecutorName = executorName;
        }
    }
}