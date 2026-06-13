using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PCRE.Tests.Integration;

public partial class IntegrationTests
{
    public static int Main(string[] args)
    {
        var tests = new IntegrationTests();
        return tests.Run(args) ? 0 : 1;
    }

    private bool Run(string[] args)
    {
        _success = true;

        CheckArgs(args, out var aot, out var build);
        Safe(() => RunTestUtf16(PcreOptions.None));
        Safe(() => RunTestUtf16(PcreOptions.Compiled));
        Safe(() => RunTestUtf8(PcreOptions.None));
        Safe(() => RunTestUtf8(PcreOptions.Compiled));
        Safe(RunStaticInterceptorTest);
        Safe(RunReplacementPatternTest);
        Safe(() => CheckAot(aot));
        Safe(() => CheckBuild(build));

        PrintSummary();
        return _success;
    }

    private void CheckArgs(string[] args, out bool aot, out bool build)
    {
        Header("Arguments");

        var full = args.Contains("--full");
        aot = full || args.Contains("--aot");
        build = full || args.Contains("--build");

        Check(args.All(arg => arg is "--full" or "--aot" or "--build"));
    }

    private void RunTestUtf16(PcreOptions options)
    {
        Header($"UTF-16, Options: {options}");

        var re = new PcreRegex("a+(b+)c+", options);
        var match = re.Match("xxxaaabbccczzz");

        Check(match.Success);
        Check(match.CaptureCount == 1);
        Check(match.Value == "aaabbccc");
        Check(match.ValueSpan.ToString() == "aaabbccc");
        Check(match.Index == 3);
        Check(match.EndIndex == 11);
        Check(match.Length == 8);

        Check(match[1].Success);
        Check(match[1].Value == "bb");
        Check(match[1].ValueSpan.ToString() == "bb");
        Check(match[1].Index == 6);
        Check(match[1].Length == 2);

        Check(ReferenceEquals(match.Groups[1], match[1]));

        using var matchBuffer = re.CreateMatchBuffer();
        var bufferedMatch = matchBuffer.Match("xxxaaabbccczzz");

        Check(bufferedMatch.Success);
        Check(bufferedMatch.CaptureCount == 1);
        Check(bufferedMatch.Value is "aaabbccc");
        Check(bufferedMatch.Index == 3);
        Check(bufferedMatch.EndIndex == 11);
        Check(bufferedMatch.Length == 8);

        Check(bufferedMatch[1].Success);
        Check(bufferedMatch[1].Value is "bb");
        Check(bufferedMatch[1].Index == 6);
        Check(bufferedMatch[1].Length == 2);
    }

    private void RunTestUtf8(PcreOptions options)
    {
        Header($"UTF-8, Options: {options}");

        var re = new PcreRegexUtf8("a+(b+)c+"u8, options);
        var match = re.Match("xxxaaabbccczzz"u8);

        Check(match.Success);
        Check(match.CaptureCount == 1);
        Check(match.Value.SequenceEqual("aaabbccc"u8));
        Check(match.Index == 3);
        Check(match.EndIndex == 11);
        Check(match.Length == 8);

        Check(match[1].Success);
        Check(match[1].Value.SequenceEqual("bb"u8));
        Check(match[1].Index == 6);
        Check(match[1].Length == 2);

        using var matchBuffer = re.CreateMatchBuffer();
        var bufferedMatch = matchBuffer.Match("xxxaaabbccczzz"u8);

        Check(bufferedMatch.Success);
        Check(bufferedMatch.CaptureCount == 1);
        Check(bufferedMatch.Value.SequenceEqual("aaabbccc"u8));
        Check(bufferedMatch.Index == 3);
        Check(bufferedMatch.EndIndex == 11);
        Check(bufferedMatch.Length == 8);

        Check(bufferedMatch[1].Success);
        Check(bufferedMatch[1].Value.SequenceEqual("bb"u8));
        Check(bufferedMatch[1].Index == 6);
        Check(bufferedMatch[1].Length == 2);
    }

