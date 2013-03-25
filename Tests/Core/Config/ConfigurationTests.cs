using Codestellation.DarkFlow.Config;
using NUnit.Framework;
using SimpleConfig;

namespace Codestellation.DarkFlow.Tests.Core.Config
{
    [TestFixture]
    public class ConfigurationTests
    {
        private DarkFlowConfiguration _section;

        [SetUp]
        public void Setup()
        {
            _section = Configuration.Load<DarkFlowConfiguration>("darkFlow");
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
            var executorSettings = _section.Executors[0];

            Assert.That(executorSettings.Name, Is.EqualTo("pipeline"));
            Assert.That(executorSettings.Priority, Is.EqualTo(3));
            Assert.That(executorSettings.MaxConcurrency, Is.EqualTo(4));
        }

        [Test]
        public void Correctly_reads_persistence_settings()
        {
            var persistence = _section.Persistence;

            Assert.That(persistence, Is.Not.Null);
        }

        [Test]
        public void Correctly_reads_namespace_route()
        {
            var route = _section.Routes[0];

            Assert.That(route.Type, Is.EqualTo("namespace"));
            Assert.That(route.RouteTo, Is.EqualTo("pipeline"));
            Assert.That(route.Mask, Is.EqualTo("Codestellation.*"));
        }

        [Test]
        public void Correctly_reads_attribute_route()
        {
            var route = _section.Routes[1];

            Assert.That(route.RouteTo, Is.EqualTo("pipeline"));

            Assert.That(route.Type, Is.EqualTo("attribute"));
            Assert.That(route.Assembly, Is.EqualTo("Codestellation.DarkFlow"));
            Assert.That(route.Attributes, Is.EqualTo("PutItHere"));
        }
    }
}