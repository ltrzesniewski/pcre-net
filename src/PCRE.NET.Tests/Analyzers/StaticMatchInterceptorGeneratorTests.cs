using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
                    _ = PcreRegex.Match("subject", "pattern");
                    _ = PcreRegex.IsMatch("subject", "pattern");
                    _ = PcreRegex.Match("subject", "pattern", PcreOptions.Caseless);
                    _ = PcreRegex.IsMatch("subject", "pattern", PcreOptions.Caseless);
                    _ = PcreRegex.Match("subject", "pattern", PcreOptions.Caseless, 42);
                    _ = PcreRegex.IsMatch("subject", "pattern", PcreOptions.Caseless, 42);

                    _ = PcreRegex.Match("subject", "pattern");
                    _ = PcreRegex.Match("subject", "pattern", PcreOptions.Compiled);
                    _ = PcreRegex.Match("subject", "pattern2");
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
                    _ = PcreRegex.Match(GetString(), GetString());
                    _ = PcreRegex.IsMatch(GetString(), GetString());
                    _ = PcreRegex.Match(GetString(), GetString(), PcreOptions.Caseless);
                    _ = PcreRegex.IsMatch(GetString(), GetString(), PcreOptions.Caseless);
                    _ = PcreRegex.Match(GetString(), GetString(), PcreOptions.Caseless, 42);
                    _ = PcreRegex.IsMatch(GetString(), GetString(), PcreOptions.Caseless, 42);
                }

                static string GetString() => "foo";
            }
            """
        );
    }

    [Test]
    public async Task covers_full_api()
    {
        var methods = typeof(PcreRegex).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                       .Where(static m => m.GetParameters().Any(static i => i.Name is "pattern"))
                                       .OrderBy(static m => $"{m.Name} {string.Join(", ", m.GetParameters().Select(static p => $"{p.Name} {p.ParameterType.Name}"))}")
                                       .ToList();

        var sb = new StringBuilder();
        sb.AppendLine(
            """
            using PCRE;

            class C
            {
                void M()
                {
            """
        );

        foreach (var method in methods)
        {
            sb.Append("        _ = PcreRegex.").Append(method.Name).Append('(');

            foreach (var parameter in method.GetParameters())
            {
                var value = parameter.ParameterType switch
                {
                    var t when t == typeof(string)                  => $"\"{parameter.Name}\"",
                    var t when t == typeof(int)                     => "42",
                    var t when t == typeof(PcreOptions)             => "PcreOptions.Caseless",
                    var t when t == typeof(PcreSplitOptions)        => "PcreSplitOptions.None",
                    var t when t == typeof(PcreSubstituteOptions)   => "PcreSubstituteOptions.None",
                    var t when t == typeof(Func<PcreMatch, string>) => "match => \"foo\"",
                    _                                               => throw new InvalidOperationException($"Unexpected parameter type for {parameter.Name}: {parameter.ParameterType}")
                };

                sb.Append(value).Append(", ");
            }

            sb.Remove(sb.Length - 2, 2).AppendLine(");");
        }

        sb.AppendLine(
            """
                }
            }
            """
        );

        var result = Generate(sb.ToString());

        await Verify(result);

        var output = (await result.GeneratedTrees.Single().GetTextAsync()).ToString();
        var interceptedCount = Regex.Matches(output, @"\bpublic\b").Count - 1; // Subtract InterceptsLocationAttribute

        Assert.That(interceptedCount, Is.EqualTo(methods.Count), "Not all methods were intercepted");
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

#if DEBUG
        if (diagnostics.Any())
            Console.WriteLine(result.GeneratedTrees.FirstOrDefault()?.GetText());
#endif

        Assert.That(diagnostics, Is.Empty);

        return result;
    }

    private static Task Verify([StringSyntax("csharp")] string input)
    {
        var result = Generate(input);
        return Verify(result);
    }

    [MustUseReturnValue]
    private static Task Verify([StringSyntax("csharp")] GeneratorDriverRunResult result)
    {
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
