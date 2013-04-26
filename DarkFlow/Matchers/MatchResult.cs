using System;

namespace Codestellation.DarkFlow.Matchers
{
    public class MatchResult
    {
        public static readonly MatchResult NonMatched = new MatchResult();
        
        private readonly string _value;
        private readonly bool _matched;

        private MatchResult()
        {
            _matched = false;
        }

        private MatchResult(string value)
        {
            _matched = true;
            _value = value;
        }

        public bool Matched
        {
            get { return _matched; }
        }

        public string Value
        {
            get
            {
                if (_matched)
                {
                    return _value;
                }

                throw new InvalidOperationException("Could not get value on non-matched result.");
            }
        }

        public static implicit operator bool(MatchResult instance)
        {
            return instance._matched;
        }

        public static MatchResult Matches(string value)
        {
            return new MatchResult(value);
        }
    }
}