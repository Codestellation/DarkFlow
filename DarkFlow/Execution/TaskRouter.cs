using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.DarkFlow.Matchers;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Execution
{
    public class TaskRouter : ITaskRouter
    {
        private readonly IEnumerable<IMatcher> _matchers;
        private readonly Dictionary<Type, string> _matchedCache;

        public TaskRouter(IEnumerable<IMatcher> matchers)
        {
            _matchers = matchers;
            _matchedCache = new Dictionary<Type, string>();
            Contract.Require(matchers != null, "matchers != null");
        }

        public string ResolveQueueFor(ITask task)
        {
            Contract.Require(task != null, "task != null");

            //TODO: Caching required. 

            var type = task.GetType();

            var result = _matchedCache.GetOrAddThreadSafe(type, () => InternalResolveQueueFor(task));

            return result;
        }

        private string InternalResolveQueueFor(ITask task)
        {
            var matches = _matchers.Where(x => x.Match(task));

            if (matches.Count() > 1)
            {
                throw new InvalidOperationException(string.Format("Task {0} matches more than one queue.", task));
            }

            if (!matches.Any())
            {
                throw new InvalidOperationException(string.Format("Task {0} does not match any queue", task));
            }

            return matches.Single().QueueName;
        }
    }
}