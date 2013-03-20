using System.Linq;
using Codestellation.DarkFlow.Config;
using Codestellation.DarkFlow.Matchers;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Config
{
    [TestFixture]
    public class ConfigurationTests
    {
        private DarkFlowConfigurationSection _section;

        [SetUp]
        public void Setup()
        {
            _section = DarkFlowConfigurationSection.GetSection();
        }
        
        [Test]
        public void Correctly_reads_dispatcher_concurrency()
        {
            var dispatcherConcurrence = _section.Dispatcher.MaxConcurrency;
            Assert.That(dispatcherConcurrence, Is.EqualTo(10));
        }

        [Test]
        public void Correctly_reads_task_queue_settings()
        {
            var queueElmement = _section.Executors.Cast<ExecutorConfigurationElement>().First<ExecutorConfigurationElement>();
            

            Assert.That(queueElmement.Name, Is.EqualTo("pipeline"));
            Assert.That(queueElmement.Priority, Is.EqualTo(3));
            Assert.That(queueElmement.MaxConcurrency, Is.EqualTo(4));
        }

        [Test]
        public void Correctly_reads_namespace_route()
        {
            var route = _section.Routing.Cast<RouteConfigurationElement>().ToList()[0];
            
            Assert.That(route.RouteTo, Is.EqualTo("pipeline"));
            Assert.That(route.Type, Is.EqualTo("namespace"));
            Assert.That(route.NamespaceMask, Is.EqualTo("Codestellation.*"));
        }

        [Test]
        public void Correctly_reads_attribute_route()
        {
            var route = _section.Routing.Cast<RouteConfigurationElement>().ToList()[1];

            Assert.That(route.RouteTo, Is.EqualTo("pipeline"));

            Assert.That(route.Type, Is.EqualTo("attribute"));
            Assert.That(route.Assembly, Is.EqualTo("Codestellation.DarkFlow"));
            Assert.That(route.Attributes, Is.EqualTo("PutItHere"));
        }
    }
}