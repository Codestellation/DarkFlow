using Codestellation.DarkFlow.Matchers;
using Codestellation.DarkFlow.Tests.Core.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Matchers
{
    [TestFixture]
    public class NamespaceMatcherTests
    {
        [TestCase(NamespaceMatcher.AnyWildCard)]
        [TestCase("Codestellation.DarkFlow.Tests.Core.Execution.*")]
        [TestCase("Codestellation.DarkFlow.Tests.Core.Execution.PersistentTask")]
        public void Matches_tasks_by_filter(string namespaceFilter)
        {
            var matcher = new NamespaceMatcher("test", namespaceFilter);

            var matchResult = matcher.TryMatch(PersistentTask.Instance);

            Assert.That(matchResult.Matched, Is.True);
        }

        [Test]
        public void Does_not_match_tasks_if_namespace_incorrect()
        {
            var matcher = new NamespaceMatcher("test", "No.Such.Tasks");
            var matchResult = matcher.TryMatch(PersistentTask.Instance);
            
            Assert.That(matchResult.Matched, Is.False);
        }
    }
}