using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using PCRE.Internal;
using PCRE.Tests.Support;
using Shouldly;

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

        Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]).ShouldBeTrue();
    }

    [Test]
    public void should_use_match_buffer_utf8()
    {
        var re = new PcreRegexUtf8("foo"u8);
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("foo"u8);

        Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]).ShouldBeTrue();
    }

    [Test]
    public void should_use_match_buffer_for_no_match()
    {
        var re = new PcreRegex("foo");
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("bar".AsSpan());

        Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]).ShouldBeTrue();
    }

    [Test]
    public void should_use_match_buffer_for_no_match_utf8()
    {
        var re = new PcreRegexUtf8("foo"u8);
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("bar"u8);

        Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]).ShouldBeTrue();
    }

    [Test]
    public void should_use_callout_buffer()
    {
        var re = new PcreRegex(@"f(o)(?C1)o");
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("foo".AsSpan(), data =>
        {
            data.Match.Value.ShouldBe("fo");
            data.Match[1].Value.ShouldBe("o");

            Unsafe.AreSame(ref MemoryMarshal.GetReference(data.OutputVector), ref buffer.CalloutOutputVector[0]).ShouldBeTrue();

            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
    }

    [Test]
    public void should_use_callout_buffer_utf8()
    {
        var re = new PcreRegexUtf8(@"f(o)(?C1)o"u8);
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("foo"u8, data =>
        {
            data.Match.Value.SequenceEqual("fo"u8).ShouldBeTrue();
            data.Match[1].Value.SequenceEqual("o"u8).ShouldBeTrue();

            Unsafe.AreSame(ref MemoryMarshal.GetReference(data.OutputVector), ref buffer.CalloutOutputVector[0]).ShouldBeTrue();

            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
    }

#if NET

    [Test]
    [Retry(10)]
    [NonParallelizable]
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

        (bytesAfter - bytesBefore).ShouldBe(0);

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

    [Test]
    [Retry(10)]
    [NonParallelizable]
    public void should_not_allocate_utf8()
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

        var re = new PcreRegexUtf8(regexBuilder.ToString(), PcreOptions.Compiled);
        var buffer = re.CreateMatchBuffer();
        var subject = Encoding.UTF8.GetBytes(subjectBuilder.ToString());

        for (var i = 0; i < 10; ++i)
            Iteration();

        var bytesBefore = GC.GetAllocatedBytesForCurrentThread();

        for (var i = 0; i < 10000; ++i)
            Iteration();

        var bytesAfter = GC.GetAllocatedBytesForCurrentThread();

        (bytesAfter - bytesBefore).ShouldBe(0);

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

    [Test]
    [Retry(10)]
    [NonParallelizable]
    public void should_not_allocate_8bit()
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

        var re = new PcreRegex8Bit(regexBuilder.ToString().ToLatin1Bytes(), TestSupport.Latin1Encoding, PcreOptions.Compiled);
        var buffer = re.CreateMatchBuffer();
        var subject = subjectBuilder.ToString().ToLatin1Bytes();

        for (var i = 0; i < 10; ++i)
            Iteration();

        var bytesBefore = GC.GetAllocatedBytesForCurrentThread();

        for (var i = 0; i < 10000; ++i)
            Iteration();

        var bytesAfter = GC.GetAllocatedBytesForCurrentThread();

        (bytesAfter - bytesBefore).ShouldBe(0);

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
