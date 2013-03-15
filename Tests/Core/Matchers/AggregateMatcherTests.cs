using Codestellation.DarkFlow.Matchers;
using Codestellation.DarkFlow.Tests.Core.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Matchers
{
    [TestFixture]
    public class AggregateMatcherTests
    {
        [Test]
        public void Should_match_and_return_results_from_different_matchers()
        {
            var nsMatcher = new NamespaceMatcher("byNamespace", typeof (PersistentTask).Namespace);
            var funcMatcher = new FuncMatcher("byFunc", task => task.GetType() == typeof (LongRunningTask));

            var matcher = new AggregateMatcher(nsMatcher, funcMatcher);

            var namespaceMatch = matcher.TryMatch(PersistentTask.Instance);

            Assert.That(namespaceMatch.Matched, Is.True);
            Assert.That(namespaceMatch.Value, Is.EqualTo("byNamespace"));

            var anotherTask = new LongRunningTask(false);
            var funcMatch = matcher.TryMatch(anotherTask);

            Assert.That(funcMatch.Matched, Is.True);
            Assert.That(funcMatch.Value, Is.EqualTo("byFunc"));
        }
    }
}