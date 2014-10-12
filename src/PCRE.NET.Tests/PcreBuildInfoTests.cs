using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace PCRE.Tests
{
    [TestFixture]
    public class PcreBuildInfoTests
    {
        [Test]
        public void should_report_version()
        {
            var value = Pcre.BuildInfo.VersionString;
            Console.WriteLine(value);
            Assert.IsNotNullOrEmpty(value);
        }

        [Test]
        public void should_report_jit_target()
        {
            var value = Pcre.BuildInfo.JitTarget;
            Console.WriteLine(value);
            Assert.IsNotNullOrEmpty(value);
        }

        [Test]
        public void should_report_all_config_info()
        {
            var info = Pcre.BuildInfo;

            var properties = info.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.CanRead);

            foreach (var propertyInfo in properties)
            {
                var value = propertyInfo.GetValue(info);
                Console.WriteLine("{0} = {1}", propertyInfo.Name, value);
            }
        }
    }
}
