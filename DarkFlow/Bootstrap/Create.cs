using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;
using Codestellation.DarkFlow.Matchers;
using SimpleConfig;

namespace Codestellation.DarkFlow.Bootstrap
{
    public static class Create
    {
        public static IExecutor FromXmlConfig()
        {
            var config = Configuration.Load<DarkFlowConfiguration>("darkFlow");

            var executors = new QueuedExecutor[config.Executors.Count];

            var database = new ManagedEsentDatabase();


            var persisterMatcher = BuildMatcher(config.Persistence.Matchers);

            var persister = new Persister(database, persisterMatcher);

            for (int i = 0; i < config.Executors.Count; i++)
            {
                executors[i] = new QueuedExecutor(config.Executors[i], persister);
            }

            var routerMatcher = BuildMatcher(config.Routes);

            var router = new TaskRouter(routerMatcher, executors);
            
            var dispatcher = new TaskDispatcher((byte) config.Dispatcher.MaxConcurrency, executors);

            var result = new Executor(router, dispatcher, DefaultReleaser.Instance);

            var container = new DisposableContainer(result, result, dispatcher, router, executors, persister, database);

            return container;
        }

        //note - usage of default queue name is merely hach to bring things to work without refactoting matchers. (Persister does not need to know queue name)
        private static IMatcher BuildMatcher(List<MatcherSettings> matcherSettings, string executorName = "no-matter")
        {
            var matchers = new List<IMatcher>();

            foreach (var matcherSettingse in matcherSettings)
            {
                IMatcher matcher;
                switch (matcherSettingse.Type)
                {
                    case "namespace" :
                        matcher = new NamespaceMatcher(matcherSettingse.RouteTo ?? executorName, matcherSettingse.Mask);
                        matchers.Add(matcher);
                        break;
                    case "attribute" :
                        var assembly = Assembly.Load(matcherSettingse.Assembly);
                        var attributeNames = matcherSettingse.Attributes.Split(',').Select(x => x.Trim()).ToArray();

                        var attributeTypes = assembly.GetTypes()
                                                    .Where(
                                                        x =>
                                                        attributeNames.Any(
                                                            attrib =>
                                                            attrib.Equals(x.Name,
                                                                          StringComparison.InvariantCultureIgnoreCase)))
                                                    .ToArray();

                        matcher = new AttributeMatcher(executorName, attributeTypes);
                        matchers.Add(matcher);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            var result = new AggregateMatcher(matchers.ToArray());
            return result;
        }
    }
}