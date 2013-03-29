using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Bootstrap
{
    public abstract class StaticExecutorNameBuilder : IMatcherBuilder
    {
        public string ExecutorName { get; set; }

        public abstract IMatcher Build();

        public StaticExecutorNameBuilder To(string executorName)
        {
            ExecutorName = executorName;
            return this;
        }
    }
}