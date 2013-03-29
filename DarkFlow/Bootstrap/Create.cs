using System;
using System.Collections.Generic;
using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Database;
using Codestellation.DarkFlow.Execution;

namespace Codestellation.DarkFlow.Bootstrap
{
    public static class Create
    {
        public static IExecutor FromXmlConfig()
        {
            var config = DarkFlowConfiguration.Instance;

            var executors = new QueuedExecutor[config.Executors.Count];

            var database = new ManagedEsentDatabase();


            var persisterMatcher = BuildMatcher(config.Persistence.Matchers);

            var persister = new Persister(database, persisterMatcher.Build());

            for (int i = 0; i < config.Executors.Count; i++)
            {
                executors[i] = new QueuedExecutor(config.Executors[i], persister);
            }

            var routerMatcher = BuildMatcher(config.Routes);

            var router = new TaskRouter(routerMatcher.Build(), executors);
            
            var dispatcher = new TaskDispatcher((byte) config.Dispatcher.MaxConcurrency, executors);

            var result = new Executor(router, dispatcher, DefaultReleaser.Instance);

            var container = new DisposableContainer(result, result, dispatcher, router, executors, persister, database);

            return container;
        }

        //note - usage of default queue name is merely hach to bring things to work without refactoting matchers. (Persister does not need to know queue name)
        public static IMatcherBuilder BuildMatcher(IEnumerable<MatcherSettings> matcherSettings, string executorName = "no-matter")
        {
            var matchers = new List<IMatcherBuilder>();
            var result = new AggregateMatcherBuilder();

            foreach (var matcherSettingse in matcherSettings)
            {
                IMatcherBuilder builder;
                var routeTo = matcherSettingse.RouteTo ?? executorName;
                switch (matcherSettingse.Type)
                {
                    case "namespace" :
                        builder = new NamespaceMatcherBuilder{Mask = matcherSettingse.Mask}.To(routeTo);
                        matchers.Add(builder);
                        break;
                    case "attribute" :

                        builder = new AttributeMatcherBuilder()
                            .Match(matcherSettingse.Attributes)
                            .FromAssembly(matcherSettingse.Assembly)
                            .To(executorName); 
                        matchers.Add(builder);
                        break;
                    default:
                        throw new ArgumentException();

                }
                result.AddBuilder(builder);
            }

            
            return result;
        }
    }
}