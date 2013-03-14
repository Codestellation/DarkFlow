using Codestellation.DarkFlow.Matchers;
using Codestellation.DarkFlow.Tests.Core.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Matchers
{
    [TestFixture]
    public class NamespaceMatcherTests
    {
        private PersistentTask _task;

        [SetUp]
        public void Setup()
        {
            _task = new PersistentTask(1);
        }

        [TestCase(NamespaceMatcher.All)]
        [TestCase("Codestellation.DarkFlow.Tests.Core.Execution.*")]
        [TestCase("Codestellation.DarkFlow.Tests.Core.Execution.PersistentTask")]
        public void Matches_tasks_by_filter(string namespaceFilter)
        {
            var matcher = new NamespaceMatcher(namespaceFilter, "test");

            Assert.That(matcher.Match(_task), Is.True);
        }

        [Test]
        public void Does_not_match_tasks_if_namespace_incorrect()
        {
            var matcher = new NamespaceMatcher("No.Such.Tasks", "test");
            Assert.That(matcher.Match(_task), Is.False);
        }
    }
}