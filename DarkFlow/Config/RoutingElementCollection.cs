using System;
using System.Configuration;

namespace Codestellation.DarkFlow.Config
{
    [ConfigurationCollection(typeof(RouteConfigurationElement), AddItemName = "route")]
    public class RoutingElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RouteConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            //note: this is merely hack. I don't need keys for this collection;
            return Guid.NewGuid();
        }
    }
}