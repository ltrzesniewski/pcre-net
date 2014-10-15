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
            var value = PcreRegex.BuildInfo.VersionString;
            Console.WriteLine(value);
            Assert.IsNotNullOrEmpty(value);
        }

        [Test]
        public void should_report_jit_target()
        {
            var value = PcreRegex.BuildInfo.JitTarget;
            Console.WriteLine(value);
            Assert.IsNotNullOrEmpty(value);
        }

        [Test]
        public void should_report_all_config_info()
        {
            var info = PcreRegex.BuildInfo;

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
