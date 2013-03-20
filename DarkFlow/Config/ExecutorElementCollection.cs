using System.Configuration;

namespace Codestellation.DarkFlow.Config
{
    [ConfigurationCollection(typeof(ExecutorConfigurationElement), AddItemName = "executor")]
    public class ExecutorElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExecutorConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var queueConfigurationElement = (ExecutorConfigurationElement) element;
            
            return queueConfigurationElement.Name;
        }
    }
}