using System.Configuration;

namespace Codestellation.DarkFlow.Config
{
    public class DispatcherElement : ConfigurationElement
    {
        [ConfigurationProperty("maxConcurrency")]
        //[IntegerValidator(MinValue = 1, MaxValue = byte.MaxValue)]
        public int MaxConcurrency
        {
            get { return (int) this["maxConcurrency"]; }
            set { this["maxConcurrency"] = value; }
        }
    }
}