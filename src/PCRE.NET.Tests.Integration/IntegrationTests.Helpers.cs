using System;
using System.Runtime.CompilerServices;

namespace PCRE.Tests.Integration;

public partial class IntegrationTests
{
    private const string _reset = "\x1B[0m";
    private const string _bold = "\x1B[1m";
    private const string _red = "\x1B[91m";
    private const string _green = "\x1B[92m";
    private const string _yellow = "\x1B[93m";

    private bool _success = true;

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

    private static void Ignore()
    {
        Console.WriteLine($"  {_yellow}IGNORED{_reset}");
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

    private void PrintSummary()
    {
        Console.WriteLine();
        Console.WriteLine($"{_bold}Integration tests: {(_success ? $"{_green}PASSED" : $"{_red}FAILED")}{_reset}");
        Console.WriteLine();
    }
}
