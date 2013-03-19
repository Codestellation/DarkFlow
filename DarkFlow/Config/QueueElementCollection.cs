using System.Configuration;

namespace Codestellation.DarkFlow.Config
{
    [ConfigurationCollection(typeof(QueueConfigurationElement), AddItemName = "queue")]
    public class QueueElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new QueueConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var queueConfigurationElement = (QueueConfigurationElement) element;
            
            return queueConfigurationElement.Name;
        }
    }
}