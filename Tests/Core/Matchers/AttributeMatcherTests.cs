using System;
using Codestellation.DarkFlow.Matchers;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Matchers
{
    [TestFixture]
    public class AttributeMatcherTests
    {
        private class TaskMarkAttribute : Attribute { }
        
        [TaskMark]
        private class MarkedTask : ITask
        {
            public void Execute() { }
        }

        
        private class NotMarkedTask : ITask
        {
            public void Execute() { }
        }

        
        [Test]
        public void Should_match_if_marked_with_attribute()
        {
            var matcher = new AttributeMatcher("test", typeof(TaskMarkAttribute));
            var task = new MarkedTask();
            var result = matcher.TryMatch(task);
            Assert.That(result.Matched, Is.True);
            Assert.That(result.Value, Is.EqualTo("test"));
        }

        [Test]
        public void Should_not_match_if_was_not_marked_with_attribute()
        {
            var matcher = new AttributeMatcher("test", typeof(TaskMarkAttribute));
            var task = new NotMarkedTask();
            var result = matcher.TryMatch(task);
            
            Assert.That(result.Matched, Is.False);
            Assert.Throws<InvalidOperationException>(delegate { var noMatter = result.Value;});
        }
    }
}