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
            var builder = new ExecutorBuilder();

            var config = DarkFlowConfiguration.Instance;

            foreach (var settings in config.Executors)
            {
                builder.WithQueuedExecutors(settings);
            }

            if (config.Persistence.Matchers.Count > 0)
            {
                var persisterMatcherBuilder = BuildMatcher(config.Persistence.Matchers);
                
                builder
                    .PersistTasks(mb => mb.AddBuilder(persisterMatcherBuilder))
                    .UseManagedEsentDatabase();
            }


            var routerMatcherBuilder = BuildMatcher(config.Routes);

            builder
                .MaxConcurrency(config.Dispatcher.MaxConcurrency)
                .RouteTasks(mb => mb.AddBuilder(routerMatcherBuilder));

            return builder.Build();
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

        public static ExecutorBuilder Executor
        {
            get {return new ExecutorBuilder();}
        }

        
    }
}