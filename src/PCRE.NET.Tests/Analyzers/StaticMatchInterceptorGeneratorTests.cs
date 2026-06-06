using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using PCRE.NET.Analyzers;
using VerifyNUnit;

namespace PCRE.Tests.Analyzers;

[TestFixture]
public class StaticMatchInterceptorGeneratorTests
{
    [Test]
    public Task generates_intercepts_with_literals()
    {
        return Verify(
            """
            using PCRE;

            class C
            {
                void M()
                {
                    _ = PcreRegex.Match("input", "pattern");
                    _ = PcreRegex.IsMatch("input", "pattern");
                    _ = PcreRegex.Match("input", "pattern", PcreOptions.Caseless);
                    _ = PcreRegex.IsMatch("input", "pattern", PcreOptions.Caseless);
                    _ = PcreRegex.Match("input", "pattern", PcreOptions.Caseless, 42);
                    _ = PcreRegex.IsMatch("input", "pattern", PcreOptions.Caseless, 42);

                    _ = PcreRegex.Match("input", "pattern");
                    _ = PcreRegex.Match("input", "pattern", PcreOptions.Compiled);
                    _ = PcreRegex.Match("input", "pattern2");
                }
            }
            """
        );
    }

    [Test]
    public Task does_not_generate_intercepts_with_non_literals()
    {
        return Verify(
            """
            using PCRE;

            class C
            {
                void M()
                {
                    _ = PcreRegex.Match(ToString(), ToString());
                    _ = PcreRegex.IsMatch(ToString(), ToString());
                    _ = PcreRegex.Match(ToString(), ToString(), PcreOptions.Caseless);
                    _ = PcreRegex.IsMatch(ToString(), ToString(), PcreOptions.Caseless);
                    _ = PcreRegex.Match(ToString(), ToString(), PcreOptions.Caseless, 42);
                    _ = PcreRegex.IsMatch(ToString(), ToString(), PcreOptions.Caseless, 42);
                }
            }
            """
        );
    }

    private static GeneratorDriverRunResult Generate(string input)
    {
        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        var parseOptions = CSharpParseOptions.Default
                                             .WithFeatures([new("InterceptorsNamespaces", "PCRE.Generated")]);

        var compilation = CSharpCompilation.Create("TestAssembly")
                                           .AddReferences(
                                               MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                                               MetadataReference.CreateFromFile(typeof(PcreRegex).Assembly.Location),
#if NETFRAMEWORK
                                               MetadataReference.CreateFromFile(typeof(ReadOnlySpan<>).Assembly.Location),
#elif FORCE_NET_STANDARD
                                               MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Memory.dll")),
#endif
                                               MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "netstandard.dll")),
                                               MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Runtime.dll"))
                                           )
                                           .AddSyntaxTrees(CSharpSyntaxTree.ParseText(input, parseOptions))
                                           .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithNullableContextOptions(NullableContextOptions.Enable));

        var result = CSharpGeneratorDriver.Create(
                                              generators: [new StaticMatchInterceptorGenerator().AsSourceGenerator()],
                                              parseOptions: parseOptions
                                          )
                                          .RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out _)
                                          .GetRunResult();

        var diagnostics = updatedCompilation.GetDiagnostics()
                                            .Where(i => i.Id is not "CS1702")
                                            .ToList();

        Assert.That(diagnostics, Is.Empty);

        return result;
    }

    [MustUseReturnValue]
    private static Task Verify([StringSyntax("csharp")] string input)
    {
        var result = Generate(input);
        return Verifier.Verify(result)
                       .ScrubLinesWithReplace(
                           i => Regex.Replace(
                               i,
                               """
                               (?x)
                               (?<before>InterceptsLocationAttribute\()
                               [0-9]+,[ ]
                               "[^"]+"
                               (?<after>\)\])
                               (?:\s*//.*)? # Only in DEBUG mode
                               """,
                               """${before}0, "..."${after}"""
                           )
                       );
    }
}
