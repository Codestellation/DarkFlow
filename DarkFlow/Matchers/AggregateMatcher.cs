namespace Codestellation.DarkFlow.Matchers
{
    public class AggregateMatcher : IMatcher
    {
        private readonly IMatcher[] _matchers;

        public AggregateMatcher(params IMatcher[] matchers)
        {
            _matchers = matchers;
        }

        public MatchResult TryMatch(ITask task)
        {
            for (int i = 0; i < _matchers.Length; i++)
            {
                var matcher = _matchers[i];
            
                var result = matcher.TryMatch(task);

                if (result) return result;
            }
            return MatchResult.NonMatched;
        }
    }
}