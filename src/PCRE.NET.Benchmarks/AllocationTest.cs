using System;
using System.Text;
using PCRE.Internal;

namespace PCRE.NET.Benchmarks
{
    internal static class AllocationTest
    {
        public static bool TestAllocations()
        {
            var regexBuilder = new StringBuilder();
            var subjectBuilder = new StringBuilder();

            regexBuilder.Append("(?<char>.)");

            for (var i = 0; i < 2 * InternalRegex.MaxStackAllocCaptureCount; ++i)
            {
                regexBuilder.Append(@"(?C{before})(.)(?C{after})");
                subjectBuilder.Append("foobar");
            }

            var re = new PcreRegex(regexBuilder.ToString(), PcreOptions.Compiled);
            var buffer = re.CreateMatchBuffer();
            var subject = subjectBuilder.ToString().AsSpan();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            var gcCountBefore = GC.CollectionCount(0);
            var matchCount = 0;

            for (var i = 0; i < 10000; ++i)
            {
                var matches = buffer.Matches(subject, 0, PcreMatchOptions.None, static data =>
                {
                    _ = data.Match.Groups["char"].Value;
                    _ = data.Match.Groups[data.Match.Groups.Count - 1].Value;
                    _ = data.String;

                    return PcreCalloutResult.Pass;
                });

                foreach (var match in matches)
                {
                    _ = match.Value;
                    _ = match.Groups["char"].Value;
                    _ = match.Groups[match.Groups.Count - 1].Value;
                    ++matchCount;
                }
            }

            var gcCountAfter = GC.CollectionCount(0);
            var gcCount = gcCountAfter - gcCountBefore;

            Console.WriteLine($"GC count: {gcCount}");
            Console.WriteLine($"Match count: {matchCount}");

            return gcCount == 0;
        }
    }
}
