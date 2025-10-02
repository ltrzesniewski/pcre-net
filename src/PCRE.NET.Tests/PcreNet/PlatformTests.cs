using System;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public class PlatformTests
{
    [Test]
    public void print_current_platform()
    {
        Console.WriteLine("TESTS RUNNING IN {0}-bit mode", Environment.Is64BitProcess ? 64 : 32);
        Console.WriteLine("ARCHITECTURE: {0}", RuntimeInformation.ProcessArchitecture);
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

        Assert.That(
            namespaces,
            Is.EqualTo(
                [
                    "PCRE",
                    "PCRE.Conversion",
                    "PCRE.Dfa"
                ]
            )
        );
    }

#if EXPECT_X86
    [Test]
    public void validate_platform_x86()
    {
        Assert.That(RuntimeInformation.ProcessArchitecture, Is.EqualTo(Architecture.X86));
    }
#endif

#if EXPECT_X64
    [Test]
    public void validate_platform_x64()
    {
        Assert.That(RuntimeInformation.ProcessArchitecture, Is.EqualTo(Architecture.X64));
    }
#endif

#if EXPECT_ARM64
    [Test]
    public void validate_platform_arm64()
    {
        Assert.That(RuntimeInformation.ProcessArchitecture, Is.EqualTo(Architecture.Arm64));
    }
#endif
}