    [SuppressMessage("ReSharper", "RedundantVerbatimStringPrefix")]
    private void RunStaticInterceptorTest()
    {
        Header("Static Interceptor");

        // subject, pattern
        Check(PcreRegex.Match("foo", "f.o").Success);
        Check(PcreRegex.Match("foo", "f.o").Success);
        Check(!PcreRegex.Match("FOO", "f.o").Success);

        Check(PcreRegex.IsMatch("foo", "f.o"));
        Check(PcreRegex.IsMatch("foo", "f.o"));
        Check(!PcreRegex.IsMatch("FOO", "f.o"));

        Check(PcreRegex.Matches("foo", "f.o").Any());
        Check(PcreRegex.Matches("foo", "f.o").Any());
        Check(!PcreRegex.Matches("FOO", "f.o").Any());

        // subject, pattern, options
        Check(PcreRegex.Match("FOO", "f.o", PcreOptions.Caseless).Success);
        Check(PcreRegex.Match("FOO", "f.o", PcreOptions.Caseless | PcreOptions.Compiled).Success);
        Check(!PcreRegex.Match("FOO", "f.o", PcreOptions.None).Success);

        Check(PcreRegex.IsMatch("FOO", "f.o", PcreOptions.Caseless));
        Check(PcreRegex.IsMatch("FOO", "f.o", PcreOptions.Caseless | PcreOptions.Compiled));
        Check(!PcreRegex.IsMatch("FOO", "f.o", PcreOptions.None));

        Check(PcreRegex.Matches("FOO", "f.o", PcreOptions.Caseless).Any());
        Check(PcreRegex.Matches("FOO", "f.o", PcreOptions.Caseless | PcreOptions.Compiled).Any());
        Check(!PcreRegex.Matches("FOO", "f.o", PcreOptions.None).Any());

        // subject, pattern, options, startIndex
        Check(PcreRegex.Match("FOO", "f.o", startIndex: 0, options: PcreOptions.Caseless).Success);
        Check(!PcreRegex.Match("FOO", "f.o", startIndex: 1, options: PcreOptions.Caseless).Success);

        Check(PcreRegex.IsMatch("FOO", "f.o", startIndex: 0, options: PcreOptions.Caseless));
        Check(!PcreRegex.IsMatch("FOO", "f.o", startIndex: 1, options: PcreOptions.Caseless));

        Check(PcreRegex.Matches("FOO", "f.o", startIndex: 0, options: PcreOptions.Caseless).Any());
        Check(!PcreRegex.Matches("FOO", "f.o", startIndex: 1, options: PcreOptions.Caseless).Any());

        // Another regex
        Check(PcreRegex.Match("bar", "b.r").Success);
        Check(PcreRegex.IsMatch("bar", "b.r"));
        Check(PcreRegex.Matches("bar", "b.r").Any());

        // Other string literals
        const string pattern = @"(?x)baz";
        Check(PcreRegex.IsMatch("baz", pattern));
        Check(PcreRegex.IsMatch("baz", @"(?x)baz"));
        Check(PcreRegex.IsMatch("baz", """(?x)baz"""));
        Check(
            PcreRegex.IsMatch(
                "baz",
                """
                  (?x)

                  b
                  a
                  z

                  """
            ),
            "Multiline raw string"
        );

        // Non-literal
        Check(PcreRegex.IsMatch("baz", GetRegexPattern()));

        // Split
        Check(PcreRegex.Split("a b c", @"\s+").ToList() is ["a", "b", "c"]);
        Check(PcreRegex.Split("a b c", @"\s+", PcreOptions.Caseless).ToList() is ["a", "b", "c"]);
        Check(PcreRegex.Split("a b c", @"\s+", PcreOptions.Compiled).ToList() is ["a", "b", "c"]);
        Check(PcreRegex.Split("a b c", @"\s+", 1).ToList() is ["a", "b c"]);
        Check(PcreRegex.Split("a b c", @"\s+", PcreOptions.Compiled, PcreSplitOptions.None).ToList() is ["a", "b", "c"]);
        Check(PcreRegex.Split("a b c", @"\s+", PcreOptions.Compiled, PcreSplitOptions.None, 1).ToList() is ["a", "b c"]);
        Check(PcreRegex.Split("a b c", @"\s+", PcreOptions.Compiled, PcreSplitOptions.None, 1, 2).ToList() is ["a b", "c"]);

        // Substitute
        Check(PcreRegex.Substitute("a b c", @"\s+", "-") == "a-b c");
        Check(PcreRegex.Substitute("a b c", @"\s+", "-", PcreOptions.Compiled, PcreSubstituteOptions.SubstituteGlobal) == "a-b-c");

        // Out-of-order parameters
        Check(PcreRegex.IsMatch(pattern: "f.o", startIndex: GetSubjectAndStartIndex(out var subject), options: PcreOptions.Caseless, subject: subject));

        return;

        static string GetRegexPattern() => "baz";

        static int GetSubjectAndStartIndex(out string subject)
        {
            subject = "FOO";
            return 0;
        }
    }

