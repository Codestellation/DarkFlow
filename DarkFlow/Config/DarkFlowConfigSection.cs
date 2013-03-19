using System;
using System.Configuration;
using System.Xml;

namespace Codestellation.DarkFlow.Config
{
    //TODO: Check that queues has at least one route and a route has valid queue
    public class DarkFlowConfigurationSection : ConfigurationSection
    {
        public static DarkFlowConfigurationSection GetSection()
        {
            return (DarkFlowConfigurationSection)ConfigurationManager.GetSection("darkFlow");
        }

        [ConfigurationProperty("dispatcher")]
        public DispatcherElement Dispatcher
        {
            get { return (DispatcherElement) this["dispatcher"]; }
            set { this["dispatcher"] = value; }
        }

        [ConfigurationProperty("queues")]
        public QueueElementCollection Queues
        {
            get { return (QueueElementCollection) this["queues"]; }
            set { this["queues"] = value; }
        }

        [ConfigurationProperty("routing")]
        public RoutingElementCollection Routing
        {
            get { return (RoutingElementCollection)this["routing"]; }
            set { this["routing"] = value; }
        }
    }
}