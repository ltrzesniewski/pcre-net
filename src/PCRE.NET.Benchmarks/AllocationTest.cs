using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using PCRE.Internal;

namespace PCRE.NET.Benchmarks;

internal static class AllocationTest
{
    [SuppressMessage("ReSharper", "UseIndexFromEndExpression")]
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
        var subject = subjectBuilder.ToString();
        var matchCount = 0;

        for (var i = 0; i < 10; ++i)
            Iteration();

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
        var gcCountBefore = GC.CollectionCount(0);
        var bytesBefore = GetAllocatedBytes();

        for (var i = 0; i < 25000; ++i)
            Iteration();

        var bytesAfter = GetAllocatedBytes();
        var gcCountAfter = GC.CollectionCount(0);

        var allocatedBytes = bytesAfter - bytesBefore;
        var gcCount = gcCountAfter - gcCountBefore;

#if NET
        Console.WriteLine($"Allocated bytes: {allocatedBytes}");
#endif

        Console.WriteLine($"GC count: {gcCount}");
        Console.WriteLine($"Match count: {matchCount}");

        return allocatedBytes == 0 && gcCount == 0;

        void Iteration()
        {
            var matches = buffer.Matches(subject.AsSpan(), 0, PcreMatchOptions.None, static data =>
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
    }

    private static long GetAllocatedBytes()
#if NET
        => GC.GetAllocatedBytesForCurrentThread();
#else
        => 0;
#endif
}
