using System;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Matchers;
using Newtonsoft.Json;

namespace Codestellation.DarkFlow.CastleWindsor.Impl
{
    public class WindsorPersister : PersisterBase
    {
        public WindsorPersister(IDatabase database, IKernel kernel, IMatcher matcher) : base(database, matcher)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }

            var converter = new WindsorConverter(kernel);
            Settings.Converters = new[] {converter};
        }

        private static Type ExtractType(string serialized)
        {
            const string typebegin = "\"$type\": \"";
            var startIndex = serialized.IndexOf(typebegin, StringComparison.OrdinalIgnoreCase) + typebegin.Length;
            var endIndex = serialized.IndexOf("\",", startIndex, StringComparison.OrdinalIgnoreCase);

            var typename = serialized.Substring(startIndex, endIndex - startIndex);
            var type = Type.GetType(typename);
            return type;
        }

        protected override string Serialize(ITask task)
        {
            var unproxiedTask = ProxyUtil.GetUnproxiedInstance(task);

            var serialized = JsonConvert.SerializeObject(unproxiedTask, Settings);
            return serialized;
        }


        protected override ITask Deserialize(string serialized)
        {
            var type = ExtractType(serialized);
            var result = (ITask) JsonConvert.DeserializeObject(serialized, type, Settings);
            return result;
        }
    }
}