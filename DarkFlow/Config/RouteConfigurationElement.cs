using System.Configuration;
using Codestellation.DarkFlow.Matchers;

namespace Codestellation.DarkFlow.Config
{
    public class RouteConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("routeTo", IsRequired = true)]
        public string RouteTo
        {
            get { return (string) this["routeTo"]; }
            set { this["routeTo"] = value; } 
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("namespaceMask")]
        public string NamespaceMask
        {
            get { return (string)this["namespaceMask"]; }
            set { this["namespaceMask"] = value; }
        }

        [ConfigurationProperty("assembly")]
        public string Assembly
        {
            get { return (string)this["assembly"]; }
            set { this["assembly"] = value; }
        }

        [ConfigurationProperty("attributes")]
        public string Attributes
        {
            get { return (string)this["attributes"]; }
            set { this["attributes"] = value; }
        }
    }
}