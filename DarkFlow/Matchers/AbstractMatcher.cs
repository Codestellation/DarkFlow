using System;

namespace Codestellation.DarkFlow.Matchers
{
    public abstract class AbstractMatcher : IMatcher
    {
        private readonly MatchResult _matchResult;

        protected AbstractMatcher(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("Should be not empty not null string.", "queueName");
            }
            
            _matchResult = MatchResult.Matches(queueName);
        }

        protected abstract bool Match(ITask task);

        public MatchResult TryMatch(ITask task)
        {
            if (Match(task))
            {
                return _matchResult;
            }
            return MatchResult.NonMatched;
        }
    }
}