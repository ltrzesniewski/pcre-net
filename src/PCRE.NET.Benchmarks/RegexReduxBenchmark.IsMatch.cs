using System.Linq;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace PCRE.NET.Benchmarks;

[MemoryDiagnoser]
public class RegexReduxBenchmarkIsMatch
{
    private static readonly Regex[] _regexes;
    private static readonly PcreRegex[] _pcreRegexes;

    static RegexReduxBenchmarkIsMatch()
    {
        _regexes = RegexReduxBenchmarkData.Patterns.Select(pattern => new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant)).ToArray();
        _pcreRegexes = RegexReduxBenchmarkData.Patterns.Select(pattern => new PcreRegex(pattern, PcreOptions.Compiled)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int Regex()
    {
        var matches = 0;

        foreach (var regex in _regexes)
        {
            if (regex.IsMatch(RegexReduxBenchmarkData.Subject))
                ++matches;
        }

        return matches;
    }

    [Benchmark]
    public int PcreRegex()
    {
        var matches = 0;

        foreach (var regex in _pcreRegexes)
        {
            if (regex.IsMatch(RegexReduxBenchmarkData.Subject))
                ++matches;
        }

        return matches;
    }
}
