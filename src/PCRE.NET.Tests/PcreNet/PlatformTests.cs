using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using PCRE.Internal;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    public class PlatformTests
    {
        [Test]
        public void print_current_platform()
        {
            Console.WriteLine("TESTS RUNNING IN {0}-bit mode", Environment.Is64BitProcess ? 64 : 32);
        }

        [Test, Explicit]
        public void print_asmz_hashes()
        {
            foreach (var prefix in new[] { "", "x86:", "x64:" })
            {
                var str = prefix + "PCRE.NET.Wrapper, Version=0.0.0.0, Culture=neutral, PublicKeyToken=8f58d558eeff25a3";
                var hash = new Guid(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str.ToLowerInvariant())));

                Console.WriteLine("{0:N} = {1}", hash, str);
            }
        }

        [Test]
        public void should_not_expose_internal_namespace()
        {
            foreach (var type in typeof(PcreRegex).Assembly.GetExportedTypes())
            {
                Assert.That(type.Namespace, Does.Not.Contain("Internal"));
            }
        }
    }
}
