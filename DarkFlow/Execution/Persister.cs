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
            return serialized;
        }

        protected override ITask Deserialize(string serialized)
        {
            var deserialized = JsonConvert.DeserializeObject(serialized, Settings);
            return (ITask) deserialized;
        }
    }
}