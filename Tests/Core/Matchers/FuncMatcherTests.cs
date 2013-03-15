using Codestellation.DarkFlow.Matchers;
using Codestellation.DarkFlow.Tests.Core.Execution;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Matchers
{
    [TestFixture]
    public class FuncMatcherTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void Should_return_result_of_func(bool funcResult)
        {
            var matcher = new FuncMatcher("test", type => funcResult);

            var matchResult = matcher.TryMatch(PersistentTask.Instance);

            Assert.That(matchResult.Matched, Is.EqualTo(funcResult));
        }
    }
}