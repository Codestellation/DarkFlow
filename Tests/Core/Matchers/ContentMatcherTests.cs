using System;
using Codestellation.DarkFlow.Matchers;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Matchers
{
    [TestFixture]
    public class ContentMatcherTests
    {
        private class ContentTestTask : ITask
        {
            public int Count;

            public string Name { get; set; }

            public void Execute()
            {
                
            }
        }
        
        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable("envvar", null);

        }

        [Test]
        public void Should_build_corrent_string()
        {
            var task = new ContentTestTask {Name = "Test" , Count = 7 };

            Environment.SetEnvironmentVariable("envvar", "Ok");

            var matcher = new ContentMatcher("executor-{Name}-{Count}-{env:enVvaR}");

            var result = matcher.TryMatch(task);

            Assert.That(result.Value, Is.EqualTo("executor-Test-7-Ok"));
        }
    }
}