using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PublicApiGenerator;
using VerifyNUnit;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public class SanityChecks
{
    [Test]
    public Task should_respect_verify_conventions()
        => VerifyChecks.Run();

    [Test]
    public Task should_export_expected_namespaces()
    {
        return Verifier.Verify(
            typeof(PcreRegex).Assembly
                             .ExportedTypes
                             .Select(i => i.Namespace)
                             .OrderBy(i => i)
                             .Distinct()
        );
    }

    [Test]
    public Task should_have_expected_public_api()
    {
        return Verifier.Verify(
            typeof(PcreRegex).Assembly
                             .GeneratePublicApi(
                                 new ApiGeneratorOptions
                                 {
                                     IncludeAssemblyAttributes = false,
                                     ExcludeAttributes =
                                     [
                                         typeof(ObsoleteAttribute).FullName!
                                     ]
                                 }
                             )
        ).UniqueForTargetFrameworkAndVersion();
    }
}
