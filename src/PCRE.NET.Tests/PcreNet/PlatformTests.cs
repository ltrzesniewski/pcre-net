using System;
using System.Linq;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public class PlatformTests
{
    [Test]
    public void print_current_platform()
    {
        Console.WriteLine("TESTS RUNNING IN {0}-bit mode", Environment.Is64BitProcess ? 64 : 32);
    }

    [Test]
    public void should_not_expose_internal_namespace()
    {
        var namespaces = typeof(PcreRegex).Assembly
                                          .GetExportedTypes()
                                          .Select(i => i.Namespace)
                                          .Distinct(StringComparer.Ordinal)
                                          .OrderBy(i => i, StringComparer.Ordinal)
                                          .ToList();

        Assert.That(namespaces, Is.EqualTo(new[]
        {
            "PCRE",
            "PCRE.Conversion",
            "PCRE.Dfa"
        }));
    }

#if EXPECT_X86
    [Test]
    public void validate_platform_x86()
    {
        Assert.That(System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture, Is.EqualTo(System.Runtime.InteropServices.Architecture.X86));
    }
#endif

#if EXPECT_X64
    [Test]
    public void validate_platform_x64()
    {
        Assert.That(System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture, Is.EqualTo(System.Runtime.InteropServices.Architecture.X64));
    }
#endif
}
