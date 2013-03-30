using System;
using NLog;

namespace Codestellation.DarkFlow.Matchers
{
    public abstract class AbstractMatcher : IMatcher
    {
        private readonly MatchResult _matchResult;
        private readonly Logger Logger;

        protected AbstractMatcher(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("Should be not empty not null string.", "queueName");
            }
            Logger = LogManager.GetLogger(GetType().FullName);
            _matchResult = MatchResult.Matches(queueName);
        }

        protected abstract bool Match(ITask task);

        public MatchResult TryMatch(ITask task)
        {
            if (Match(task))
            {
                Logger.Debug("Task {0} matched to {1}.", task, _matchResult.Value);
                return _matchResult;
            }
            Logger.Debug("Task {0} not matched.", task);
            return MatchResult.NonMatched;
        }
    }
}