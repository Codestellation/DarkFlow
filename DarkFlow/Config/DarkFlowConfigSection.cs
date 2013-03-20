using System.Configuration;

namespace Codestellation.DarkFlow.Config
{
    //TODO: Check that queues has at least one route and a route has valid queue
    public class DarkFlowConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("dispatcher")]
        public DispatcherElement Dispatcher
        {
            get { return (DispatcherElement) this["dispatcher"]; }
            set { this["dispatcher"] = value; }
        }

        [ConfigurationProperty("executors")]
        public ExecutorElementCollection Executors
        {
            get { return (ExecutorElementCollection) this["executors"]; }
            set { this["executors"] = value; }
        }

        [ConfigurationProperty("routing")]
        public RoutingElementCollection Routing
        {
            get { return (RoutingElementCollection) this["routing"]; }
            set { this["routing"] = value; }
        }

        public static DarkFlowConfigurationSection GetSection()
        {
            return (DarkFlowConfigurationSection) ConfigurationManager.GetSection("darkFlow");
        }
    }
}