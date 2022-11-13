using System;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace PCRE.NET.Benchmarks;

[MemoryDiagnoser]
[ReturnValueValidator]
[SimpleJob(RuntimeMoniker.Net70)]
public class MemoryBenchmark
{
    private const string _pattern = @"\b(?<user>[-+\w.]+)@(?<domain>[-\w.]+\.[A-Za-z]{2,})\b";

    private readonly Regex _netRegex = new(_pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private readonly PcreRegex _pcreRegex = new(_pattern, PcreOptions.Compiled);

    private string Subject { get; }

    public MemoryBenchmark()
    {
        var sb = new StringBuilder();

        for (var i = 0; i < 10; ++i)
            sb.Append("foobar foo bar baz foobar@foo bar baz foobar@ foo bar baz foobar foo bar baz foobar foo bar baz foobar foo bar baz foo@bar.baz ");

        Subject = sb.ToString();
    }

    [Benchmark(Baseline = true)]
    public int NetRegex()
    {
        var length = 0;

        foreach (Match match in _netRegex.Matches(Subject))
            length += match.Value.Length + match.Groups["user"].Value.Length;

        return length;
    }

    [Benchmark]
    public int PcreRegex()
    {
        var length = 0;

        foreach (var match in _pcreRegex.Matches(Subject))
            length += match.Value.Length + match.Groups["user"].Value.Length;

        return length;
    }

    [Benchmark]
    public int PcreRegexRef()
    {
        var length = 0;

        foreach (var match in _pcreRegex.Matches(Subject.AsSpan()))
            length += match.Value.Length + match.Groups["user"].Value.Length;

        return length;
    }
}
