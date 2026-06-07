using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace PCRE.Tests.Integration;

public class IntegrationTests
{
    private const string _reset = "\e[0m";
    private const string _bold = "\e[1m";
    private const string _red = "\e[91m";
    private const string _green = "\e[92m";

    private bool _success = true;

    public static int Main()
    {
        var tests = new IntegrationTests();
        return tests.Run() ? 0 : 1;
    }

    private bool Run()
    {
        _success = true;

        Safe(() => RunTestUtf16(PcreOptions.None));
        Safe(() => RunTestUtf16(PcreOptions.Compiled));
        Safe(() => RunTestUtf8(PcreOptions.None));
        Safe(() => RunTestUtf8(PcreOptions.Compiled));
        Safe(RunStaticInterceptorTest);
        Safe(RunReplacementPatternTest);
        RunBuildTest();

        Console.WriteLine();
        Console.WriteLine($"{_bold}Integration tests: {(_success ? $"{_green}PASSED" : $"{_red}FAILED")}{_reset}");
        Console.WriteLine();

        return _success;
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

        // subject, pattern, options
        Check(PcreRegex.Match("FOO", "f.o", PcreOptions.Caseless).Success);
        Check(PcreRegex.Match("FOO", "f.o", PcreOptions.Caseless | PcreOptions.Compiled).Success);
        Check(!PcreRegex.Match("FOO", "f.o", PcreOptions.None).Success);

        Check(PcreRegex.IsMatch("FOO", "f.o", PcreOptions.Caseless));
        Check(PcreRegex.IsMatch("FOO", "f.o", PcreOptions.Caseless | PcreOptions.Compiled));
        Check(!PcreRegex.IsMatch("FOO", "f.o", PcreOptions.None));

        // subject, pattern, options, startIndex
        Check(PcreRegex.Match("FOO", "f.o", startIndex: 0, options: PcreOptions.Caseless).Success);
        Check(!PcreRegex.Match("FOO", "f.o", startIndex: 1, options: PcreOptions.Caseless).Success);

        Check(PcreRegex.IsMatch("FOO", "f.o", startIndex: 0, options: PcreOptions.Caseless));
        Check(!PcreRegex.IsMatch("FOO", "f.o", startIndex: 1, options: PcreOptions.Caseless));

        // Another regex
        Check(PcreRegex.Match("bar", "b.r").Success);
        Check(PcreRegex.IsMatch("bar", "b.r"));

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

        return;

        static string GetRegexPattern() => "baz";
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

        return;

        string NonInterceptedReplace(string input, string replacement)
            => regex.Replace(input, replacement);
    }

    private void RunBuildTest()
    {
        Header("Build");

        Check(!RuntimeFeature.IsDynamicCodeSupported);
        Check(string.IsNullOrEmpty(GetAssemblyLocation()));

        const bool isInIntegrationTest =
#if PCRENET_INTEGRATION_TEST
            true;
#else
            false;
#endif

        Check(isInIntegrationTest, "Built in integration tests mode");

        [UnconditionalSuppressMessage("SingleFile", "IL3000")]
        static string GetAssemblyLocation()
            => typeof(IntegrationTests).Assembly.Location;
    }

    private static void Header(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"{_bold}{title}{_reset}");
    }

    private void Check(bool success, [CallerArgumentExpression(nameof(success))] string? code = null)
    {
        if (success)
            Pass(code);
        else
            Fail(code);
    }

    private static void Pass(string? message)
    {
        Console.WriteLine($"  {_green}PASSED:{_reset} {message}");
    }

    private void Fail(string? message)
    {
        Console.WriteLine($"  {_red}FAILED:{_reset} {message}");
        _success = false;
    }

    private void Safe(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Fail(ex.Message);
        }
    }
}
