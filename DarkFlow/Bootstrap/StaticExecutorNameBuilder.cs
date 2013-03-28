using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Bootstrap
{
    public abstract class StaticExecutorNameBuilder : IIMatcherBuilder
    {
        public string ExecutorName { get; set; }

        public abstract IMatcher ToMatcher();

        public void To(string executorName)
        {
            ExecutorName = executorName;
        }
    }
}