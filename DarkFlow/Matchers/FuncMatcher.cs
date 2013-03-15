using System;
using System.Linq;

namespace Codestellation.DarkFlow.Matchers
{
    public class FuncMatcher : AbstractMatcher
    {
        private readonly Func<ITask, bool>[] _matchingFuncs;

        public FuncMatcher(string queueName, params Func<ITask, bool>[] matchingFuncs) : base(queueName)
        {
            if (matchingFuncs == null)
            {
                throw new ArgumentNullException("matchingFuncs");
            }
            
            _matchingFuncs = matchingFuncs;
        }

        protected override bool Match(ITask task)
        {
            return _matchingFuncs.Select(matchingFunc => matchingFunc(task)).Any(matched => matched);
        }
    }
}