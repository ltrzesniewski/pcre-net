using System;
using System.Linq;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace PCRE.NET.Benchmarks;

[MemoryDiagnoser]
public class RegexReduxBenchmarkMatches
{
    private static readonly Regex[] _regexes;
    private static readonly PcreRegex[] _pcreRegexes;
    private static readonly PcreMatchBuffer[] _pcreRegexBuffers;

    static RegexReduxBenchmarkMatches()
    {
        _regexes = RegexReduxBenchmarkData.Patterns.Select(pattern => new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant)).ToArray();
        _pcreRegexes = RegexReduxBenchmarkData.Patterns.Select(pattern => new PcreRegex(pattern, PcreOptions.Compiled)).ToArray();
        _pcreRegexBuffers = _pcreRegexes.Select(re => re.CreateMatchBuffer()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int Regex()
    {
        var length = 0;

        foreach (var regex in _regexes)
        {
            foreach (Match match in regex.Matches(RegexReduxBenchmarkData.Subject))
                length += match.Length;
        }

        return length;
    }

    [Benchmark]
    public int PcreRegex()
    {
        var length = 0;

        foreach (var regex in _pcreRegexes)
        {
            foreach (var match in regex.Matches(RegexReduxBenchmarkData.Subject))
                length += match.Length;
        }

        return length;
    }

    [Benchmark]
    public int PcreRegexSpan()
    {
        var length = 0;

        foreach (var regex in _pcreRegexes)
        {
            foreach (var match in regex.Matches(RegexReduxBenchmarkData.Subject.AsSpan()))
                length += match.Length;
        }

        return length;
    }

    [Benchmark]
    public int PcreRegexZeroAlloc()
    {
        var length = 0;

        foreach (var regex in _pcreRegexBuffers)
        {
            foreach (var match in regex.Matches(RegexReduxBenchmarkData.Subject.AsSpan()))
                length += match.Length;
        }

        return length;
    }
}
