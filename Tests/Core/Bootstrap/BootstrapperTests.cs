using System;
using Codestellation.DarkFlow.Bootstrap;
using NUnit.Framework;

namespace Codestellation.DarkFlow.Tests.Core.Bootstrap
{
    [TestFixture]
    public class BootstrapperTests
    {
        [Test]
        public void Rude_test_for_reading_config()
        {
            var executor = Create.FromXmlConfig();

            Assert.That(executor, Is.Not.Null);

            ((IDisposable)executor).Dispose();
        }
    }
}