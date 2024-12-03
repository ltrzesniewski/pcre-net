#if NET

using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace PCRE.NET.Benchmarks;

[MemoryDiagnoser]
public class HelloGoodbyeBenchmark
{
    // language=regex
    private const string _pattern = "^(?:Hello|Goodbye)";

    private static readonly Regex _regex = new(_pattern, RegexOptions.Compiled);
    private static readonly PcreMatchBuffer _pcreRegex = new PcreRegex(_pattern, PcreOptions.Compiled).CreateMatchBuffer();

    [Benchmark(Baseline = true)]
    public int Regex()
        => _regex.Match("Hello").Index;

    [Benchmark]
    public int PcreRegex()
        => _pcreRegex.Match("Hello", PcreMatchOptions.NoUtfCheck).Index;
}

#endif
