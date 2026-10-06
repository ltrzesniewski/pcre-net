using System;
using System.Linq;
using NUnit.Framework;
using PCRE.Tests.Support;
using PublicApiGenerator;
using Shouldly;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public class SanityChecks
{
    [Test]
    public void should_export_expected_namespaces()
    {
        typeof(PcreRegex).Assembly
                         .ExportedTypes
                         .Select(i => i.Namespace)
                         .Distinct()
                         .ShouldBe([
                             "PCRE",
                             "PCRE.Conversion",
                             "PCRE.Dfa"
                         ], ignoreOrder: true);
    }

    [Test]
    public void should_have_expected_public_api()
    {
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
                         .ShouldMatchApproved(
                             cfg => cfg.WithFileExtension(".cs")
                                       .WithDiscriminator(typeof(SanityChecks).Assembly.GetMetadataValue("TargetFramework") ?? throw new InvalidOperationException("TargetFramework metadata not found"))
                         );
    }
}
