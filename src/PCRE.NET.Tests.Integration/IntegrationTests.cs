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

        Safe(() => RunTest(PcreOptions.None));
        Safe(() => RunTest(PcreOptions.Compiled));
        RunBuildTest();

        Console.WriteLine();
        Console.WriteLine($"{_bold}Integration tests: {(_success ? $"{_green}PASSED" : $"{_red}FAILED")}{_reset}");
        Console.WriteLine();

        return _success;
    }

    private void RunTest(PcreOptions options)
    {
        Header($"Options: {options}");

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
    }

    private void RunBuildTest()
    {
        Header("Build");

        Check(!RuntimeFeature.IsDynamicCodeSupported);
        Check(string.IsNullOrEmpty(GetAssemblyLocation()));

#if !PCRENET_INTEGRATION_TEST
        Fail("Not in integration tests mode");
#endif

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
