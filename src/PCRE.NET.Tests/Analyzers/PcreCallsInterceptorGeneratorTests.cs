using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using PCRE.Analyzers;

namespace PCRE.Tests.Analyzers;

[TestFixture]
public class PcreCallsInterceptorGeneratorTests : BaseInterceptorTests<PcreCallsInterceptorGenerator>
{
    [Test]
    public Task generates_static_intercepts_with_literals()
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
    public Task does_not_generate_static_intercepts_with_non_literals()
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
    public Task generates_instance_replace_intercepts_with_literals()
    {
        return Verify(
            """
            using PCRE;

            class C
            {
                void M()
                {
                    var regex = new PcreRegex("foo(?<group>bar)baz");

                    _ = regex.Replace("subject", "");
                    _ = regex.Replace("subject", "replacement");
                    _ = regex.Replace("subject", "a $$ b");
                    _ = regex.Replace("subject", "a $& b");
                    _ = regex.Replace("subject", "a $0 b");
                    _ = regex.Replace("subject", "a $1 b");
                    _ = regex.Replace("subject", "a $2 b");
                    _ = regex.Replace("subject", "a ${group} b");
                    _ = regex.Replace("subject", "a ${other} b");
                    _ = regex.Replace("subject", "a $` b");
                    _ = regex.Replace("subject", "a $' b");
                    _ = regex.Replace("subject", "a $_ b");
                    _ = regex.Replace("subject", "a $+ b");
                    _ = regex.Replace("subject", "a $+ b");
                }
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
}
