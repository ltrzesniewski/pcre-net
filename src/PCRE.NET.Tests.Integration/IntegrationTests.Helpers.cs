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
        WriteLine();
        WriteLine($"{_bold}{title}{_reset}");
    }

    private static void Info(string label, string? value)
        => WriteLine($"  {_green}{label}:{_reset} {value}");

    private void Check(bool success, [CallerArgumentExpression(nameof(success))] string? code = null)
        => Check(success, true, code);

    private void Check(bool success, bool enforce, [CallerArgumentExpression(nameof(success))] string? code = null)
    {
        if (success)
            Pass(code);
        else
            Fail(code, enforce);
    }

    private static void Pass(string? message)
        => WriteLine($"  {_green}PASSED:{_reset} {message}");

    private void Fail(string? message, bool enforced)
    {
        if (enforced)
        {
            WriteLine($"  {_red}FAILED:{_reset} {message}");
            _success = false;
        }
        else
        {
            WriteLine($"  {_yellow}FAILED:{_reset} {message} {_yellow}(ignored){_reset}");
        }
    }

    private void Safe(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Fail(ex.Message, true);
        }
    }

    private T? Safe<T>(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            Fail(ex.Message, true);
            return default;
        }
    }

    private void PrintSummary()
    {
        WriteLine();
        WriteLine($"{_bold}Integration tests: {(_success ? $"{_green}PASSED" : $"{_red}FAILED")}{_reset}");
        WriteLine();
    }

    private static void WriteLine(string? message = null)
        => Console.WriteLine(message);
}
