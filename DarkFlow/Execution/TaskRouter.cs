using System;
using System.Collections.Generic;
using System.Linq;
using Codestellation.DarkFlow.Matchers;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Execution
{
    
    //TODO This class should take care of pushing tasks to queues, or die otherwise.
    public class TaskRouter : ITaskRouter
    {
        private readonly IMatcher _matcher;

        public TaskRouter(IMatcher matcher)
        {
            Contract.Require(matcher != null, "matcher != null");
            _matcher = matcher;
        }

        public string ResolveQueueFor(ITask task)
        {
            Contract.Require(task != null, "task != null");

            //TODO: Caching required. 

            var result = _matcher.TryMatch(task);

            if (result)
            {
                return result.Value;
            }

            throw new InvalidOperationException(string.Format("Task {0} does not match any queue", task));
        }
    }
}