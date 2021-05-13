using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace PCRE.NET.Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50, baseline: true)]
    public class AutoCalloutBenchmark
    {
        private const string _subjectString = "foo baz bar foobar";

        private readonly PcreRegex _regexInterpretedNoCallout;
        private readonly PcreRegex _regexInterpretedAutoCallout;
        private readonly PcreRegex _regexCompiledNoCallout;
        private readonly PcreRegex _regexCompiledAutoCallout;

        public AutoCalloutBenchmark()
        {
            const string regex = "foobar";

            _regexInterpretedNoCallout = new PcreRegex(regex, PcreOptions.None);
            _regexInterpretedAutoCallout = new PcreRegex(regex, PcreOptions.AutoCallout);

            _regexCompiledNoCallout = new PcreRegex(regex, PcreOptions.Compiled);
            _regexCompiledAutoCallout = new PcreRegex(regex, PcreOptions.Compiled | PcreOptions.AutoCallout);
        }

        [Benchmark(Baseline = true)]
        public PcreMatch InterpretedNoCallout()
            => _regexInterpretedNoCallout.Match(_subjectString);

        [Benchmark]
        public PcreMatch InterpretedAutoCallout()
            => _regexInterpretedAutoCallout.Match(_subjectString, _ => PcreCalloutResult.Pass);

        [Benchmark]
        public PcreMatch CompiledNoCallout()
            => _regexCompiledNoCallout.Match(_subjectString);

        [Benchmark]
        public PcreMatch CompiledAutoCallout()
            => _regexCompiledAutoCallout.Match(_subjectString, _ => PcreCalloutResult.Pass);
    }
}
