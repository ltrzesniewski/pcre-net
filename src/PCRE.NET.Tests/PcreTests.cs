using System;
using NUnit.Framework;

namespace PCRE.Tests
{
    [TestFixture]
    public class PcreTests
    {
        [Test]
        public void should_report_version()
        {
            var version = Pcre.VersionString;
            Console.WriteLine(version);
            Assert.IsNotNullOrEmpty(version);
        }
    }
}
