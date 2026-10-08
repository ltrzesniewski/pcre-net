using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Shouldly;

namespace PCRE.Tests.Analyzers;

public abstract class BaseInterceptorTests<TGenerator>
    where TGenerator : IIncrementalGenerator, new()
{
    protected static GeneratorDriverRunResult Generate(string input)
    {
        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        var parseOptions = CSharpParseOptions.Default
                                             .WithLanguageVersion(LanguageVersion.CSharp11)
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
                                              generators: [new TGenerator().AsSourceGenerator()],
                                              parseOptions: parseOptions
                                          )
                                          .RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out _)
                                          .GetRunResult();

        var diagnostics = updatedCompilation.GetDiagnostics()
                                            .Where(i => i.Id is not "CS1702")
                                            .ToList();

#if DEBUG
        if (diagnostics.Any())
            Console.WriteLine(result.GeneratedTrees.FirstOrDefault()?.GetText());
#endif


        diagnostics.ShouldBeEmpty();

        return result;
    }

    protected static void VerifyNone([StringSyntax("csharp")] string input)
    {
        var result = Generate(input);
        result.GeneratedTrees.ShouldBeEmpty();
    }

    protected static void Verify([StringSyntax("csharp")] string input)
        => Verify(Generate(input));

    protected static void Verify([StringSyntax("csharp")] GeneratorDriverRunResult result)
    {
        var tree = result.GeneratedTrees.ShouldHaveSingleItem();

        tree.ToString().ShouldMatchApproved(
            cfg => cfg.WithScrubber(Scrub)
                      .WithFileExtension(".cs")
                      .LocateTestMethodUsingAttribute<TestAttribute>()
        );
    }

    private static string Scrub(string input)
    {
        return Regex.Replace(
            input,
            """
            (?x)
            (?<before>InterceptsLocationAttribute\()
            [0-9]+,[ ]
            "[^"]+"
            (?<after>\)\])
            (?:\s*//.*)? # Only in DEBUG mode
            """,
            """${before}0, "..."${after}"""
        );
    }
}
