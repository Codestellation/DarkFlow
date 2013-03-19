using System.Configuration;

namespace Codestellation.DarkFlow.Config
{
    public class QueueConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }
        
        [ConfigurationProperty("priority", IsRequired = true)]
        public int Priority
        {
            get { return (int) this["priority"]; }
            set { this["priority"] = value; }
        }

        [ConfigurationProperty("maxConcurrency", IsRequired = true)]
        public int MaxConcurrency
        {
            get { return (int) this["maxConcurrency"]; }
            set { this["maxConcurrency"] = value; }
        }
    }
}