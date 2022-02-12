#if NETCOREAPP

using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace PCRE.NET.Benchmarks;

[MemoryDiagnoser]
public class HelloGoodbyeBenchmark
{
    private static readonly Regex _regex = new("^(?:Hello|Goodbye)", RegexOptions.Compiled);
    private static readonly PcreMatchBuffer _pcreRegex = new PcreRegex("^(?:Hello|Goodbye)", PcreOptions.Compiled).CreateMatchBuffer();

    [Benchmark(Baseline = true)]
    public int Regex()
        => _regex.Match("Hello").Index;

    [Benchmark]
    public int PcreRegex()
        => _pcreRegex.Match("Hello", PcreMatchOptions.NoUtfCheck).Index;
}

#endif