    [SuppressMessage("ReSharper", "RedundantVerbatimStringPrefix")]
    private void RunReplacementPatternTest()
    {
        Header("Replacement Pattern Interceptor");

        var regex = new PcreRegex("a+(?<group>b+)(c+)");

        Check(regex.Replace("abbcbar", "foo") == "foobar");

        Check(regex.Replace("<abc>", "") == "<>");
        Check(regex.Replace("<abc>", "replacement") == "<replacement>");
        Check(regex.Replace("<abc>", "[$$]") == "<[$]>");
        Check(regex.Replace("<abc>", "[$&]") == "<[abc]>");
        Check(regex.Replace("<abc>", "[$0]") == "<[abc]>");
        Check(regex.Replace("<abc>", "[$1]") == "<[b]>");
        Check(regex.Replace("<abc>", "[$2]") == "<[c]>");
        Check(regex.Replace("<abc>", "[$3]") == "<[$3]>");
        Check(regex.Replace("<abc>", "[${group}]") == "<[b]>");
        Check(regex.Replace("<abc>", "[${other}]") == "<[${other}]>");
        Check(regex.Replace("<abc>", "[${2}]") == "<[c]>");
        Check(regex.Replace("<abc>", "[$`]") == "<[<]>");
        Check(regex.Replace("<abc>", "[$']") == "<[>]>");
        Check(regex.Replace("<abc>", "[$_]") == "<[<abc>]>");
        Check(regex.Replace("<abc>", "[$+]") == "<[c]>");
        Check(regex.Replace("<abc>", "[$+]") == "<[c]>");

        Check(NonInterceptedReplace("<abc>", "") == "<>");
        Check(NonInterceptedReplace("<abc>", "replacement") == "<replacement>");
        Check(NonInterceptedReplace("<abc>", "[$$]") == "<[$]>");
        Check(NonInterceptedReplace("<abc>", "[$&]") == "<[abc]>");
        Check(NonInterceptedReplace("<abc>", "[$0]") == "<[abc]>");
        Check(NonInterceptedReplace("<abc>", "[$1]") == "<[b]>");
        Check(NonInterceptedReplace("<abc>", "[$2]") == "<[c]>");
        Check(NonInterceptedReplace("<abc>", "[$3]") == "<[$3]>");
        Check(NonInterceptedReplace("<abc>", "[${group}]") == "<[b]>");
        Check(NonInterceptedReplace("<abc>", "[${other}]") == "<[${other}]>");
        Check(NonInterceptedReplace("<abc>", "[${2}]") == "<[c]>");
        Check(NonInterceptedReplace("<abc>", "[$`]") == "<[<]>");
        Check(NonInterceptedReplace("<abc>", "[$']") == "<[>]>");
        Check(NonInterceptedReplace("<abc>", "[$_]") == "<[<abc>]>");
        Check(NonInterceptedReplace("<abc>", "[$+]") == "<[c]>");
        Check(NonInterceptedReplace("<abc>", "[$+]") == "<[c]>");

        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "") == "<>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "replacement") == "<replacement>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[$$]") == "<[$]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[$&]") == "<[abc]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[$0]") == "<[abc]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[$1]") == "<[b]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[$2]") == "<[c]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[$3]") == "<[$3]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[${group}]") == "<[b]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[${other}]") == "<[${other}]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[${2}]") == "<[c]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[$`]") == "<[<]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[$']") == "<[>]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[$_]") == "<[<abc>]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[$+]") == "<[c]>");
        Check(PcreRegex.Replace("<abc>", "a+(?<group>b+)(c+)", "[$+]") == "<[c]>");

        return;

        string NonInterceptedReplace(string input, string replacement)
            => regex.Replace(input, replacement);
    }

    private void CheckAot(bool run)
    {
        Header("AOT");

        if (!run)
        {
            Ignore();
            return;
        }

        Check(!RuntimeFeature.IsDynamicCodeSupported);
        Check(string.IsNullOrEmpty(GetAssemblyLocation()));

        return;

        [UnconditionalSuppressMessage("SingleFile", "IL3000")]
        static string GetAssemblyLocation()
            => typeof(IntegrationTests).Assembly.Location;
    }

    private void CheckBuild(bool run)
    {
        Header("Build");

        if (!run)
        {
            Ignore();
            return;
        }

        const bool isInIntegrationTest =
#if PCRENET_INTEGRATION_TEST
            true;
#else
            false;
#endif

        Check(isInIntegrationTest, "Built in integration tests mode");
    }
}
