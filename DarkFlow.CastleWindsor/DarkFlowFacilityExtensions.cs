using Codestellation.DarkFlow.Bootstrap;
using Codestellation.DarkFlow.Config;

namespace Codestellation.DarkFlow.CastleWindsor
{
    public static class DarkFlowFacilityExtensions
    {
        public static DarkFlowFacility ConfigureFromXml(this DarkFlowFacility self)
        {
            var routeMatcherBuilder = Create.BuildMatcher(DarkFlowConfiguration.Instance.Routes);
            var persisterMatcherBuilder = Create.BuildMatcher(DarkFlowConfiguration.Instance.Persistence.Matchers);

            self
                .MaxConcurrency((byte) DarkFlowConfiguration.Instance.Dispatcher.MaxConcurrency)
                .WithQueuedExecutor(DarkFlowConfiguration.Instance.Executors.ToArray())
                .RouteTasks(routeMatcherBuilder)
                .PersistTasks(persisterMatcherBuilder);

            return self;
        }
    }
}