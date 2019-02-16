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
        public void should_not_expose_wrapper_in_public_api()
        {
            var wrapperAssembly = typeof(InternalRegex).Assembly;

            foreach (var type in typeof(PcreRegex).Assembly.GetExportedTypes())
            {
                if (type.BaseType != null)
                    Assert.That(type.BaseType.Assembly, Is.Not.EqualTo(wrapperAssembly));

                foreach (var method in type.GetMethods().Where(m => m.IsPublic || m.IsFamily))
                {
                    Assert.That(method.ReturnType.Assembly, Is.Not.EqualTo(wrapperAssembly));

                    foreach (var param in method.GetParameters())
                        Assert.That(param.ParameterType.Assembly, Is.Not.EqualTo(wrapperAssembly));
                }
            }
        }
    }
}
