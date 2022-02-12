using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using PCRE.Internal;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public unsafe class PcreMatchBufferTests
{
    [Test]
    public void should_use_match_buffer()
    {
        var re = new PcreRegex("foo");
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("foo".AsSpan());

        Assert.That(Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]), Is.True);
    }

    [Test]
    public void should_use_match_buffer_for_no_match()
    {
        var re = new PcreRegex("foo");
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("bar".AsSpan());

        Assert.That(Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]), Is.True);
    }

    [Test]
    public void should_use_callout_buffer()
    {
        var re = new PcreRegex(@"f(o)(?C1)o");
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("foo".AsSpan(), data =>
        {
            Assert.That(data.Match.Value.ToString(), Is.EqualTo("fo"));
            Assert.That(data.Match[1].Value.ToString(), Is.EqualTo("o"));

            Assert.That(Unsafe.AreSame(ref MemoryMarshal.GetReference(data.OutputVector), ref buffer.CalloutOutputVector[0]), Is.True);

            return PcreCalloutResult.Pass;
        });

        Assert.That(match.Success, Is.True);
    }

#if NETCOREAPP

    [Test]
    public void should_not_allocate()
    {
        // This is a simplified version of AllocationTest

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

        for (var i = 0; i < 10; ++i)
            Iteration();

        var bytesBefore = GC.GetAllocatedBytesForCurrentThread();

        for (var i = 0; i < 10000; ++i)
            Iteration();

        var bytesAfter = GC.GetAllocatedBytesForCurrentThread();

        Assert.That(bytesAfter - bytesBefore, Is.Zero);

        void Iteration()
        {
            var matches = buffer.Matches(subject, 0, PcreMatchOptions.None, static data =>
            {
                _ = data.Match.Groups["char"].Value;
                _ = data.Match.Groups[^1].Value;
                _ = data.String;

                return PcreCalloutResult.Pass;
            });

            foreach (var match in matches)
            {
                _ = match.Value;
                _ = match.Groups["char"].Value;
                _ = match.Groups[^1].Value;
            }
        }
    }

#endif
}
