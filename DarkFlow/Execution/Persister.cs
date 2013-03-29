using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Matchers;
using Newtonsoft.Json;

namespace Codestellation.DarkFlow.Execution
{
    public class Persister : PersisterBase
    {
        public Persister(IDatabase database, IMatcher matcher) : base(database, matcher)
        {
        }

        protected override string Serialize(ITask task)
        {
            var serialized = JsonConvert.SerializeObject(task, Settings);

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Task {0} serialized to {1}", task, serialized);
            }

            return serialized;
        }

        protected override ITask Deserialize(string serialized)
        {
            var deserialized = JsonConvert.DeserializeObject(serialized, Settings);

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Task {0} deserialized from {1}", deserialized, serialized);
            }

            return (ITask)deserialized;
        }
    }
}