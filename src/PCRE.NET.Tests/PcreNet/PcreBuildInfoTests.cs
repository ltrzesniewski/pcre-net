using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    public class PcreBuildInfoTests
    {
        [Test]
        public void should_report_version()
        {
            var value = PcreBuildInfo.Version;
            Console.WriteLine(value);
            Assert.That(value, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void should_report_jit_target()
        {
            var value = PcreBuildInfo.JitTarget;
            Console.WriteLine(value);
            Assert.That(value, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void should_report_all_config_info()
        {
            var properties = typeof(PcreBuildInfo)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(prop => prop.CanRead);

            foreach (var propertyInfo in properties)
            {
                var value = propertyInfo.GetValue(null);
                Console.WriteLine("{0} = {1}", propertyInfo.Name, value);
            }
        }
    }
}
