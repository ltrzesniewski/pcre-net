using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using PCRE.Internal;
using PCRE.Tests.Support;
using Shouldly;

namespace PCRE.Tests.PcreNet;

[TestFixture]
[SuppressMessage("ReSharper", "ArrangeDefaultValueWhenTypeNotEvident")]
[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
public class MatchTests
{
    [Test]
    public void should_match_pattern()
    {
        var re = new PcreRegex(@"a+(b+)c+");
        var match = re.Match("xxxaaabbccczzz");

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(1);
        match.Value.ShouldBe("aaabbccc");
        match.ValueSpan.ShouldBe("aaabbccc");
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(11);
        match.Length.ShouldBe(8);

        match[1].ShouldNotBeNull();
        match[1].Success.ShouldBeTrue();
        match[1].Value.ShouldBe("bb");
        match[1].ValueSpan.ShouldBe("bb");
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match.Groups[1].ShouldBeSameAs(match[1]);
    }

    [Test]
    public void should_match_pattern_ref()
    {
        var re = new PcreRegex(@"a+(b+)c+");
        var match = re.Match("xxxaaabbccczzz".AsSpan());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(1);
        match.Value.ShouldBe("aaabbccc");
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(11);
        match.Length.ShouldBe(8);

        match[1].Success.ShouldBeTrue();
        match[1].Value.ShouldBe("bb");
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
    }

    [Test]
    public void should_match_pattern_buf()
    {
        var re = new PcreRegex(@"a+(b+)c+");
        var match = re.CreateMatchBuffer().Match("xxxaaabbccczzz".AsSpan());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(1);
        match.Value.ShouldBe("aaabbccc");
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(11);
        match.Length.ShouldBe(8);

        match[1].Success.ShouldBeTrue();
        match[1].Value.ShouldBe("bb");
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
    }

    [Test]
    public void should_match_pattern_utf8()
    {
        var re = new PcreRegexUtf8(@"a+(b+)c+"u8);
        var match = re.Match("xxxaaabbccczzz"u8);

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(1);
        match.Value.SequenceEqual("aaabbccc"u8).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(11);
        match.Length.ShouldBe(8);

        match[1].Success.ShouldBeTrue();
        match[1].Value.SequenceEqual("bb"u8).ShouldBeTrue();
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
    }

    [Test]
    public void should_match_pattern_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"a+(b+)c+"u8);
        var match = re.CreateMatchBuffer().Match("xxxaaabbccczzz"u8);

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(1);
        match.Value.SequenceEqual("aaabbccc"u8).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(11);
        match.Length.ShouldBe(8);

        match[1].Success.ShouldBeTrue();
        match[1].Value.SequenceEqual("bb"u8).ShouldBeTrue();
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
    }

    [Test]
    public void should_match_pattern_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a+(b+)c+".ToLatin1Bytes());
        var match = re.Match("xxxaaabbccczzz".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(1);
        match.Value.SequenceEqual("aaabbccc".ToLatin1Bytes()).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(11);
        match.Length.ShouldBe(8);

        match[1].Success.ShouldBeTrue();
        match[1].Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
    }

    [Test]
    public void should_match_pattern_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a+(b+)c+".ToLatin1Bytes());
        var match = re.CreateMatchBuffer().Match("xxxaaabbccczzz".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(1);
        match.Value.SequenceEqual("aaabbccc".ToLatin1Bytes()).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(11);
        match.Length.ShouldBe(8);

        match[1].Success.ShouldBeTrue();
        match[1].Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
    }

    [Test]
    public void should_support_multiple_groups()
    {
        var re = new PcreRegex(@"a+(b+)(c+)?(d+)e+");
        var match = re.Match("xxxaaabbddeeezzz");

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.ShouldBe("aaabbddeee");
        match.ValueSpan.ShouldBe("aaabbddeee");
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(10);

        match[1].ShouldNotBeNull();
        match[1].Success.ShouldBeTrue();
        match[1].IsDefined.ShouldBeTrue();
        match[1].Value.ShouldBe("bb");
        match[1].ValueSpan.ShouldBe("bb");
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].ShouldNotBeNull();
        match[2].Success.ShouldBeFalse();
        match[2].IsDefined.ShouldBeTrue();
        match[2].Value.ShouldBeSameAs(string.Empty);
        match[2].ValueSpan.Length.ShouldBe(0);
        match[2].Index.ShouldBe(-1);
        match[2].Length.ShouldBe(0);

        match[3].ShouldNotBeNull();
        match[3].Success.ShouldBeTrue();
        match[3].IsDefined.ShouldBeTrue();
        match[3].Value.ShouldBe("dd");
        match[3].ValueSpan.ShouldBe("dd");
        match[3].Index.ShouldBe(8);
        match[3].Length.ShouldBe(2);

        match[4].ShouldNotBeNull();
        match[4].Success.ShouldBeFalse();
        match[4].IsDefined.ShouldBeFalse();
        match[4].Value.ShouldBeSameAs(string.Empty);
        match[4].ValueSpan.Length.ShouldBe(0);
        match[4].Index.ShouldBe(-1);
        match[4].Length.ShouldBe(0);

        match.TryGetGroup(1, out var group).ShouldBeTrue();
        group.ShouldBeSameAs(match[1]);

        match.TryGetGroup(4, out group).ShouldBeFalse();
        group.ShouldBeNull();
    }

    [Test]
    public void should_support_multiple_groups_ref()
    {
        var re = new PcreRegex(@"a+(b+)(c+)?(d+)e+");
        var match = re.Match("xxxaaabbddeeezzz".AsSpan());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.ShouldBe("aaabbddeee");
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(10);

        match[1].Success.ShouldBeTrue();
        match[1].IsDefined.ShouldBeTrue();
        match[1].Value.ShouldBe("bb");
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
        match[2].IsDefined.ShouldBeTrue();
        match[2].Value.ToString().ShouldBeSameAs(string.Empty);
        match[2].Index.ShouldBe(-1);
        match[2].Length.ShouldBe(0);

        match[3].Success.ShouldBeTrue();
        match[3].IsDefined.ShouldBeTrue();
        match[3].Value.ShouldBe("dd");
        match[3].Index.ShouldBe(8);
        match[3].Length.ShouldBe(2);

        match[4].Success.ShouldBeFalse();
        match[4].IsDefined.ShouldBeFalse();
        match[4].Value.ToString().ShouldBeSameAs(string.Empty);
        match[4].Index.ShouldBe(-1);
        match[4].Length.ShouldBe(0);

        match.TryGetGroup(1, out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.ShouldBe("bb");
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup(4, out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.ToString().ShouldBeSameAs(string.Empty);
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_support_multiple_groups_buf()
    {
        var re = new PcreRegex(@"a+(b+)(c+)?(d+)e+");
        var match = re.CreateMatchBuffer().Match("xxxaaabbddeeezzz".AsSpan());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.ShouldBe("aaabbddeee");
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(10);

        match[1].Success.ShouldBeTrue();
        match[1].IsDefined.ShouldBeTrue();
        match[1].Value.ShouldBe("bb");
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
        match[2].IsDefined.ShouldBeTrue();
        match[2].Value.ToString().ShouldBeSameAs(string.Empty);
        match[2].Index.ShouldBe(-1);
        match[2].Length.ShouldBe(0);

        match[3].Success.ShouldBeTrue();
        match[3].IsDefined.ShouldBeTrue();
        match[3].Value.ShouldBe("dd");
        match[3].Index.ShouldBe(8);
        match[3].Length.ShouldBe(2);

        match[4].Success.ShouldBeFalse();
        match[4].IsDefined.ShouldBeFalse();
        match[4].Value.ToString().ShouldBeSameAs(string.Empty);
        match[4].Index.ShouldBe(-1);
        match[4].Length.ShouldBe(0);

        match.TryGetGroup(1, out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.ShouldBe("bb");
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup(4, out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.ToString().ShouldBeSameAs(string.Empty);
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_support_multiple_groups_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"a+(b+)(c+)?(d+)e+"u8);
        var match = re.CreateMatchBuffer().Match("xxxaaabbddeeezzz"u8);

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.SequenceEqual("aaabbddeee"u8).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(10);

        match[1].Success.ShouldBeTrue();
        match[1].IsDefined.ShouldBeTrue();
        match[1].Value.SequenceEqual("bb"u8).ShouldBeTrue();
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
        match[2].IsDefined.ShouldBeTrue();
        match[2].Value.IsEmpty.ShouldBeTrue();
        match[2].Index.ShouldBe(-1);
        match[2].Length.ShouldBe(0);

        match[3].Success.ShouldBeTrue();
        match[3].IsDefined.ShouldBeTrue();
        match[3].Value.SequenceEqual("dd"u8).ShouldBeTrue();
        match[3].Index.ShouldBe(8);
        match[3].Length.ShouldBe(2);

        match[4].Success.ShouldBeFalse();
        match[4].IsDefined.ShouldBeFalse();
        match[4].Value.IsEmpty.ShouldBeTrue();
        match[4].Index.ShouldBe(-1);
        match[4].Length.ShouldBe(0);

        match.TryGetGroup(1, out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.SequenceEqual("bb"u8).ShouldBeTrue();
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup(4, out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.IsEmpty.ShouldBeTrue();
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_support_multiple_groups_utf8()
    {
        var re = new PcreRegexUtf8(@"a+(b+)(c+)?(d+)e+"u8);
        var match = re.Match("xxxaaabbddeeezzz"u8);

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.SequenceEqual("aaabbddeee"u8).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(10);

        match[1].Success.ShouldBeTrue();
        match[1].IsDefined.ShouldBeTrue();
        match[1].Value.SequenceEqual("bb"u8).ShouldBeTrue();
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
        match[2].IsDefined.ShouldBeTrue();
        match[2].Value.IsEmpty.ShouldBeTrue();
        match[2].Index.ShouldBe(-1);
        match[2].Length.ShouldBe(0);

        match[3].Success.ShouldBeTrue();
        match[3].IsDefined.ShouldBeTrue();
        match[3].Value.SequenceEqual("dd"u8).ShouldBeTrue();
        match[3].Index.ShouldBe(8);
        match[3].Length.ShouldBe(2);

        match[4].Success.ShouldBeFalse();
        match[4].IsDefined.ShouldBeFalse();
        match[4].Value.IsEmpty.ShouldBeTrue();
        match[4].Index.ShouldBe(-1);
        match[4].Length.ShouldBe(0);

        match.TryGetGroup(1, out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.SequenceEqual("bb"u8).ShouldBeTrue();
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup(4, out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.IsEmpty.ShouldBeTrue();
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_support_multiple_groups_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a+(b+)(c+)?(d+)e+".ToLatin1Bytes());
        var match = re.Match("xxxaaabbddeeezzz".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.SequenceEqual("aaabbddeee".ToLatin1Bytes()).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(10);

        match[1].Success.ShouldBeTrue();
        match[1].IsDefined.ShouldBeTrue();
        match[1].Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
        match[2].IsDefined.ShouldBeTrue();
        match[2].Value.IsEmpty.ShouldBeTrue();
        match[2].Index.ShouldBe(-1);
        match[2].Length.ShouldBe(0);

        match[3].Success.ShouldBeTrue();
        match[3].IsDefined.ShouldBeTrue();
        match[3].Value.SequenceEqual("dd".ToLatin1Bytes()).ShouldBeTrue();
        match[3].Index.ShouldBe(8);
        match[3].Length.ShouldBe(2);

        match[4].Success.ShouldBeFalse();
        match[4].IsDefined.ShouldBeFalse();
        match[4].Value.IsEmpty.ShouldBeTrue();
        match[4].Index.ShouldBe(-1);
        match[4].Length.ShouldBe(0);

        match.TryGetGroup(1, out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup(4, out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.IsEmpty.ShouldBeTrue();
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_support_multiple_groups_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a+(b+)(c+)?(d+)e+".ToLatin1Bytes());
        var match = re.CreateMatchBuffer().Match("xxxaaabbddeeezzz".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.SequenceEqual("aaabbddeee".ToLatin1Bytes()).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(10);

        match[1].Success.ShouldBeTrue();
        match[1].IsDefined.ShouldBeTrue();
        match[1].Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        match[1].Index.ShouldBe(6);
        match[1].Length.ShouldBe(2);

        match[2].Success.ShouldBeFalse();
        match[2].IsDefined.ShouldBeTrue();
        match[2].Value.IsEmpty.ShouldBeTrue();
        match[2].Index.ShouldBe(-1);
        match[2].Length.ShouldBe(0);

        match[3].Success.ShouldBeTrue();
        match[3].IsDefined.ShouldBeTrue();
        match[3].Value.SequenceEqual("dd".ToLatin1Bytes()).ShouldBeTrue();
        match[3].Index.ShouldBe(8);
        match[3].Length.ShouldBe(2);

        match[4].Success.ShouldBeFalse();
        match[4].IsDefined.ShouldBeFalse();
        match[4].Value.IsEmpty.ShouldBeTrue();
        match[4].Index.ShouldBe(-1);
        match[4].Length.ShouldBe(0);

        match.TryGetGroup(1, out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup(4, out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.IsEmpty.ShouldBeTrue();
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups()
    {
        var re = new PcreRegex(@"(a|(z))(bc)");
        var match = re.Match("abc");

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);

        match[1].Success.ShouldBeTrue();
        match[1].Index.ShouldBe(0);
        match[1].EndIndex.ShouldBe(1);
        match[1].Value.ShouldBe("a");
        match[1].ValueSpan.ShouldBe("a");

        match[2].Success.ShouldBeFalse();
        match[2].Index.ShouldBe(-1);
        match[2].EndIndex.ShouldBe(-1);
        match[2].Value.ShouldBeSameAs(string.Empty);
        match[2].ValueSpan.Length.ShouldBe(0);

        match[3].Success.ShouldBeTrue();
        match[3].Index.ShouldBe(1);
        match[3].Length.ShouldBe(2);
        match[3].Value.ShouldBe("bc");
        match[3].ValueSpan.ShouldBe("bc");
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_ref()
    {
        var re = new PcreRegex(@"(a|(z))(bc)");
        var match = re.Match("abc".AsSpan());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);

        match[1].Success.ShouldBeTrue();
        match[1].Index.ShouldBe(0);
        match[1].EndIndex.ShouldBe(1);
        match[1].Value.ShouldBe("a");

        match[2].Success.ShouldBeFalse();
        match[2].Index.ShouldBe(-1);
        match[2].EndIndex.ShouldBe(-1);
        match[2].Value.ToString().ShouldBeSameAs(string.Empty);

        match[3].Success.ShouldBeTrue();
        match[3].Index.ShouldBe(1);
        match[3].Length.ShouldBe(2);
        match[3].Value.ShouldBe("bc");
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_buf()
    {
        var re = new PcreRegex(@"(a|(z))(bc)");
        var match = re.CreateMatchBuffer().Match("abc".AsSpan());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);

        match[1].Success.ShouldBeTrue();
        match[1].Index.ShouldBe(0);
        match[1].EndIndex.ShouldBe(1);
        match[1].Value.ShouldBe("a");

        match[2].Success.ShouldBeFalse();
        match[2].Index.ShouldBe(-1);
        match[2].EndIndex.ShouldBe(-1);
        match[2].Value.ToString().ShouldBeSameAs(string.Empty);

        match[3].Success.ShouldBeTrue();
        match[3].Index.ShouldBe(1);
        match[3].Length.ShouldBe(2);
        match[3].Value.ShouldBe("bc");
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_utf8()
    {
        var re = new PcreRegexUtf8(@"(a|(z))(bc)"u8);
        var match = re.Match("abc"u8);

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);

        match[1].Success.ShouldBeTrue();
        match[1].Index.ShouldBe(0);
        match[1].EndIndex.ShouldBe(1);
        match[1].Value.SequenceEqual("a"u8).ShouldBeTrue();

        match[2].Success.ShouldBeFalse();
        match[2].Index.ShouldBe(-1);
        match[2].EndIndex.ShouldBe(-1);
        match[2].Value.IsEmpty.ShouldBeTrue();

        match[3].Success.ShouldBeTrue();
        match[3].Index.ShouldBe(1);
        match[3].Length.ShouldBe(2);
        match[3].Value.SequenceEqual("bc"u8).ShouldBeTrue();
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"(a|(z))(bc)"u8);
        var match = re.CreateMatchBuffer().Match("abc"u8);

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);

        match[1].Success.ShouldBeTrue();
        match[1].Index.ShouldBe(0);
        match[1].EndIndex.ShouldBe(1);
        match[1].Value.SequenceEqual("a"u8).ShouldBeTrue();

        match[2].Success.ShouldBeFalse();
        match[2].Index.ShouldBe(-1);
        match[2].EndIndex.ShouldBe(-1);
        match[2].Value.IsEmpty.ShouldBeTrue();

        match[3].Success.ShouldBeTrue();
        match[3].Index.ShouldBe(1);
        match[3].Length.ShouldBe(2);
        match[3].Value.SequenceEqual("bc"u8).ShouldBeTrue();
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(a|(z))(bc)".ToLatin1Bytes());
        var match = re.CreateMatchBuffer().Match("abc".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);

        match[1].Success.ShouldBeTrue();
        match[1].Index.ShouldBe(0);
        match[1].EndIndex.ShouldBe(1);
        match[1].Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();

        match[2].Success.ShouldBeFalse();
        match[2].Index.ShouldBe(-1);
        match[2].EndIndex.ShouldBe(-1);
        match[2].Value.IsEmpty.ShouldBeTrue();

        match[3].Success.ShouldBeTrue();
        match[3].Index.ShouldBe(1);
        match[3].Length.ShouldBe(2);
        match[3].Value.SequenceEqual("bc".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    public void should_match_starting_at_end_of_string()
    {
        var re = new PcreRegex(@"(?<=a)");
        var match = re.Match("xxa", 3);

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
    }

    [Test]
    public void should_match_starting_at_end_of_string_ref()
    {
        var re = new PcreRegex(@"(?<=a)");
        var match = re.Match("xxa".AsSpan(), 3);

        match.Success.ShouldBeTrue();
    }

    [Test]
    public void should_match_starting_at_end_of_string_buf()
    {
        var re = new PcreRegex(@"(?<=a)");
        var match = re.CreateMatchBuffer().Match("xxa".AsSpan(), 3);

        match.Success.ShouldBeTrue();
    }

    [Test]
    public void should_match_starting_at_end_of_string_utf8()
    {
        var re = new PcreRegexUtf8(@"(?<=a)"u8);
        var match = re.Match("xxa"u8, 3);

        match.Success.ShouldBeTrue();
    }

    [Test]
    public void should_match_starting_at_end_of_string_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"(?<=a)"u8);
        var match = re.CreateMatchBuffer().Match("xxa"u8, 3);

        match.Success.ShouldBeTrue();
    }

    [Test]
    public void should_match_starting_at_end_of_string_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?<=a)".ToLatin1Bytes());
        var match = re.Match("xxa".ToLatin1Bytes(), 3);

        match.Success.ShouldBeTrue();
    }

    [Test]
    public void should_match_starting_at_end_of_string_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?<=a)".ToLatin1Bytes());
        var match = re.CreateMatchBuffer().Match("xxa".ToLatin1Bytes(), 3);

        match.Success.ShouldBeTrue();
    }

    [Test]
    public void should_handle_named_groups()
    {
        var re = new PcreRegex(@"a+(?<bees>b+)(c+)(?<dees>d+)e+");

        var match = re.Match("xxxaaabbcccddeeezzz");

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.ShouldBe("aaabbcccddeee");
        match.ValueSpan.ShouldBe("aaabbcccddeee");
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(13);

        match["bees"].ShouldNotBeNull();
        match["bees"].Success.ShouldBeTrue();
        match["bees"].IsDefined.ShouldBeTrue();
        match["bees"].Value.ShouldBe("bb");
        match["bees"].ValueSpan.ShouldBe("bb");
        match["bees"].Index.ShouldBe(6);
        match["bees"].Length.ShouldBe(2);

        match.Groups["bees"].ShouldBeSameAs(match["bees"]);

        match[2].ShouldNotBeNull();
        match[2].Value.ShouldBe("ccc");
        match[2].ValueSpan.ShouldBe("ccc");
        match[2].Index.ShouldBe(8);
        match[2].Length.ShouldBe(3);

        match["dees"].ShouldNotBeNull();
        match["dees"].Success.ShouldBeTrue();
        match["dees"].IsDefined.ShouldBeTrue();
        match["dees"].Value.ShouldBe("dd");
        match["dees"].ValueSpan.ShouldBe("dd");
        match["dees"].Index.ShouldBe(11);
        match["dees"].Length.ShouldBe(2);

        match["nope"].ShouldNotBeNull();
        match["nope"].Success.ShouldBeFalse();
        match["nope"].IsDefined.ShouldBeFalse();
        match["nope"].Value.ShouldBeSameAs(string.Empty);
        match["nope"].ValueSpan.Length.ShouldBe(0);
        match["nope"].Index.ShouldBe(-1);
        match["nope"].Length.ShouldBe(0);

        match.TryGetGroup("bees", out var group).ShouldBeTrue();
        group.ShouldBeSameAs(match["bees"]);

        match.TryGetGroup("nope", out group).ShouldBeFalse();
        group.ShouldBeNull();
    }

    [Test]
    public void should_handle_named_groups_ref()
    {
        var re = new PcreRegex(@"a+(?<bees>b+)(c+)(?<dees>d+)e+");

        var match = re.Match("xxxaaabbcccddeeezzz".AsSpan());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.ShouldBe("aaabbcccddeee");
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(13);

        match["bees"].Success.ShouldBeTrue();
        match["bees"].IsDefined.ShouldBeTrue();
        match["bees"].Value.ShouldBe("bb");
        match["bees"].Index.ShouldBe(6);
        match["bees"].Length.ShouldBe(2);

        match[2].Value.ShouldBe("ccc");
        match[2].Index.ShouldBe(8);
        match[2].Length.ShouldBe(3);

        match["dees"].Success.ShouldBeTrue();
        match["dees"].IsDefined.ShouldBeTrue();
        match["dees"].Value.ShouldBe("dd");
        match["dees"].Index.ShouldBe(11);
        match["dees"].Length.ShouldBe(2);

        match["nope"].Success.ShouldBeFalse();
        match["nope"].IsDefined.ShouldBeFalse();
        match["nope"].Value.ToString().ShouldBeSameAs(string.Empty);
        match["nope"].Index.ShouldBe(-1);
        match["nope"].Length.ShouldBe(0);

        match.TryGetGroup("bees", out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.ShouldBe("bb");
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup("nope", out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.ToString().ShouldBeSameAs(string.Empty);
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_handle_named_groups_buf()
    {
        var re = new PcreRegex(@"a+(?<bees>b+)(c+)(?<dees>d+)e+");

        var match = re.CreateMatchBuffer().Match("xxxaaabbcccddeeezzz".AsSpan());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.ShouldBe("aaabbcccddeee");
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(13);

        match["bees"].Success.ShouldBeTrue();
        match["bees"].IsDefined.ShouldBeTrue();
        match["bees"].Value.ShouldBe("bb");
        match["bees"].Index.ShouldBe(6);
        match["bees"].Length.ShouldBe(2);

        match[2].Value.ShouldBe("ccc");
        match[2].Index.ShouldBe(8);
        match[2].Length.ShouldBe(3);

        match["dees"].Success.ShouldBeTrue();
        match["dees"].IsDefined.ShouldBeTrue();
        match["dees"].Value.ShouldBe("dd");
        match["dees"].Index.ShouldBe(11);
        match["dees"].Length.ShouldBe(2);

        match["nope"].Success.ShouldBeFalse();
        match["nope"].IsDefined.ShouldBeFalse();
        match["nope"].Value.ToString().ShouldBeSameAs(string.Empty);
        match["nope"].Index.ShouldBe(-1);
        match["nope"].Length.ShouldBe(0);

        match.TryGetGroup("bees", out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.ShouldBe("bb");
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup("nope", out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.ToString().ShouldBeSameAs(string.Empty);
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_handle_named_groups_utf8()
    {
        var re = new PcreRegexUtf8(@"a+(?<bees>b+)(c+)(?<dees>d+)e+"u8);

        var match = re.Match("xxxaaabbcccddeeezzz"u8);

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.SequenceEqual("aaabbcccddeee"u8).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(13);

        match["bees"].Success.ShouldBeTrue();
        match["bees"].IsDefined.ShouldBeTrue();
        match["bees"].Value.SequenceEqual("bb"u8).ShouldBeTrue();
        match["bees"].Index.ShouldBe(6);
        match["bees"].Length.ShouldBe(2);

        match[2].Value.SequenceEqual("ccc"u8).ShouldBeTrue();
        match[2].Index.ShouldBe(8);
        match[2].Length.ShouldBe(3);

        match["dees"].Success.ShouldBeTrue();
        match["dees"].IsDefined.ShouldBeTrue();
        match["dees"].Value.SequenceEqual("dd"u8).ShouldBeTrue();
        match["dees"].Index.ShouldBe(11);
        match["dees"].Length.ShouldBe(2);

        match["nope"].Success.ShouldBeFalse();
        match["nope"].IsDefined.ShouldBeFalse();
        match["nope"].Value.IsEmpty.ShouldBeTrue();
        match["nope"].Index.ShouldBe(-1);
        match["nope"].Length.ShouldBe(0);

        match.TryGetGroup("bees", out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.SequenceEqual("bb"u8).ShouldBeTrue();
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup("nope", out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.IsEmpty.ShouldBeTrue();
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_handle_named_groups_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"a+(?<bees>b+)(c+)(?<dees>d+)e+"u8);

        var match = re.CreateMatchBuffer().Match("xxxaaabbcccddeeezzz"u8);

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.SequenceEqual("aaabbcccddeee"u8).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(13);

        match["bees"].Success.ShouldBeTrue();
        match["bees"].IsDefined.ShouldBeTrue();
        match["bees"].Value.SequenceEqual("bb"u8).ShouldBeTrue();
        match["bees"].Index.ShouldBe(6);
        match["bees"].Length.ShouldBe(2);

        match[2].Value.SequenceEqual("ccc"u8).ShouldBeTrue();
        match[2].Index.ShouldBe(8);
        match[2].Length.ShouldBe(3);

        match["dees"].Success.ShouldBeTrue();
        match["dees"].IsDefined.ShouldBeTrue();
        match["dees"].Value.SequenceEqual("dd"u8).ShouldBeTrue();
        match["dees"].Index.ShouldBe(11);
        match["dees"].Length.ShouldBe(2);

        match["nope"].Success.ShouldBeFalse();
        match["nope"].IsDefined.ShouldBeFalse();
        match["nope"].Value.IsEmpty.ShouldBeTrue();
        match["nope"].Index.ShouldBe(-1);
        match["nope"].Length.ShouldBe(0);

        match.TryGetGroup("bees", out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.SequenceEqual("bb"u8).ShouldBeTrue();
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup("nope", out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.IsEmpty.ShouldBeTrue();
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_handle_named_groups_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a+(?<bees>b+)(c+)(?<dees>d+)e+".ToLatin1Bytes());

        var match = re.Match("xxxaaabbcccddeeezzz".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.SequenceEqual("aaabbcccddeee".ToLatin1Bytes()).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(13);

        match["bees"].Success.ShouldBeTrue();
        match["bees"].IsDefined.ShouldBeTrue();
        match["bees"].Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        match["bees"].Index.ShouldBe(6);
        match["bees"].Length.ShouldBe(2);

        match[2].Value.SequenceEqual("ccc".ToLatin1Bytes()).ShouldBeTrue();
        match[2].Index.ShouldBe(8);
        match[2].Length.ShouldBe(3);

        match["dees"].Success.ShouldBeTrue();
        match["dees"].IsDefined.ShouldBeTrue();
        match["dees"].Value.SequenceEqual("dd".ToLatin1Bytes()).ShouldBeTrue();
        match["dees"].Index.ShouldBe(11);
        match["dees"].Length.ShouldBe(2);

        match["nope"].Success.ShouldBeFalse();
        match["nope"].IsDefined.ShouldBeFalse();
        match["nope"].Value.IsEmpty.ShouldBeTrue();
        match["nope"].Index.ShouldBe(-1);
        match["nope"].Length.ShouldBe(0);

        match.TryGetGroup("bees", out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup("nope", out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.IsEmpty.ShouldBeTrue();
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_handle_named_groups_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a+(?<bees>b+)(c+)(?<dees>d+)e+".ToLatin1Bytes());

        var match = re.CreateMatchBuffer().Match("xxxaaabbcccddeeezzz".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(3);
        match.Value.SequenceEqual("aaabbcccddeee".ToLatin1Bytes()).ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.Length.ShouldBe(13);

        match["bees"].Success.ShouldBeTrue();
        match["bees"].IsDefined.ShouldBeTrue();
        match["bees"].Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        match["bees"].Index.ShouldBe(6);
        match["bees"].Length.ShouldBe(2);

        match[2].Value.SequenceEqual("ccc".ToLatin1Bytes()).ShouldBeTrue();
        match[2].Index.ShouldBe(8);
        match[2].Length.ShouldBe(3);

        match["dees"].Success.ShouldBeTrue();
        match["dees"].IsDefined.ShouldBeTrue();
        match["dees"].Value.SequenceEqual("dd".ToLatin1Bytes()).ShouldBeTrue();
        match["dees"].Index.ShouldBe(11);
        match["dees"].Length.ShouldBe(2);

        match["nope"].Success.ShouldBeFalse();
        match["nope"].IsDefined.ShouldBeFalse();
        match["nope"].Value.IsEmpty.ShouldBeTrue();
        match["nope"].Index.ShouldBe(-1);
        match["nope"].Length.ShouldBe(0);

        match.TryGetGroup("bees", out var group).ShouldBeTrue();
        group.Success.ShouldBeTrue();
        group.IsDefined.ShouldBeTrue();
        group.Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        group.Index.ShouldBe(6);
        group.Length.ShouldBe(2);

        match.TryGetGroup("nope", out group).ShouldBeFalse();
        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.IsEmpty.ShouldBeTrue();
        group.Index.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_handle_case_sensitive_group_names()
    {
        var re = new PcreRegex(@"a+(?<grp>b+)(?<GRP>c+)(?<GrP>d+)e+");

        var match = re.Match("xxxaaabbcccddeeezzz");

        match["grp"].ShouldNotBeNull();
        match["grp"].Value.ShouldBe("bb");
        match["grp"].ValueSpan.ShouldBe("bb");
        match["grp"].Index.ShouldBe(6);
        match["grp"].Length.ShouldBe(2);

        match["GRP"].ShouldNotBeNull();
        match["GRP"].Value.ShouldBe("ccc");
        match["GRP"].ValueSpan.ShouldBe("ccc");
        match["GRP"].Index.ShouldBe(8);
        match["GRP"].Length.ShouldBe(3);

        match["GrP"].ShouldNotBeNull();
        match["GrP"].Value.ShouldBe("dd");
        match["GrP"].ValueSpan.ShouldBe("dd");
        match["GrP"].Index.ShouldBe(11);
        match["GrP"].Length.ShouldBe(2);
    }

    [Test]
    public void should_handle_case_sensitive_group_names_ref()
    {
        var re = new PcreRegex(@"a+(?<grp>b+)(?<GRP>c+)(?<GrP>d+)e+");

        var match = re.Match("xxxaaabbcccddeeezzz".AsSpan());

        match["grp"].Value.ShouldBe("bb");
        match["grp"].Index.ShouldBe(6);
        match["grp"].Length.ShouldBe(2);

        match["GRP"].Value.ShouldBe("ccc");
        match["GRP"].Index.ShouldBe(8);
        match["GRP"].Length.ShouldBe(3);

        match["GrP"].Value.ShouldBe("dd");
        match["GrP"].Index.ShouldBe(11);
        match["GrP"].Length.ShouldBe(2);
    }

    [Test]
    public void should_handle_case_sensitive_group_names_buf()
    {
        var re = new PcreRegex(@"a+(?<grp>b+)(?<GRP>c+)(?<GrP>d+)e+");

        var match = re.CreateMatchBuffer().Match("xxxaaabbcccddeeezzz".AsSpan());

        match["grp"].Value.ShouldBe("bb");
        match["grp"].Index.ShouldBe(6);
        match["grp"].Length.ShouldBe(2);

        match["GRP"].Value.ShouldBe("ccc");
        match["GRP"].Index.ShouldBe(8);
        match["GRP"].Length.ShouldBe(3);

        match["GrP"].Value.ShouldBe("dd");
        match["GrP"].Index.ShouldBe(11);
        match["GrP"].Length.ShouldBe(2);
    }

    [Test]
    public void should_handle_case_sensitive_group_names_utf8()
    {
        var re = new PcreRegexUtf8(@"a+(?<grp>b+)(?<GRP>c+)(?<GrP>d+)e+"u8);

        var match = re.Match("xxxaaabbcccddeeezzz"u8);

        match["grp"].Value.SequenceEqual("bb"u8).ShouldBeTrue();
        match["grp"].Index.ShouldBe(6);
        match["grp"].Length.ShouldBe(2);

        match["GRP"].Value.SequenceEqual("ccc"u8).ShouldBeTrue();
        match["GRP"].Index.ShouldBe(8);
        match["GRP"].Length.ShouldBe(3);

        match["GrP"].Value.SequenceEqual("dd"u8).ShouldBeTrue();
        match["GrP"].Index.ShouldBe(11);
        match["GrP"].Length.ShouldBe(2);
    }

    [Test]
    public void should_handle_case_sensitive_group_names_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"a+(?<grp>b+)(?<GRP>c+)(?<GrP>d+)e+"u8);

        var match = re.CreateMatchBuffer().Match("xxxaaabbcccddeeezzz"u8);

        match["grp"].Value.SequenceEqual("bb"u8).ShouldBeTrue();
        match["grp"].Index.ShouldBe(6);
        match["grp"].Length.ShouldBe(2);

        match["GRP"].Value.SequenceEqual("ccc"u8).ShouldBeTrue();
        match["GRP"].Index.ShouldBe(8);
        match["GRP"].Length.ShouldBe(3);

        match["GrP"].Value.SequenceEqual("dd"u8).ShouldBeTrue();
        match["GrP"].Index.ShouldBe(11);
        match["GrP"].Length.ShouldBe(2);
    }

    [Test]
    public void should_handle_case_sensitive_group_names_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a+(?<grp>b+)(?<GRP>c+)(?<GrP>d+)e+".ToLatin1Bytes());

        var match = re.Match("xxxaaabbcccddeeezzz".ToLatin1Bytes());

        match["grp"].Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        match["grp"].Index.ShouldBe(6);
        match["grp"].Length.ShouldBe(2);

        match["GRP"].Value.SequenceEqual("ccc".ToLatin1Bytes()).ShouldBeTrue();
        match["GRP"].Index.ShouldBe(8);
        match["GRP"].Length.ShouldBe(3);

        match["GrP"].Value.SequenceEqual("dd".ToLatin1Bytes()).ShouldBeTrue();
        match["GrP"].Index.ShouldBe(11);
        match["GrP"].Length.ShouldBe(2);
    }

    [Test]
    public void should_handle_case_sensitive_group_names_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a+(?<grp>b+)(?<GRP>c+)(?<GrP>d+)e+".ToLatin1Bytes());

        var match = re.CreateMatchBuffer().Match("xxxaaabbcccddeeezzz".ToLatin1Bytes());

        match["grp"].Value.SequenceEqual("bb".ToLatin1Bytes()).ShouldBeTrue();
        match["grp"].Index.ShouldBe(6);
        match["grp"].Length.ShouldBe(2);

        match["GRP"].Value.SequenceEqual("ccc".ToLatin1Bytes()).ShouldBeTrue();
        match["GRP"].Index.ShouldBe(8);
        match["GRP"].Length.ShouldBe(3);

        match["GrP"].Value.SequenceEqual("dd".ToLatin1Bytes()).ShouldBeTrue();
        match["GrP"].Index.ShouldBe(11);
        match["GrP"].Length.ShouldBe(2);
    }

    [Test]
    public void should_allow_duplicate_names()
    {
        var re = new PcreRegex(@"(?<g>a)?(?<g>b)?(?<g>c)?", PcreOptions.DupNames);
        var match = re.Match("b");

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        match["g"].Value.ShouldBe("b");
        match["g"].ValueSpan.ShouldBe("b");

        match.GetDuplicateNamedGroups("g").Select(g => g.Success).ShouldBe([false, true, false]);

        match = re.Match("bc");
        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        match["g"].Value.ShouldBe("b");
        match["g"].ValueSpan.ShouldBe("b");

        match.GetDuplicateNamedGroups("g").Select(g => g.Success).ShouldBe([false, true, true]);
    }

    [Test]
    public void should_allow_duplicate_names_ref()
    {
        var re = new PcreRegex(@"(?<g>a)?(?<g>b)?(?<g>c)?", PcreOptions.DupNames);
        var match = re.Match("b".AsSpan());

        match.Success.ShouldBeTrue();
        match["g"].Value.ShouldBe("b");

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, false]);

        match = re.Match("bc".AsSpan());
        match.Success.ShouldBeTrue();
        match["g"].Value.ShouldBe("b");

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    [Test]
    public void should_allow_duplicate_names_buf()
    {
        var re = new PcreRegex(@"(?<g>a)?(?<g>b)?(?<g>c)?", PcreOptions.DupNames);
        var match = re.CreateMatchBuffer().Match("b".AsSpan());

        match.Success.ShouldBeTrue();
        match["g"].Value.ShouldBe("b");

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, false]);

        match = re.Match("bc".AsSpan());
        match.Success.ShouldBeTrue();
        match["g"].Value.ShouldBe("b");

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    [Test]
    public void should_allow_duplicate_names_utf8()
    {
        var re = new PcreRegexUtf8(@"(?<g>a)?(?<g>b)?(?<g>c)?"u8, PcreOptions.DupNames);
        var match = re.Match("b"u8);

        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b"u8).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, false]);

        match = re.Match("bc"u8);
        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b"u8).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    [Test]
    public void should_allow_duplicate_names_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"(?<g>a)?(?<g>b)?(?<g>c)?"u8, PcreOptions.DupNames);
        var match = re.CreateMatchBuffer().Match("b"u8);

        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b"u8).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, false]);

        match = re.Match("bc"u8);
        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b"u8).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    [Test]
    public void should_allow_duplicate_names_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?<g>a)?(?<g>b)?(?<g>c)?".ToLatin1Bytes(), PcreOptions.DupNames);
        var match = re.Match("b".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b".ToLatin1Bytes()).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, false]);

        match = re.Match("bc".ToLatin1Bytes());
        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b".ToLatin1Bytes()).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    [Test]
    public void should_allow_duplicate_names_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?<g>a)?(?<g>b)?(?<g>c)?".ToLatin1Bytes(), PcreOptions.DupNames);
        var match = re.CreateMatchBuffer().Match("b".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b".ToLatin1Bytes()).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, false]);

        match = re.Match("bc".ToLatin1Bytes());
        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b".ToLatin1Bytes()).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    [Test]
    public void should_detect_duplicate_names()
    {
        var re = new PcreRegex(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?");

        var match = re.Match("bc");
        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        match["g"].Value.ShouldBe("b");

        match.GetDuplicateNamedGroups("g").Select(g => g.Success).ShouldBe([false, true, true]);
    }

    [Test]
    public void should_detect_duplicate_names_ref()
    {
        var re = new PcreRegex(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?");

        var match = re.Match("bc".AsSpan());
        match.Success.ShouldBeTrue();
        match["g"].Value.ShouldBe("b");

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    [Test]
    public void should_detect_duplicate_names_buf()
    {
        var re = new PcreRegex(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?");

        var match = re.CreateMatchBuffer().Match("bc".AsSpan());
        match.Success.ShouldBeTrue();
        match["g"].Value.ShouldBe("b");

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    [Test]
    public void should_detect_duplicate_names_utf8()
    {
        var re = new PcreRegexUtf8(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?"u8);

        var match = re.Match("bc"u8);
        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b"u8).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    [Test]
    public void should_detect_duplicate_names_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?"u8);

        var match = re.CreateMatchBuffer().Match("bc"u8);
        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b"u8).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    [Test]
    public void should_detect_duplicate_names_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?".ToLatin1Bytes());

        var match = re.Match("bc".ToLatin1Bytes());
        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b".ToLatin1Bytes()).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    [Test]
    public void should_detect_duplicate_names_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?".ToLatin1Bytes());

        var match = re.CreateMatchBuffer().Match("bc".ToLatin1Bytes());
        match.Success.ShouldBeTrue();
        match["g"].Value.SequenceEqual("b".ToLatin1Bytes()).ShouldBeTrue();

        GetDuplicateNamedGroupsSuccesses(match, "g").ShouldBe([false, true, true]);
    }

    private static List<bool> GetDuplicateNamedGroupsSuccesses(PcreRefMatch match, string groupName)
        => match.GetDuplicateNamedGroups(groupName).ToList(i => i.Success);

    private static List<bool> GetDuplicateNamedGroupsSuccesses(PcreRefMatch8Bit match, string groupName)
        => match.GetDuplicateNamedGroups(groupName).ToList(i => i.Success);

    [Test]
    public void should_return_value_span_from_subject_string()
    {
        var subject = string.Concat("foo", "bar");
        var re = new PcreRegex(@"b(a)(r)");

        var match = re.Match(subject);

        ref var subjectRef = ref MemoryMarshal.GetReference(subject.AsSpan(3));
        ref var valueRef = ref MemoryMarshal.GetReference(match.ValueSpan);
        Unsafe.AreSame(ref valueRef, ref subjectRef).ShouldBeTrue();

        _ = match.Value; // Reading the string value shouldn't change the span target

        valueRef = ref MemoryMarshal.GetReference(match.ValueSpan);
        Unsafe.AreSame(ref valueRef, ref subjectRef).ShouldBeTrue();
    }

    [Test]
    public void should_return_value_span_from_subject_reference_utf8()
    {
        var subject = Encoding.UTF8.GetBytes("foo" + "bar");
        var re = new PcreRegexUtf8(@"b(a)(r)"u8);

        var match = re.Match(subject);

        ref var subjectRef = ref MemoryMarshal.GetReference(subject.AsSpan(3));
        ref var valueRef = ref MemoryMarshal.GetReference(match.Value);
        Unsafe.AreSame(ref valueRef, ref subjectRef).ShouldBeTrue();
    }

    [Test]
    public void should_return_value_span_from_subject_reference_8bit()
    {
        var subject = "foobar".ToLatin1Bytes();
        var re = new PcreRegex8Bit(@"b(a)(r)".ToLatin1Bytes(), TestSupport.Latin1Encoding);

        var match = re.Match(subject);

        ref var subjectRef = ref MemoryMarshal.GetReference(subject.AsSpan(3));
        ref var valueRef = ref MemoryMarshal.GetReference(match.Value);
        Unsafe.AreSame(ref valueRef, ref subjectRef).ShouldBeTrue();
    }

    [Test]
    public void should_return_marks()
    {
        var re = new PcreRegex(@"a(?:(*MARK:foo)b(*MARK:bar)|c)");
        var match = re.Match("ab");

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        match.Mark.ShouldBe("bar");

        match = re.Match("ac");
        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        match.Mark.ShouldBeNull();
    }

    [Test]
    public void should_return_marks_ref()
    {
        var re = new PcreRegex(@"a(?:(*MARK:foo)b(*MARK:bar)|c)");
        var match = re.Match("ab".AsSpan());

        match.Success.ShouldBeTrue();
        match.Mark.ShouldBe("bar");

        match = re.Match("ac".AsSpan());
        match.Success.ShouldBeTrue();
        match.Mark.ShouldBe(string.Empty);
    }

    [Test]
    public void should_return_marks_buf()
    {
        var re = new PcreRegex(@"a(?:(*MARK:foo)b(*MARK:bar)|c)");
        var match = re.CreateMatchBuffer().Match("ab".AsSpan());

        match.Success.ShouldBeTrue();
        match.Mark.ShouldBe("bar");

        match = re.Match("ac".AsSpan());
        match.Success.ShouldBeTrue();
        match.Mark.ShouldBe(string.Empty);
    }

    [Test]
    public void should_return_marks_utf8()
    {
        var re = new PcreRegexUtf8(@"a(?:(*MARK:foo)b(*MARK:bar)|c)"u8);
        var match = re.Match("ab"u8);

        match.Success.ShouldBeTrue();
        match.Mark.SequenceEqual("bar"u8).ShouldBeTrue();

        match = re.Match("ac"u8);
        match.Success.ShouldBeTrue();
        match.Mark.IsEmpty.ShouldBeTrue();
    }

    [Test]
    public void should_return_marks_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"a(?:(*MARK:foo)b(*MARK:bar)|c)"u8);
        var match = re.CreateMatchBuffer().Match("ab"u8);

        match.Success.ShouldBeTrue();
        match.Mark.SequenceEqual("bar"u8).ShouldBeTrue();

        match = re.Match("ac"u8);
        match.Success.ShouldBeTrue();
        match.Mark.IsEmpty.ShouldBeTrue();
    }

    [Test]
    public void should_return_marks_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a(?:(*MARK:foo)b(*MARK:bar)|c)".ToLatin1Bytes());
        var match = re.Match("ab".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match.Mark.SequenceEqual("bar".ToLatin1Bytes()).ShouldBeTrue();

        match = re.Match("ac".ToLatin1Bytes());
        match.Success.ShouldBeTrue();
        match.Mark.IsEmpty.ShouldBeTrue();
    }

    [Test]
    public void should_return_marks_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a(?:(*MARK:foo)b(*MARK:bar)|c)".ToLatin1Bytes());
        var match = re.CreateMatchBuffer().Match("ab".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match.Mark.SequenceEqual("bar".ToLatin1Bytes()).ShouldBeTrue();

        match = re.Match("ac".ToLatin1Bytes());
        match.Success.ShouldBeTrue();
        match.Mark.IsEmpty.ShouldBeTrue();
    }

    [Test]
    public void should_use_callout_result()
    {
        var regex = new PcreRegex(@"(\d+)(*SKIP)(?C1):\s*(\w+)");

        var match = regex.Match(
            "1542: not_this, 1764: hello",
            data => data.Number == 1
                    && int.Parse(data.Match[1].Value) % 42 == 0
                ? PcreCalloutResult.Pass
                : PcreCalloutResult.Fail);

        match[2].Value.ShouldBe("hello");
    }

    [Test]
    public void should_use_callout_result_ref()
    {
        var regex = new PcreRegex(@"(\d+)(*SKIP)(?C1):\s*(\w+)");

        var match = regex.Match(
            "1542: not_this, 1764: hello".AsSpan(),
            data => data.Number == 1
                    && int.Parse(data.Match[1].Value.ToString()) % 42 == 0
                ? PcreCalloutResult.Pass
                : PcreCalloutResult.Fail);

        match[2].Value.ShouldBe("hello");
    }

    [Test]
    public void should_use_callout_result_buf()
    {
        var regex = new PcreRegex(@"(\d+)(*SKIP)(?C1):\s*(\w+)");

        var match = regex.CreateMatchBuffer().Match(
            "1542: not_this, 1764: hello".AsSpan(),
            data => data.Number == 1
                    && int.Parse(data.Match[1].Value.ToString()) % 42 == 0
                ? PcreCalloutResult.Pass
                : PcreCalloutResult.Fail);

        match[2].Value.ShouldBe("hello");
    }

    [Test]
    public void should_use_callout_result_utf8()
    {
        var regex = new PcreRegexUtf8(@"(\d+)(*SKIP)(?C1):\s*(\w+)"u8);

        var match = regex.Match(
            "1542: not_this, 1764: hello"u8,
            data => data.Number == 1
                    && int.Parse(data.Match[1].ToString()) % 42 == 0
                ? PcreCalloutResult.Pass
                : PcreCalloutResult.Fail);

        match[2].Value.SequenceEqual("hello"u8).ShouldBeTrue();
    }

    [Test]
    public void should_use_callout_result_buf_utf8()
    {
        var regex = new PcreRegexUtf8(@"(\d+)(*SKIP)(?C1):\s*(\w+)"u8);

        var match = regex.CreateMatchBuffer().Match(
            "1542: not_this, 1764: hello"u8,
            data => data.Number == 1
                    && int.Parse(data.Match[1].ToString()) % 42 == 0
                ? PcreCalloutResult.Pass
                : PcreCalloutResult.Fail);

        match[2].Value.SequenceEqual("hello"u8).ShouldBeTrue();
    }

    [Test]
    public void should_use_callout_result_8bit()
    {
        var regex = TestSupport.CreatePcreRegex8Bit(@"(\d+)(*SKIP)(?C1):\s*(\w+)".ToLatin1Bytes());

        var match = regex.Match(
            "1542: not_this, 1764: hello".ToLatin1Bytes(),
            data => data.Number == 1
                    && int.Parse(data.Match[1].ToString()) % 42 == 0
                ? PcreCalloutResult.Pass
                : PcreCalloutResult.Fail);

        match[2].Value.SequenceEqual("hello".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    public void should_use_callout_result_buf_8bit()
    {
        var regex = TestSupport.CreatePcreRegex8Bit(@"(\d+)(*SKIP)(?C1):\s*(\w+)".ToLatin1Bytes());

        var match = regex.CreateMatchBuffer().Match(
            "1542: not_this, 1764: hello".ToLatin1Bytes(),
            data => data.Number == 1
                    && int.Parse(data.Match[1].ToString()) % 42 == 0
                ? PcreCalloutResult.Pass
                : PcreCalloutResult.Fail);

        match[2].Value.SequenceEqual("hello".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    public void should_execute_passing_callout()
    {
        const string pattern = @"(a)(*MARK:foo)(x)?(?C42)(bc)";
        var re = new PcreRegex(pattern);

        var calls = 0;

        var match = re.Match("abc", data =>
        {
            data.Number.ShouldBe(42);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(pattern.IndexOf("(?C42)", StringComparison.Ordinal) + 6);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(0);
            data.String.ShouldBeNull();

            data.Match.Value.ShouldBe("a");
            data.Match[1].Value.ShouldBe("a");
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.ShouldBeSameAs(string.Empty);
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.ShouldBeSameAs(string.Empty);

            data.Match.Mark.ShouldBe("foo");

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_passing_callout_ref()
    {
        const string pattern = @"(a)(*MARK:foo)(x)?(?C42)(bc)";
        var re = new PcreRegex(pattern);

        var calls = 0;

        var match = re.Match("abc".AsSpan(), data =>
        {
            data.Number.ShouldBe(42);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(pattern.IndexOf("(?C42)", StringComparison.Ordinal) + 6);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(0);
            data.String.ShouldBeNull();

            data.Match.Value.ShouldBe("a");
            data.Match[1].Value.ShouldBe("a");
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.ToString().ShouldBeSameAs(string.Empty);
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.ToString().ShouldBeSameAs(string.Empty);

            data.Match.Mark.ShouldBe("foo");

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_passing_callout_buf()
    {
        const string pattern = @"(a)(*MARK:foo)(x)?(?C42)(bc)";
        var re = new PcreRegex(pattern);

        var calls = 0;

        var match = re.CreateMatchBuffer().Match("abc".AsSpan(), data =>
        {
            data.Number.ShouldBe(42);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(pattern.IndexOf("(?C42)", StringComparison.Ordinal) + 6);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(0);
            data.String.ShouldBeNull();

            data.Match.Value.ShouldBe("a");
            data.Match[1].Value.ShouldBe("a");
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.ToString().ShouldBeSameAs(string.Empty);
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.ToString().ShouldBeSameAs(string.Empty);

            data.Match.Mark.ShouldBe("foo");

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_passing_callout_utf8()
    {
        static ReadOnlySpan<byte> GetPattern() => @"(a)(*MARK:foo)(x)?(?C42)(bc)"u8;
        var re = new PcreRegexUtf8(GetPattern());

        var calls = 0;

        var match = re.Match(
            "abc"u8,
            data =>
            {
                data.Number.ShouldBe(42);
                data.CurrentOffset.ShouldBe(1);
                data.PatternPosition.ShouldBe(GetPattern().IndexOf("(?C42)"u8) + 6);
                data.StartOffset.ShouldBe(0);
                data.LastCapture.ShouldBe(1);
                data.MaxCapture.ShouldBe(2);
                data.NextPatternItemLength.ShouldBe(1);
                data.StringOffset.ShouldBe(0);
                data.String.ShouldBeNull();

                data.Match.Value.SequenceEqual("a"u8).ShouldBeTrue();
                data.Match[1].Value.SequenceEqual("a"u8).ShouldBeTrue();
                data.Match[2].Success.ShouldBeFalse();
                data.Match[2].Value.IsEmpty.ShouldBeTrue();
                data.Match[3].Success.ShouldBeFalse();
                data.Match[3].Value.IsEmpty.ShouldBeTrue();

                data.Match.Mark.SequenceEqual("foo"u8).ShouldBeTrue();

                ++calls;
                return PcreCalloutResult.Pass;
            }
        );

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_passing_callout_buf_utf8()
    {
        static ReadOnlySpan<byte> GetPattern() => @"(a)(*MARK:foo)(x)?(?C42)(bc)"u8;
        var re = new PcreRegexUtf8(GetPattern());

        var calls = 0;

        var match = re.CreateMatchBuffer().Match("abc"u8, data =>
        {
            data.Number.ShouldBe(42);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(GetPattern().IndexOf("(?C42)"u8) + 6);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(0);
            data.String.ShouldBeNull();

            data.Match.Value.SequenceEqual("a"u8).ShouldBeTrue();
            data.Match[1].Value.SequenceEqual("a"u8).ShouldBeTrue();
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.IsEmpty.ShouldBeTrue();
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.IsEmpty.ShouldBeTrue();

            data.Match.Mark.SequenceEqual("foo"u8).ShouldBeTrue();

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_passing_callout_8bit()
    {
        static ReadOnlySpan<byte> GetPattern() => @"(a)(*MARK:foo)(x)?(?C42)(bc)".ToLatin1Bytes();
        var re = TestSupport.CreatePcreRegex8Bit(GetPattern());

        var calls = 0;

        var match = re.Match(
            "abc".ToLatin1Bytes(),
            data =>
            {
                data.Number.ShouldBe(42);
                data.CurrentOffset.ShouldBe(1);
                data.PatternPosition.ShouldBe(GetPattern().IndexOf("(?C42)".ToLatin1Bytes()) + 6);
                data.StartOffset.ShouldBe(0);
                data.LastCapture.ShouldBe(1);
                data.MaxCapture.ShouldBe(2);
                data.NextPatternItemLength.ShouldBe(1);
                data.StringOffset.ShouldBe(0);
                data.String.ShouldBeNull();

                data.Match.Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
                data.Match[1].Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
                data.Match[2].Success.ShouldBeFalse();
                data.Match[2].Value.IsEmpty.ShouldBeTrue();
                data.Match[3].Success.ShouldBeFalse();
                data.Match[3].Value.IsEmpty.ShouldBeTrue();

                data.Match.Mark.SequenceEqual("foo".ToLatin1Bytes()).ShouldBeTrue();

                ++calls;
                return PcreCalloutResult.Pass;
            }
        );

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_passing_callout_buf_8bit()
    {
        static ReadOnlySpan<byte> GetPattern() => @"(a)(*MARK:foo)(x)?(?C42)(bc)".ToLatin1Bytes();
        var re = TestSupport.CreatePcreRegex8Bit(GetPattern());

        var calls = 0;

        var match = re.CreateMatchBuffer().Match("abc".ToLatin1Bytes(), data =>
        {
            data.Number.ShouldBe(42);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(GetPattern().IndexOf("(?C42)".ToLatin1Bytes()) + 6);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(0);
            data.String.ShouldBeNull();

            data.Match.Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
            data.Match[1].Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.IsEmpty.ShouldBeTrue();
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.IsEmpty.ShouldBeTrue();

            data.Match.Mark.SequenceEqual("foo".ToLatin1Bytes()).ShouldBeTrue();

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_failing_callout()
    {
        var re = new PcreRegex(@".(?C42)");

        var first = true;

        var match = re.Match("ab", data =>
        {
            data.Number.ShouldBe(42);
            if (first)
            {
                first = false;
                return PcreCalloutResult.Fail;
            }

            return PcreCalloutResult.Pass;
        });

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        match.Value.ShouldBe("b");
    }

    [Test]
    public void should_execute_failing_callout_ref()
    {
        var re = new PcreRegex(@".(?C42)");

        var first = true;

        var match = re.Match("ab".AsSpan(), data =>
        {
            data.Number.ShouldBe(42);
            if (first)
            {
                first = false;
                return PcreCalloutResult.Fail;
            }

            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        match.Value.ShouldBe("b");
    }

    [Test]
    public void should_execute_failing_callout_buf()
    {
        var re = new PcreRegex(@".(?C42)");

        var first = true;

        var match = re.CreateMatchBuffer().Match("ab".AsSpan(), data =>
        {
            data.Number.ShouldBe(42);
            if (first)
            {
                first = false;
                return PcreCalloutResult.Fail;
            }

            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        match.Value.ShouldBe("b");
    }

    [Test]
    public void should_execute_failing_callout_utf8()
    {
        var re = new PcreRegexUtf8(@".(?C42)"u8);

        var first = true;

        var match = re.Match("ab"u8, data =>
        {
            data.Number.ShouldBe(42);
            if (first)
            {
                first = false;
                return PcreCalloutResult.Fail;
            }

            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        match.Value.SequenceEqual("b"u8).ShouldBeTrue();
    }

    [Test]
    public void should_execute_failing_callout_buf_utf8()
    {
        var re = new PcreRegexUtf8(@".(?C42)"u8);

        var first = true;

        var match = re.CreateMatchBuffer().Match("ab"u8, data =>
        {
            data.Number.ShouldBe(42);
            if (first)
            {
                first = false;
                return PcreCalloutResult.Fail;
            }

            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        match.Value.SequenceEqual("b"u8).ShouldBeTrue();
    }

    [Test]
    public void should_execute_failing_callout_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@".(?C42)".ToLatin1Bytes());

        var first = true;

        var match = re.Match("ab".ToLatin1Bytes(), data =>
        {
            data.Number.ShouldBe(42);
            if (first)
            {
                first = false;
                return PcreCalloutResult.Fail;
            }

            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        match.Value.SequenceEqual("b".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    public void should_execute_failing_callout_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@".(?C42)".ToLatin1Bytes());

        var first = true;

        var match = re.CreateMatchBuffer().Match("ab".ToLatin1Bytes(), data =>
        {
            data.Number.ShouldBe(42);
            if (first)
            {
                first = false;
                return PcreCalloutResult.Fail;
            }

            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        match.Value.SequenceEqual("b".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    public void should_execute_aborting_callout()
    {
        var re = new PcreRegex(@".(?C42)");

        var match = re.Match("ab", data =>
        {
            data.Number.ShouldBe(42);
            return PcreCalloutResult.Abort;
        });

        match.ShouldNotBeNull();
        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_execute_aborting_callout_ref()
    {
        var re = new PcreRegex(@".(?C42)");

        var match = re.Match("ab".AsSpan(), data =>
        {
            data.Number.ShouldBe(42);
            return PcreCalloutResult.Abort;
        });

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_execute_aborting_callout_buf()
    {
        var re = new PcreRegex(@".(?C42)");

        var match = re.CreateMatchBuffer().Match("ab".AsSpan(), data =>
        {
            data.Number.ShouldBe(42);
            return PcreCalloutResult.Abort;
        });

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_execute_aborting_callout_utf8()
    {
        var re = new PcreRegexUtf8(@".(?C42)"u8);

        var match = re.Match("ab"u8, data =>
        {
            data.Number.ShouldBe(42);
            return PcreCalloutResult.Abort;
        });

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_execute_aborting_callout_buf_utf8()
    {
        var re = new PcreRegexUtf8(@".(?C42)"u8);

        var match = re.CreateMatchBuffer().Match("ab"u8, data =>
        {
            data.Number.ShouldBe(42);
            return PcreCalloutResult.Abort;
        });

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_execute_aborting_callout_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@".(?C42)".ToLatin1Bytes());

        var match = re.Match("ab".ToLatin1Bytes(), data =>
        {
            data.Number.ShouldBe(42);
            return PcreCalloutResult.Abort;
        });

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_execute_aborting_callout_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@".(?C42)".ToLatin1Bytes());

        var match = re.CreateMatchBuffer().Match("ab".ToLatin1Bytes(), data =>
        {
            data.Number.ShouldBe(42);
            return PcreCalloutResult.Abort;
        });

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_throw_when_callout_throws()
    {
        var re = new PcreRegex(@".(?C42)");

        var ex = Should.Throw<PcreCalloutException>(() => re.Match("ab", _ => throw new DivideByZeroException("test")));

        ex.ErrorCode.ShouldBe(PcreErrorCode.Callout);
        ex.InnerException.ShouldBeOfType<DivideByZeroException>();
    }

    [Test]
    public void should_throw_when_callout_throws_ref()
    {
        var re = new PcreRegex(@".(?C42)");

        var ex = Should.Throw<PcreCalloutException>(() => re.Match("ab".AsSpan(), _ => throw new DivideByZeroException("test")));

        ex.ErrorCode.ShouldBe(PcreErrorCode.Callout);
        ex.InnerException.ShouldBeOfType<DivideByZeroException>();
    }

    [Test]
    public void should_throw_when_callout_throws_buf()
    {
        var re = new PcreRegex(@".(?C42)");

        var buffer = re.CreateMatchBuffer();
        var ex = Should.Throw<PcreCalloutException>(() => buffer.Match("ab".AsSpan(), _ => throw new DivideByZeroException("test")));

        ex.ErrorCode.ShouldBe(PcreErrorCode.Callout);
        ex.InnerException.ShouldBeOfType<DivideByZeroException>();
    }

    [Test]
    public void should_throw_when_callout_throws_utf8()
    {
        var re = new PcreRegexUtf8(@".(?C42)"u8);

        var ex = Should.Throw<PcreCalloutException>(() => re.Match("ab"u8, _ => throw new DivideByZeroException("test")));

        ex.ErrorCode.ShouldBe(PcreErrorCode.Callout);
        ex.InnerException.ShouldBeOfType<DivideByZeroException>();
    }

    [Test]
    public void should_throw_when_callout_throws_buf_utf8()
    {
        var re = new PcreRegexUtf8(@".(?C42)"u8);

        var buffer = re.CreateMatchBuffer();
        var ex = Should.Throw<PcreCalloutException>(() => buffer.Match("ab"u8, _ => throw new DivideByZeroException("test")));

        ex.ErrorCode.ShouldBe(PcreErrorCode.Callout);
        ex.InnerException.ShouldBeOfType<DivideByZeroException>();
    }

    [Test]
    public void should_throw_when_callout_throws_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@".(?C42)".ToLatin1Bytes());

        var ex = Should.Throw<PcreCalloutException>(() => re.Match("ab".ToLatin1Bytes(), _ => throw new DivideByZeroException("test")));

        ex.ErrorCode.ShouldBe(PcreErrorCode.Callout);
        ex.InnerException.ShouldBeOfType<DivideByZeroException>();
    }

    [Test]
    public void should_throw_when_callout_throws_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@".(?C42)".ToLatin1Bytes());

        var buffer = re.CreateMatchBuffer();
        var ex = Should.Throw<PcreCalloutException>(() => buffer.Match("ab".ToLatin1Bytes(), _ => throw new DivideByZeroException("test")));

        ex.ErrorCode.ShouldBe(PcreErrorCode.Callout);
        ex.InnerException.ShouldBeOfType<DivideByZeroException>();
    }

    [Test]
    public void should_auto_callout()
    {
        var re = new PcreRegex(@"a.c", PcreOptions.AutoCallout);

        var count = 0;

        var match = re.Match("abc", data =>
        {
            data.Number.ShouldBe(255);
            ++count;
            return PcreCalloutResult.Pass;
        });

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        count.ShouldBe(4);
    }

    [Test]
    public void should_auto_callout_ref()
    {
        var re = new PcreRegex(@"a.c", PcreOptions.AutoCallout);

        var count = 0;

        var match = re.Match("abc".AsSpan(), data =>
        {
            data.Number.ShouldBe(255);
            ++count;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        count.ShouldBe(4);
    }

    [Test]
    public void should_auto_callout_buf()
    {
        var re = new PcreRegex(@"a.c", PcreOptions.AutoCallout);

        var count = 0;

        var match = re.CreateMatchBuffer().Match("abc".AsSpan(), data =>
        {
            data.Number.ShouldBe(255);
            ++count;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        count.ShouldBe(4);
    }

    [Test]
    public void should_auto_callout_utf8()
    {
        var re = new PcreRegexUtf8(@"a.c"u8, PcreOptions.AutoCallout);

        var count = 0;

        var match = re.Match("abc"u8, data =>
        {
            data.Number.ShouldBe(255);
            ++count;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        count.ShouldBe(4);
    }

    [Test]
    public void should_auto_callout_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"a.c"u8, PcreOptions.AutoCallout);

        var count = 0;

        var match = re.CreateMatchBuffer().Match("abc"u8, data =>
        {
            data.Number.ShouldBe(255);
            ++count;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        count.ShouldBe(4);
    }

    [Test]
    public void should_auto_callout_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a.c".ToLatin1Bytes(), PcreOptions.AutoCallout);

        var count = 0;

        var match = re.Match("abc".ToLatin1Bytes(), data =>
        {
            data.Number.ShouldBe(255);
            ++count;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        count.ShouldBe(4);
    }

    [Test]
    public void should_auto_callout_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a.c".ToLatin1Bytes(), PcreOptions.AutoCallout);

        var count = 0;

        var match = re.CreateMatchBuffer().Match("abc".ToLatin1Bytes(), data =>
        {
            data.Number.ShouldBe(255);
            ++count;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        count.ShouldBe(4);
    }

    [Test]
    public void should_get_info_for_auto_callouts()
    {
        var re = new PcreRegex(@"a.c", PcreOptions.AutoCallout);

        var count = 0;

        var match = re.Match("abc", data =>
        {
            ++count;
            data.Info.ShouldNotBeNull();
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        count.ShouldBe(4);
    }

    [Test]
    public void should_get_info_for_auto_callouts_utf8()
    {
        var re = new PcreRegexUtf8(@"a.c"u8, PcreOptions.AutoCallout);

        var count = 0;

        var match = re.Match("abc"u8, data =>
        {
            ++count;
            data.Info.ShouldNotBeNull();
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        count.ShouldBe(4);
    }

    [Test]
    public void should_get_info_for_auto_callouts_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a.c".ToLatin1Bytes(), PcreOptions.AutoCallout);

        var count = 0;

        var match = re.Match("abc".ToLatin1Bytes(), data =>
        {
            ++count;
            data.Info.ShouldNotBeNull();
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        count.ShouldBe(4);
    }

    [Test]
    public void should_provide_callout_flags()
    {
        var re = new PcreRegex(@"a(?C1)(?:(?C2)(*FAIL)|b)(?C3)");

        var startMatchList = new List<bool>();
        var backtrackList = new List<bool>();

        re.Match("abc", data =>
        {
            startMatchList.Add(data.StartMatch);
            backtrackList.Add(data.Backtrack);
            return PcreCalloutResult.Pass;
        });

        startMatchList.ShouldBe([true, false, false]);
        backtrackList.ShouldBe([false, false, true]);
    }

    [Test]
    public void should_provide_callout_flags_ref()
    {
        var re = new PcreRegex(@"a(?C1)(?:(?C2)(*FAIL)|b)(?C3)");

        var startMatchList = new List<bool>();
        var backtrackList = new List<bool>();

        re.Match("abc".AsSpan(), data =>
        {
            startMatchList.Add(data.StartMatch);
            backtrackList.Add(data.Backtrack);
            return PcreCalloutResult.Pass;
        });

        startMatchList.ShouldBe([true, false, false]);
        backtrackList.ShouldBe([false, false, true]);
    }

    [Test]
    public void should_provide_callout_flags_buf()
    {
        var re = new PcreRegex(@"a(?C1)(?:(?C2)(*FAIL)|b)(?C3)");

        var startMatchList = new List<bool>();
        var backtrackList = new List<bool>();

        re.CreateMatchBuffer().Match("abc".AsSpan(), data =>
        {
            startMatchList.Add(data.StartMatch);
            backtrackList.Add(data.Backtrack);
            return PcreCalloutResult.Pass;
        });

        startMatchList.ShouldBe([true, false, false]);
        backtrackList.ShouldBe([false, false, true]);
    }

    [Test]
    public void should_provide_callout_flags_utf8()
    {
        var re = new PcreRegexUtf8(@"a(?C1)(?:(?C2)(*FAIL)|b)(?C3)"u8);

        var startMatchList = new List<bool>();
        var backtrackList = new List<bool>();

        re.Match("abc"u8, data =>
        {
            startMatchList.Add(data.StartMatch);
            backtrackList.Add(data.Backtrack);
            return PcreCalloutResult.Pass;
        });

        startMatchList.ShouldBe([true, false, false]);
        backtrackList.ShouldBe([false, false, true]);
    }

    [Test]
    public void should_provide_callout_flags_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"a(?C1)(?:(?C2)(*FAIL)|b)(?C3)"u8);

        var startMatchList = new List<bool>();
        var backtrackList = new List<bool>();

        re.CreateMatchBuffer().Match("abc"u8, data =>
        {
            startMatchList.Add(data.StartMatch);
            backtrackList.Add(data.Backtrack);
            return PcreCalloutResult.Pass;
        });

        startMatchList.ShouldBe([true, false, false]);
        backtrackList.ShouldBe([false, false, true]);
    }

    [Test]
    public void should_provide_callout_flags_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a(?C1)(?:(?C2)(*FAIL)|b)(?C3)".ToLatin1Bytes());

        var startMatchList = new List<bool>();
        var backtrackList = new List<bool>();

        re.Match("abc".ToLatin1Bytes(), data =>
        {
            startMatchList.Add(data.StartMatch);
            backtrackList.Add(data.Backtrack);
            return PcreCalloutResult.Pass;
        });

        startMatchList.ShouldBe([true, false, false]);
        backtrackList.ShouldBe([false, false, true]);
    }

    [Test]
    public void should_provide_callout_flags_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a(?C1)(?:(?C2)(*FAIL)|b)(?C3)".ToLatin1Bytes());

        var startMatchList = new List<bool>();
        var backtrackList = new List<bool>();

        re.CreateMatchBuffer().Match("abc".ToLatin1Bytes(), data =>
        {
            startMatchList.Add(data.StartMatch);
            backtrackList.Add(data.Backtrack);
            return PcreCalloutResult.Pass;
        });

        startMatchList.ShouldBe([true, false, false]);
        backtrackList.ShouldBe([false, false, true]);
    }

    [Test]
    public void should_execute_string_callout()
    {
        const string pattern = @"(a)(*MARK:foo)(x)?(?C{bar})(bc)";
        var re = new PcreRegex(pattern);

        var calls = 0;

        var match = re.Match("abc", data =>
        {
            data.Number.ShouldBe(0);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(pattern.IndexOf("(?C{bar})", StringComparison.Ordinal) + 9);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(pattern.IndexOf("(?C{bar})", StringComparison.Ordinal) + 4);
            data.String.ShouldBe("bar");

            data.Match.Value.ShouldBe("a");
            data.Match[1].Value.ShouldBe("a");
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.ShouldBeSameAs(string.Empty);
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.ShouldBeSameAs(string.Empty);

            data.Match.Mark.ShouldBe("foo");

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_string_callout_ref()
    {
        const string pattern = @"(a)(*MARK:foo)(x)?(?C{bar})(bc)";
        var re = new PcreRegex(pattern);

        var calls = 0;

        var match = re.Match("abc".AsSpan(), data =>
        {
            data.Number.ShouldBe(0);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(pattern.IndexOf("(?C{bar})", StringComparison.Ordinal) + 9);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(pattern.IndexOf("(?C{bar})", StringComparison.Ordinal) + 4);
            data.String.ShouldBe("bar");

            data.Match.Value.ShouldBe("a");
            data.Match[1].Value.ShouldBe("a");
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.ToString().ShouldBeSameAs(string.Empty);
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.ToString().ShouldBeSameAs(string.Empty);

            data.Match.Mark.ShouldBe("foo");

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_string_callout_buf()
    {
        const string pattern = @"(a)(*MARK:foo)(x)?(?C{bar})(bc)";
        var re = new PcreRegex(pattern);

        var calls = 0;

        var match = re.CreateMatchBuffer().Match("abc".AsSpan(), data =>
        {
            data.Number.ShouldBe(0);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(pattern.IndexOf("(?C{bar})", StringComparison.Ordinal) + 9);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(pattern.IndexOf("(?C{bar})", StringComparison.Ordinal) + 4);
            data.String.ShouldBe("bar");

            data.Match.Value.ShouldBe("a");
            data.Match[1].Value.ShouldBe("a");
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.ToString().ShouldBeSameAs(string.Empty);
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.ToString().ShouldBeSameAs(string.Empty);

            data.Match.Mark.ShouldBe("foo");

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_string_callout_utf8()
    {
        static ReadOnlySpan<byte> GetPattern() => @"(a)(*MARK:foo)(x)?(?C{bar})(bc)"u8;
        var re = new PcreRegexUtf8(GetPattern());

        var calls = 0;

        var match = re.Match("abc"u8, data =>
        {
            data.Number.ShouldBe(0);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(GetPattern().IndexOf("(?C{bar})"u8) + 9);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(GetPattern().IndexOf("(?C{bar})"u8) + 4);
            data.String.ShouldBe("bar");

            data.Match.Value.SequenceEqual("a"u8).ShouldBeTrue();
            data.Match[1].Value.SequenceEqual("a"u8).ShouldBeTrue();
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.IsEmpty.ShouldBeTrue();
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.IsEmpty.ShouldBeTrue();

            data.Match.Mark.SequenceEqual("foo"u8).ShouldBeTrue();

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_string_callout_buf_utf8()
    {
        static ReadOnlySpan<byte> GetPattern() => @"(a)(*MARK:foo)(x)?(?C{bar})(bc)"u8;
        var re = new PcreRegexUtf8(GetPattern());

        var calls = 0;

        var match = re.CreateMatchBuffer().Match("abc"u8, data =>
        {
            data.Number.ShouldBe(0);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(GetPattern().IndexOf("(?C{bar})"u8) + 9);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(GetPattern().IndexOf("(?C{bar})"u8) + 4);
            data.String.ShouldBe("bar");

            data.Match.Value.SequenceEqual("a"u8).ShouldBeTrue();
            data.Match[1].Value.SequenceEqual("a"u8).ShouldBeTrue();
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.IsEmpty.ShouldBeTrue();
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.IsEmpty.ShouldBeTrue();

            data.Match.Mark.SequenceEqual("foo"u8).ShouldBeTrue();

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_string_callout_8bit()
    {
        static ReadOnlySpan<byte> GetPattern() => @"(a)(*MARK:foo)(x)?(?C{bar})(bc)".ToLatin1Bytes();
        var re = TestSupport.CreatePcreRegex8Bit(GetPattern());

        var calls = 0;

        var match = re.Match("abc".ToLatin1Bytes(), data =>
        {
            data.Number.ShouldBe(0);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(GetPattern().IndexOf("(?C{bar})".ToLatin1Bytes()) + 9);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(GetPattern().IndexOf("(?C{bar})".ToLatin1Bytes()) + 4);
            data.String.ShouldBe("bar");

            data.Match.Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
            data.Match[1].Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.IsEmpty.ShouldBeTrue();
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.IsEmpty.ShouldBeTrue();

            data.Match.Mark.SequenceEqual("foo".ToLatin1Bytes()).ShouldBeTrue();

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_execute_string_callout_buf_8bit()
    {
        static ReadOnlySpan<byte> GetPattern() => @"(a)(*MARK:foo)(x)?(?C{bar})(bc)".ToLatin1Bytes();
        var re = TestSupport.CreatePcreRegex8Bit(GetPattern());

        var calls = 0;

        var match = re.CreateMatchBuffer().Match("abc".ToLatin1Bytes(), data =>
        {
            data.Number.ShouldBe(0);
            data.CurrentOffset.ShouldBe(1);
            data.PatternPosition.ShouldBe(GetPattern().IndexOf("(?C{bar})".ToLatin1Bytes()) + 9);
            data.StartOffset.ShouldBe(0);
            data.LastCapture.ShouldBe(1);
            data.MaxCapture.ShouldBe(2);
            data.NextPatternItemLength.ShouldBe(1);
            data.StringOffset.ShouldBe(GetPattern().IndexOf("(?C{bar})".ToLatin1Bytes()) + 4);
            data.String.ShouldBe("bar");

            data.Match.Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
            data.Match[1].Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Value.IsEmpty.ShouldBeTrue();
            data.Match[3].Success.ShouldBeFalse();
            data.Match[3].Value.IsEmpty.ShouldBeTrue();

            data.Match.Mark.SequenceEqual("foo".ToLatin1Bytes()).ShouldBeTrue();

            ++calls;
            return PcreCalloutResult.Pass;
        });

        match.Success.ShouldBeTrue();
        calls.ShouldBe(1);
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_in_callout()
    {
        var re = new PcreRegex(@"(a|(z))(bc)(?C42)");
        var calls = 0;

        re.Match("abc", data =>
        {
            data.Match[1].Success.ShouldBeTrue();
            data.Match[1].Index.ShouldBe(0);
            data.Match[1].Length.ShouldBe(1);
            data.Match[1].Value.ShouldBe("a");

            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Index.ShouldBe(-1);
            data.Match[2].EndIndex.ShouldBe(-1);
            data.Match[2].Value.ShouldBeSameAs(string.Empty);

            data.Match[3].Success.ShouldBeTrue();
            data.Match[3].Index.ShouldBe(1);
            data.Match[3].Length.ShouldBe(2);
            data.Match[3].Value.ShouldBe("bc");

            ++calls;
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_in_callout_ref()
    {
        var re = new PcreRegex(@"(a|(z))(bc)(?C42)");
        var calls = 0;

        re.Match("abc".AsSpan(), data =>
        {
            data.Match[1].Success.ShouldBeTrue();
            data.Match[1].Index.ShouldBe(0);
            data.Match[1].Length.ShouldBe(1);
            data.Match[1].Value.ShouldBe("a");

            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Index.ShouldBe(-1);
            data.Match[2].EndIndex.ShouldBe(-1);
            data.Match[2].Value.ToString().ShouldBeSameAs(string.Empty);

            data.Match[3].Success.ShouldBeTrue();
            data.Match[3].Index.ShouldBe(1);
            data.Match[3].Length.ShouldBe(2);
            data.Match[3].Value.ShouldBe("bc");

            ++calls;
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_in_callout_buf()
    {
        var re = new PcreRegex(@"(a|(z))(bc)(?C42)");
        var calls = 0;

        re.CreateMatchBuffer().Match("abc".AsSpan(), data =>
        {
            data.Match[1].Success.ShouldBeTrue();
            data.Match[1].Index.ShouldBe(0);
            data.Match[1].Length.ShouldBe(1);
            data.Match[1].Value.ShouldBe("a");

            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Index.ShouldBe(-1);
            data.Match[2].EndIndex.ShouldBe(-1);
            data.Match[2].Value.ToString().ShouldBeSameAs(string.Empty);

            data.Match[3].Success.ShouldBeTrue();
            data.Match[3].Index.ShouldBe(1);
            data.Match[3].Length.ShouldBe(2);
            data.Match[3].Value.ShouldBe("bc");

            ++calls;
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_in_callout_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(a|(z))(bc)(?C42)".ToLatin1Bytes());
        var calls = 0;

        re.Match("abc".ToLatin1Bytes(), data =>
        {
            data.Match[1].Success.ShouldBeTrue();
            data.Match[1].Index.ShouldBe(0);
            data.Match[1].Length.ShouldBe(1);
            data.Match[1].Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();

            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Index.ShouldBe(-1);
            data.Match[2].EndIndex.ShouldBe(-1);
            data.Match[2].Value.IsEmpty.ShouldBeTrue();

            data.Match[3].Success.ShouldBeTrue();
            data.Match[3].Index.ShouldBe(1);
            data.Match[3].Length.ShouldBe(2);
            data.Match[3].Value.SequenceEqual("bc".ToLatin1Bytes()).ShouldBeTrue();

            ++calls;
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_in_callout_utf8()
    {
        var re = new PcreRegexUtf8(@"(a|(z))(bc)(?C42)"u8);
        var calls = 0;

        re.Match("abc"u8, data =>
        {
            data.Match[1].Success.ShouldBeTrue();
            data.Match[1].Index.ShouldBe(0);
            data.Match[1].Length.ShouldBe(1);
            data.Match[1].Value.SequenceEqual("a"u8).ShouldBeTrue();

            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Index.ShouldBe(-1);
            data.Match[2].EndIndex.ShouldBe(-1);
            data.Match[2].Value.IsEmpty.ShouldBeTrue();

            data.Match[3].Success.ShouldBeTrue();
            data.Match[3].Index.ShouldBe(1);
            data.Match[3].Length.ShouldBe(2);
            data.Match[3].Value.SequenceEqual("bc"u8).ShouldBeTrue();

            ++calls;
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_in_callout_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"(a|(z))(bc)(?C42)"u8);
        var calls = 0;

        re.CreateMatchBuffer().Match("abc"u8, data =>
        {
            data.Match[1].Success.ShouldBeTrue();
            data.Match[1].Index.ShouldBe(0);
            data.Match[1].Length.ShouldBe(1);
            data.Match[1].Value.SequenceEqual("a"u8).ShouldBeTrue();

            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Index.ShouldBe(-1);
            data.Match[2].EndIndex.ShouldBe(-1);
            data.Match[2].Value.IsEmpty.ShouldBeTrue();

            data.Match[3].Success.ShouldBeTrue();
            data.Match[3].Index.ShouldBe(1);
            data.Match[3].Length.ShouldBe(2);
            data.Match[3].Value.SequenceEqual("bc"u8).ShouldBeTrue();

            ++calls;
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_support_unmatched_groups_before_matched_groups_in_callout_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(a|(z))(bc)(?C42)".ToLatin1Bytes());
        var calls = 0;

        re.CreateMatchBuffer().Match("abc".ToLatin1Bytes(), data =>
        {
            data.Match[1].Success.ShouldBeTrue();
            data.Match[1].Index.ShouldBe(0);
            data.Match[1].Length.ShouldBe(1);
            data.Match[1].Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();

            data.Match[2].Success.ShouldBeFalse();
            data.Match[2].Index.ShouldBe(-1);
            data.Match[2].EndIndex.ShouldBe(-1);
            data.Match[2].Value.IsEmpty.ShouldBeTrue();

            data.Match[3].Success.ShouldBeTrue();
            data.Match[3].Index.ShouldBe(1);
            data.Match[3].Length.ShouldBe(2);
            data.Match[3].Value.SequenceEqual("bc".ToLatin1Bytes()).ShouldBeTrue();

            ++calls;
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_handle_callouts_with_many_captures_ref()
    {
        var sb = new StringBuilder();
        const int length = InternalRegex.MaxStackAllocCaptureCount * 2;

        for (var i = 0; i < length; ++i)
            sb.Append("(a)");

        sb.Append("(?C)");

        var re = new PcreRegex(sb.ToString());
        var calls = 0;
        var subject = new string('a', length);

        re.Match(subject.AsSpan(), data =>
        {
            ++calls;
            data.Match.Length.ShouldBe(length);
            data.Match.Groups[1].Value.ShouldBe("a");
            data.Match.Groups[length].Value.ShouldBe("a");
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_handle_callouts_with_many_captures_buf()
    {
        var sb = new StringBuilder();
        const int length = InternalRegex.MaxStackAllocCaptureCount * 2;

        for (var i = 0; i < length; ++i)
            sb.Append("(a)");

        sb.Append("(?C)");

        var re = new PcreRegex(sb.ToString());
        var calls = 0;
        var subject = new string('a', length);

        re.CreateMatchBuffer().Match(subject.AsSpan(), data =>
        {
            ++calls;
            data.Match.Length.ShouldBe(length);
            data.Match.Groups[1].Value.ShouldBe("a");
            data.Match.Groups[length].Value.ShouldBe("a");
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_handle_callouts_with_many_captures_utf8()
    {
        var sb = new StringBuilder();
        const int length = InternalRegex.MaxStackAllocCaptureCount * 2;

        for (var i = 0; i < length; ++i)
            sb.Append("(a)");

        sb.Append("(?C)");

        var re = new PcreRegexUtf8(sb.ToString());
        var calls = 0;
        var subject = new byte[length];
        subject.AsSpan().Fill((byte)'a');

        re.Match(subject, data =>
        {
            ++calls;
            data.Match.Length.ShouldBe(length);
            data.Match.Groups[1].Value.SequenceEqual("a"u8).ShouldBeTrue();
            data.Match.Groups[length].Value.SequenceEqual("a"u8).ShouldBeTrue();
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_handle_callouts_with_many_captures_buf_utf8()
    {
        var sb = new StringBuilder();
        const int length = InternalRegex.MaxStackAllocCaptureCount * 2;

        for (var i = 0; i < length; ++i)
            sb.Append("(a)");

        sb.Append("(?C)");

        var re = new PcreRegexUtf8(sb.ToString());
        var calls = 0;
        var subject = new byte[length];
        subject.AsSpan().Fill((byte)'a');

        re.CreateMatchBuffer().Match(subject, data =>
        {
            ++calls;
            data.Match.Length.ShouldBe(length);
            data.Match.Groups[1].Value.SequenceEqual("a"u8).ShouldBeTrue();
            data.Match.Groups[length].Value.SequenceEqual("a"u8).ShouldBeTrue();
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_handle_callouts_with_many_captures_8bit()
    {
        var sb = new StringBuilder();
        const int length = InternalRegex.MaxStackAllocCaptureCount * 2;

        for (var i = 0; i < length; ++i)
            sb.Append("(a)");

        sb.Append("(?C)");

        var re = TestSupport.CreatePcreRegex8Bit(sb.ToString());
        var calls = 0;
        var subject = new byte[length];
        subject.AsSpan().Fill((byte)'a');

        re.Match(subject, data =>
        {
            ++calls;
            data.Match.Length.ShouldBe(length);
            data.Match.Groups[1].Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
            data.Match.Groups[length].Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_handle_callouts_with_many_captures_buf_8bit()
    {
        var sb = new StringBuilder();
        const int length = InternalRegex.MaxStackAllocCaptureCount * 2;

        for (var i = 0; i < length; ++i)
            sb.Append("(a)");

        sb.Append("(?C)");

        var re = TestSupport.CreatePcreRegex8Bit(sb.ToString());
        var calls = 0;
        var subject = new byte[length];
        subject.AsSpan().Fill((byte)'a');

        re.CreateMatchBuffer().Match(subject, data =>
        {
            ++calls;
            data.Match.Length.ShouldBe(length);
            data.Match.Groups[1].Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
            data.Match.Groups[length].Value.SequenceEqual("a".ToLatin1Bytes()).ShouldBeTrue();
            return PcreCalloutResult.Pass;
        });

        calls.ShouldBe(1);
    }

    [Test]
    public void should_handle_end_before_start()
    {
        var re = new PcreRegex(@"(?=a+\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });

        var match = re.Match("aaa");

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);
        match.Value.ShouldBe(string.Empty);
    }

    [Test]
    public void should_handle_end_before_start_ref()
    {
        var re = new PcreRegex(@"(?=a+\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });

        var match = re.Match("aaa".AsSpan());

        match.Success.ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);
        match.Value.ShouldBe(string.Empty);
    }

    [Test]
    public void should_handle_end_before_start_buf()
    {
        var re = new PcreRegex(@"(?=a+\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });

        var match = re.CreateMatchBuffer().Match("aaa".AsSpan());

        match.Success.ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);
        match.Value.ShouldBe(string.Empty);
    }

    [Test]
    public void should_handle_end_before_start_utf8()
    {
        var re = new PcreRegexUtf8(@"(?=a+\K)"u8, new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });

        var match = re.Match("aaa"u8);

        match.Success.ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);
        match.Value.IsEmpty.ShouldBeTrue();
    }

    [Test]
    public void should_handle_end_before_start_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"(?=a+\K)"u8, new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });

        var match = re.CreateMatchBuffer().Match("aaa"u8);

        match.Success.ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);
        match.Value.IsEmpty.ShouldBeTrue();
    }

    [Test]
    public void should_handle_end_before_start_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?=a+\K)".ToLatin1Bytes(), new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });

        var match = re.Match("aaa".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);
        match.Value.IsEmpty.ShouldBeTrue();
    }

    [Test]
    public void should_handle_end_before_start_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?=a+\K)".ToLatin1Bytes(), new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });

        var match = re.CreateMatchBuffer().Match("aaa".ToLatin1Bytes());

        match.Success.ShouldBeTrue();
        match.Index.ShouldBe(3);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);
        match.Value.IsEmpty.ShouldBeTrue();
    }

    [Test]
    public void should_handle_additional_options()
    {
        var re = new PcreRegex(@"bar");

        var match = re.Match("foobar", PcreMatchOptions.None);

        match.Success.ShouldBeTrue();

        match = re.Match("foobar", PcreMatchOptions.Anchored);

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_additional_options_ref()
    {
        var re = new PcreRegex(@"bar");

        var match = re.Match("foobar".AsSpan(), PcreMatchOptions.None);

        match.Success.ShouldBeTrue();

        match = re.Match("foobar".AsSpan(), PcreMatchOptions.Anchored);

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_additional_options_buf()
    {
        var re = new PcreRegex(@"bar");
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("foobar".AsSpan(), PcreMatchOptions.None);

        match.Success.ShouldBeTrue();

        match = buffer.Match("foobar".AsSpan(), PcreMatchOptions.Anchored);

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_additional_options_utf8()
    {
        var re = new PcreRegexUtf8(@"bar"u8);

        var match = re.Match("foobar"u8, PcreMatchOptions.None);

        match.Success.ShouldBeTrue();

        match = re.Match("foobar"u8, PcreMatchOptions.Anchored);

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_additional_options_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"bar"u8);
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("foobar"u8, PcreMatchOptions.None);

        match.Success.ShouldBeTrue();

        match = buffer.Match("foobar"u8, PcreMatchOptions.Anchored);

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_additional_options_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"bar".ToLatin1Bytes());

        var match = re.Match("foobar".ToLatin1Bytes(), PcreMatchOptions.None);

        match.Success.ShouldBeTrue();

        match = re.Match("foobar".ToLatin1Bytes(), PcreMatchOptions.Anchored);

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_additional_options_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"bar".ToLatin1Bytes());
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("foobar".ToLatin1Bytes(), PcreMatchOptions.None);

        match.Success.ShouldBeTrue();

        match = buffer.Match("foobar".ToLatin1Bytes(), PcreMatchOptions.Anchored);

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_extra_options()
    {
        var re = new PcreRegex(@"bar", new PcreRegexSettings(PcreOptions.Literal)
        {
            ExtraCompileOptions = PcreExtraCompileOptions.MatchWord
        });

        re.IsMatch("foo bar baz").ShouldBeTrue();
        re.IsMatch("foobar baz").ShouldBeFalse();
    }

    [Test]
    public void should_handle_extra_options_ref()
    {
        var re = new PcreRegex(@"bar", new PcreRegexSettings(PcreOptions.Literal)
        {
            ExtraCompileOptions = PcreExtraCompileOptions.MatchWord
        });

        re.IsMatch("foo bar baz".AsSpan()).ShouldBeTrue();
        re.IsMatch("foobar baz".AsSpan()).ShouldBeFalse();
    }

    [Test]
    public void should_handle_extra_options_buf()
    {
        var re = new PcreRegex(@"bar", new PcreRegexSettings(PcreOptions.Literal)
        {
            ExtraCompileOptions = PcreExtraCompileOptions.MatchWord
        });

        var buffer = re.CreateMatchBuffer();

        buffer.IsMatch("foo bar baz".AsSpan()).ShouldBeTrue();
        buffer.IsMatch("foobar baz".AsSpan()).ShouldBeFalse();
    }

    [Test]
    public void should_handle_extra_options_utf8()
    {
        var re = new PcreRegexUtf8(@"bar"u8, new PcreRegexSettings(PcreOptions.Literal)
        {
            ExtraCompileOptions = PcreExtraCompileOptions.MatchWord
        });

        re.IsMatch("foo bar baz"u8).ShouldBeTrue();
        re.IsMatch("foobar baz"u8).ShouldBeFalse();
    }

    [Test]
    public void should_handle_extra_options_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"bar"u8, new PcreRegexSettings(PcreOptions.Literal)
        {
            ExtraCompileOptions = PcreExtraCompileOptions.MatchWord
        });

        var buffer = re.CreateMatchBuffer();

        buffer.IsMatch("foo bar baz"u8).ShouldBeTrue();
        buffer.IsMatch("foobar baz"u8).ShouldBeFalse();
    }

    [Test]
    public void should_handle_extra_options_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"bar".ToLatin1Bytes(), new PcreRegexSettings(PcreOptions.Literal)
        {
            ExtraCompileOptions = PcreExtraCompileOptions.MatchWord
        });

        re.IsMatch("foo bar baz".ToLatin1Bytes()).ShouldBeTrue();
        re.IsMatch("foobar baz".ToLatin1Bytes()).ShouldBeFalse();
    }

    [Test]
    public void should_handle_extra_options_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"bar".ToLatin1Bytes(), new PcreRegexSettings(PcreOptions.Literal)
        {
            ExtraCompileOptions = PcreExtraCompileOptions.MatchWord
        });

        var buffer = re.CreateMatchBuffer();

        buffer.IsMatch("foo bar baz".ToLatin1Bytes()).ShouldBeTrue();
        buffer.IsMatch("foobar baz".ToLatin1Bytes()).ShouldBeFalse();
    }

    [Test]
    [TestCase(PcreMatchOptions.PartialSoft)]
    [TestCase(PcreMatchOptions.PartialHard)]
    public void should_match_partially(PcreMatchOptions options)
    {
        var re = new PcreRegex(@"(?<=abc)123");

        var match = re.Match("xyzabc12", options);

        match.Success.ShouldBeFalse();
        match.IsPartialMatch.ShouldBeTrue();
        match.Index.ShouldBe(6);
        match.EndIndex.ShouldBe(8);
        match.Length.ShouldBe(2);
        match.Value.ShouldBe("12");
    }

    [Test]
    [TestCase(PcreMatchOptions.PartialSoft)]
    [TestCase(PcreMatchOptions.PartialHard)]
    public void should_match_partially_ref(PcreMatchOptions options)
    {
        var re = new PcreRegex(@"(?<=abc)123");

        var match = re.Match("xyzabc12".AsSpan(), options);

        match.Success.ShouldBeFalse();
        match.IsPartialMatch.ShouldBeTrue();
        match.Index.ShouldBe(6);
        match.EndIndex.ShouldBe(8);
        match.Length.ShouldBe(2);
        match.Value.ShouldBe("12");
    }

    [Test]
    [TestCase(PcreMatchOptions.PartialSoft)]
    [TestCase(PcreMatchOptions.PartialHard)]
    public void should_match_partially_buf(PcreMatchOptions options)
    {
        var re = new PcreRegex(@"(?<=abc)123");

        var match = re.CreateMatchBuffer().Match("xyzabc12".AsSpan(), options);

        match.Success.ShouldBeFalse();
        match.IsPartialMatch.ShouldBeTrue();
        match.Index.ShouldBe(6);
        match.EndIndex.ShouldBe(8);
        match.Length.ShouldBe(2);
        match.Value.ShouldBe("12");
    }

    [Test]
    [TestCase(PcreMatchOptions.PartialSoft)]
    [TestCase(PcreMatchOptions.PartialHard)]
    public void should_match_partially_utf8(PcreMatchOptions options)
    {
        var re = new PcreRegexUtf8(@"(?<=abc)123"u8);

        var match = re.Match("xyzabc12"u8, options);

        match.Success.ShouldBeFalse();
        match.IsPartialMatch.ShouldBeTrue();
        match.Index.ShouldBe(6);
        match.EndIndex.ShouldBe(8);
        match.Length.ShouldBe(2);
        match.Value.SequenceEqual("12"u8).ShouldBeTrue();
    }

    [Test]
    [TestCase(PcreMatchOptions.PartialSoft)]
    [TestCase(PcreMatchOptions.PartialHard)]
    public void should_match_partially_buf_utf8(PcreMatchOptions options)
    {
        var re = new PcreRegexUtf8(@"(?<=abc)123"u8);

        var match = re.CreateMatchBuffer().Match("xyzabc12"u8, options);

        match.Success.ShouldBeFalse();
        match.IsPartialMatch.ShouldBeTrue();
        match.Index.ShouldBe(6);
        match.EndIndex.ShouldBe(8);
        match.Length.ShouldBe(2);
        match.Value.SequenceEqual("12"u8).ShouldBeTrue();
    }

    [Test]
    [TestCase(PcreMatchOptions.PartialSoft)]
    [TestCase(PcreMatchOptions.PartialHard)]
    public void should_match_partially_8bit(PcreMatchOptions options)
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?<=abc)123".ToLatin1Bytes());

        var match = re.Match("xyzabc12".ToLatin1Bytes(), options);

        match.Success.ShouldBeFalse();
        match.IsPartialMatch.ShouldBeTrue();
        match.Index.ShouldBe(6);
        match.EndIndex.ShouldBe(8);
        match.Length.ShouldBe(2);
        match.Value.SequenceEqual("12".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    [TestCase(PcreMatchOptions.PartialSoft)]
    [TestCase(PcreMatchOptions.PartialHard)]
    public void should_match_partially_buf_8bit(PcreMatchOptions options)
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?<=abc)123".ToLatin1Bytes());

        var match = re.CreateMatchBuffer().Match("xyzabc12".ToLatin1Bytes(), options);

        match.Success.ShouldBeFalse();
        match.IsPartialMatch.ShouldBeTrue();
        match.Index.ShouldBe(6);
        match.EndIndex.ShouldBe(8);
        match.Length.ShouldBe(2);
        match.Value.SequenceEqual("12".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    public void should_differentiate_soft_and_hard_partial_matching()
    {
        var re = new PcreRegex(@"dog(sbody)?");

        var softMatch = re.Match("dog", PcreMatchOptions.PartialSoft);
        var hardMatch = re.Match("dog", PcreMatchOptions.PartialHard);

        softMatch.Success.ShouldBeTrue();
        softMatch.IsPartialMatch.ShouldBeFalse();

        hardMatch.Success.ShouldBeFalse();
        hardMatch.IsPartialMatch.ShouldBeTrue();
    }

    [Test]
    public void should_differentiate_soft_and_hard_partial_matching_ref()
    {
        var re = new PcreRegex(@"dog(sbody)?");

        var softMatch = re.Match("dog".AsSpan(), PcreMatchOptions.PartialSoft);
        var hardMatch = re.Match("dog".AsSpan(), PcreMatchOptions.PartialHard);

        softMatch.Success.ShouldBeTrue();
        softMatch.IsPartialMatch.ShouldBeFalse();

        hardMatch.Success.ShouldBeFalse();
        hardMatch.IsPartialMatch.ShouldBeTrue();
    }

    [Test]
    public void should_differentiate_soft_and_hard_partial_matching_buf()
    {
        var re = new PcreRegex(@"dog(sbody)?");
        var buffer = re.CreateMatchBuffer();

        var softMatch = buffer.Match("dog".AsSpan(), PcreMatchOptions.PartialSoft);

        softMatch.Success.ShouldBeTrue();
        softMatch.IsPartialMatch.ShouldBeFalse();

        var hardMatch = buffer.Match("dog".AsSpan(), PcreMatchOptions.PartialHard);

        hardMatch.Success.ShouldBeFalse();
        hardMatch.IsPartialMatch.ShouldBeTrue();
    }

    [Test]
    public void should_differentiate_soft_and_hard_partial_matching_utf8()
    {
        var re = new PcreRegexUtf8(@"dog(sbody)?"u8);

        var softMatch = re.Match("dog"u8, PcreMatchOptions.PartialSoft);
        var hardMatch = re.Match("dog"u8, PcreMatchOptions.PartialHard);

        softMatch.Success.ShouldBeTrue();
        softMatch.IsPartialMatch.ShouldBeFalse();

        hardMatch.Success.ShouldBeFalse();
        hardMatch.IsPartialMatch.ShouldBeTrue();
    }

    [Test]
    public void should_differentiate_soft_and_hard_partial_matching_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"dog(sbody)?"u8);
        var buffer = re.CreateMatchBuffer();

        var softMatch = buffer.Match("dog"u8, PcreMatchOptions.PartialSoft);

        softMatch.Success.ShouldBeTrue();
        softMatch.IsPartialMatch.ShouldBeFalse();

        var hardMatch = buffer.Match("dog"u8, PcreMatchOptions.PartialHard);

        hardMatch.Success.ShouldBeFalse();
        hardMatch.IsPartialMatch.ShouldBeTrue();
    }

    [Test]
    public void should_differentiate_soft_and_hard_partial_matching_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"dog(sbody)?".ToLatin1Bytes());

        var softMatch = re.Match("dog".ToLatin1Bytes(), PcreMatchOptions.PartialSoft);
        var hardMatch = re.Match("dog".ToLatin1Bytes(), PcreMatchOptions.PartialHard);

        softMatch.Success.ShouldBeTrue();
        softMatch.IsPartialMatch.ShouldBeFalse();

        hardMatch.Success.ShouldBeFalse();
        hardMatch.IsPartialMatch.ShouldBeTrue();
    }

    [Test]
    public void should_differentiate_soft_and_hard_partial_matching_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"dog(sbody)?".ToLatin1Bytes());
        var buffer = re.CreateMatchBuffer();

        var softMatch = buffer.Match("dog".ToLatin1Bytes(), PcreMatchOptions.PartialSoft);

        softMatch.Success.ShouldBeTrue();
        softMatch.IsPartialMatch.ShouldBeFalse();

        var hardMatch = buffer.Match("dog".ToLatin1Bytes(), PcreMatchOptions.PartialHard);

        hardMatch.Success.ShouldBeFalse();
        hardMatch.IsPartialMatch.ShouldBeTrue();
    }

    [Test]
    public void should_check_pattern_utf_validity()
    {
        var ex = Should.Throw<PcrePatternException>(() => _ = new PcreRegex("A\uD800B"));
        ex.ErrorCode.ShouldBe(PcreErrorCode.Utf16Err2);
        ex.Message.ShouldContain("invalid low surrogate");
    }

    [Test]
    public void should_check_pattern_utf_validity_utf8()
    {
        var ex = Should.Throw<PcrePatternException>(() => _ = new PcreRegexUtf8([(byte)'A', (byte)'é', (byte)'B']));
        ex.ErrorCode.ShouldBe(PcreErrorCode.Utf8Err1);
        ex.Message.ShouldContain("1 byte missing at end at offset 1");
    }

    [Test]
    public void should_not_check_pattern_utf_validity_8bit()
    {
        _ = TestSupport.CreatePcreRegex8Bit([(byte)'A', (byte)'é', (byte)'B']);
    }

    [Test]
    public void should_check_subject_utf_validity()
    {
        var re = new PcreRegex(@"A");
        var ex = Should.Throw<PcreMatchException>(() => _ = re.Match("A\uD800B"));
        ex.ErrorCode.ShouldBe(PcreErrorCode.Utf16Err2);
        ex.Message.ShouldContain("invalid low surrogate");
    }

    [Test]
    public void should_check_subject_utf_validity_ref()
    {
        var re = new PcreRegex(@"A");
        var ex = Should.Throw<PcreMatchException>(() => _ = re.Match("A\uD800B".AsSpan()));
        ex.ErrorCode.ShouldBe(PcreErrorCode.Utf16Err2);
        ex.Message.ShouldContain("invalid low surrogate");
    }

    [Test]
    public void should_check_subject_utf_validity_buf()
    {
        var re = new PcreRegex(@"A");
        var buffer = re.CreateMatchBuffer();

        var ex = Should.Throw<PcreMatchException>(() => _ = buffer.Match("A\uD800B".AsSpan()));
        ex.ErrorCode.ShouldBe(PcreErrorCode.Utf16Err2);
        ex.Message.ShouldContain("invalid low surrogate");
    }

    [Test]
    public void should_check_subject_utf_validity_utf8()
    {
        var re = new PcreRegexUtf8(@"A"u8);
        var ex = Should.Throw<PcreMatchException>(() => _ = re.Match([(byte)'A', (byte)'é', (byte)'B']));
        ex.ErrorCode.ShouldBe(PcreErrorCode.Utf8Err1);
        ex.Message.ShouldContain("1 byte missing at end");
    }

    [Test]
    public void should_check_subject_utf_validity_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"A"u8);
        var buffer = re.CreateMatchBuffer();

        var ex = Should.Throw<PcreMatchException>(() => _ = buffer.Match([(byte)'A', (byte)'é', (byte)'B']));
        ex.ErrorCode.ShouldBe(PcreErrorCode.Utf8Err1);
        ex.Message.ShouldContain("1 byte missing at end");
    }

    [Test]
    public void should_not_check_subject_utf_validity_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"A".ToLatin1Bytes());
        _ = re.Match([(byte)'A', (byte)'é', (byte)'B']);
    }

    [Test]
    public void should_not_check_subject_utf_validity_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"A".ToLatin1Bytes());
        var buffer = re.CreateMatchBuffer();
        _ = buffer.Match([(byte)'A', (byte)'é', (byte)'B']);
    }

    [Test]
    public void should_handle_offset_limit()
    {
        var re = new PcreRegex(@"bar", PcreOptions.UseOffsetLimit);

        var match = re.Match("foobar");
        match.Success.ShouldBeTrue();

        match = re.Match("foobar", 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        });
        match.Success.ShouldBeTrue();

        match = re.Match("foobar", 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 2
        });
        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_offset_limit_ref()
    {
        var re = new PcreRegex(@"bar", PcreOptions.UseOffsetLimit);

        var match = re.Match("foobar".AsSpan());
        match.Success.ShouldBeTrue();

        match = re.Match("foobar".AsSpan(), 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        });
        match.Success.ShouldBeTrue();

        match = re.Match("foobar".AsSpan(), 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 2
        });
        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_offset_limit_buf()
    {
        var re = new PcreRegex(@"bar", PcreOptions.UseOffsetLimit);

        var buffer = re.CreateMatchBuffer();
        var match = buffer.Match("foobar".AsSpan());
        match.Success.ShouldBeTrue();

        buffer = re.CreateMatchBuffer(new PcreMatchSettings { OffsetLimit = 3 });
        match = buffer.Match("foobar".AsSpan());
        match.Success.ShouldBeTrue();

        buffer = re.CreateMatchBuffer(new PcreMatchSettings { OffsetLimit = 2 });
        match = buffer.Match("foobar".AsSpan());
        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_offset_limit_utf8()
    {
        var re = new PcreRegexUtf8(@"bar"u8, PcreOptions.UseOffsetLimit);

        var match = re.Match("foobar"u8);
        match.Success.ShouldBeTrue();

        match = re.Match("foobar"u8, 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        });
        match.Success.ShouldBeTrue();

        match = re.Match("foobar"u8, 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 2
        });
        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_offset_limit_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"bar"u8, PcreOptions.UseOffsetLimit);

        var buffer = re.CreateMatchBuffer();
        var match = buffer.Match("foobar"u8);
        match.Success.ShouldBeTrue();

        buffer = re.CreateMatchBuffer(new PcreMatchSettings { OffsetLimit = 3 });
        match = buffer.Match("foobar"u8);
        match.Success.ShouldBeTrue();

        buffer = re.CreateMatchBuffer(new PcreMatchSettings { OffsetLimit = 2 });
        match = buffer.Match("foobar"u8);
        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_offset_limit_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"bar".ToLatin1Bytes(), PcreOptions.UseOffsetLimit);

        var match = re.Match("foobar".ToLatin1Bytes());
        match.Success.ShouldBeTrue();

        match = re.Match("foobar".ToLatin1Bytes(), 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        });
        match.Success.ShouldBeTrue();

        match = re.Match("foobar".ToLatin1Bytes(), 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 2
        });
        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_handle_offset_limit_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"bar".ToLatin1Bytes(), PcreOptions.UseOffsetLimit);

        var buffer = re.CreateMatchBuffer();
        var match = buffer.Match("foobar".ToLatin1Bytes());
        match.Success.ShouldBeTrue();

        buffer = re.CreateMatchBuffer(new PcreMatchSettings { OffsetLimit = 3 });
        match = buffer.Match("foobar".ToLatin1Bytes());
        match.Success.ShouldBeTrue();

        buffer = re.CreateMatchBuffer(new PcreMatchSettings { OffsetLimit = 2 });
        match = buffer.Match("foobar".ToLatin1Bytes());
        match.Success.ShouldBeFalse();
    }

    [Test]
    public void should_detect_invalid_offset_limit_usage()
    {
        var re = new PcreRegex(@"bar");

        var ex = Should.Throw<PcreMatchException>(() => re.Match("foobar", 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        }));

        ex.ErrorCode.ShouldBe(PcreErrorCode.BadOffsetLimit);
    }

    [Test]
    public void should_detect_invalid_offset_limit_usage_ref()
    {
        var re = new PcreRegex(@"bar");

        var ex = Should.Throw<PcreMatchException>(() => re.Match("foobar".AsSpan(), 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        }));

        ex.ErrorCode.ShouldBe(PcreErrorCode.BadOffsetLimit);
    }

    [Test]
    public void should_detect_invalid_offset_limit_usage_buf()
    {
        var re = new PcreRegex(@"bar");
        var buffer = re.CreateMatchBuffer(new PcreMatchSettings
        {
            OffsetLimit = 3
        });

        var ex = Should.Throw<PcreMatchException>(() => buffer.Match("foobar".AsSpan()));
        ex.ErrorCode.ShouldBe(PcreErrorCode.BadOffsetLimit);
    }

    [Test]
    public void should_detect_invalid_offset_limit_usage_utf8()
    {
        var re = new PcreRegexUtf8(@"bar"u8);

        var ex = Should.Throw<PcreMatchException>(() => re.Match("foobar"u8, 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        }));

        ex.ErrorCode.ShouldBe(PcreErrorCode.BadOffsetLimit);
    }

    [Test]
    public void should_detect_invalid_offset_limit_usage_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"bar"u8);
        var buffer = re.CreateMatchBuffer(new PcreMatchSettings
        {
            OffsetLimit = 3
        });

        var ex = Should.Throw<PcreMatchException>(() => buffer.Match("foobar"u8));
        ex.ErrorCode.ShouldBe(PcreErrorCode.BadOffsetLimit);
    }

    [Test]
    public void should_detect_invalid_offset_limit_usage_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"bar".ToLatin1Bytes());

        var ex = Should.Throw<PcreMatchException>(() => re.Match("foobar".ToLatin1Bytes(), 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        }));

        ex.ErrorCode.ShouldBe(PcreErrorCode.BadOffsetLimit);
    }

    [Test]
    public void should_detect_invalid_offset_limit_usage_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"bar".ToLatin1Bytes());
        var buffer = re.CreateMatchBuffer(new PcreMatchSettings
        {
            OffsetLimit = 3
        });

        var ex = Should.Throw<PcreMatchException>(() => buffer.Match("foobar".ToLatin1Bytes()));
        ex.ErrorCode.ShouldBe(PcreErrorCode.BadOffsetLimit);
    }

    [Test]
    public void should_match_script_run()
    {
        const string subject = "123\U0001D7CF\U0001D7D0\U0001D7D1";

        var normal = PcreRegex.Match(subject, @"\d+", PcreOptions.Unicode);
        normal.Success.ShouldBeTrue();
        normal.Index.ShouldBe(0);
        normal.Length.ShouldBe(subject.Length);

        var scriptRun = PcreRegex.Match(subject, @"(*script_run:\d+)", PcreOptions.Unicode);
        scriptRun.Success.ShouldBeTrue();
        scriptRun.Index.ShouldBe(0);
        scriptRun.Length.ShouldBe(3);
    }

    [Test]
    public void should_match_script_run_ref()
    {
        const string subject = "123\U0001D7CF\U0001D7D0\U0001D7D1";

        var normal = new PcreRegex(@"\d+", PcreOptions.Unicode).Match(subject.AsSpan());
        normal.Success.ShouldBeTrue();
        normal.Index.ShouldBe(0);
        normal.Length.ShouldBe(subject.Length);

        var scriptRun = new PcreRegex(@"(*script_run:\d+)", PcreOptions.Unicode).Match(subject.AsSpan());
        scriptRun.Success.ShouldBeTrue();
        scriptRun.Index.ShouldBe(0);
        scriptRun.Length.ShouldBe(3);
    }

    [Test]
    public void should_match_script_run_buf()
    {
        const string subject = "123\U0001D7CF\U0001D7D0\U0001D7D1";

        var normal = new PcreRegex(@"\d+", PcreOptions.Unicode).CreateMatchBuffer().Match(subject.AsSpan());
        normal.Success.ShouldBeTrue();
        normal.Index.ShouldBe(0);
        normal.Length.ShouldBe(subject.Length);

        var scriptRun = new PcreRegex(@"(*script_run:\d+)", PcreOptions.Unicode).CreateMatchBuffer().Match(subject.AsSpan());
        scriptRun.Success.ShouldBeTrue();
        scriptRun.Index.ShouldBe(0);
        scriptRun.Length.ShouldBe(3);
    }

    [Test]
    public void should_match_script_run_utf8()
    {
        var subject = "123\U0001D7CF\U0001D7D0\U0001D7D1"u8;

        var normal = new PcreRegexUtf8(@"\d+"u8, PcreOptions.Unicode).Match(subject);
        normal.Success.ShouldBeTrue();
        normal.Index.ShouldBe(0);
        normal.Length.ShouldBe(subject.Length);

        var scriptRun = new PcreRegexUtf8(@"(*script_run:\d+)"u8, PcreOptions.Unicode).Match(subject);
        scriptRun.Success.ShouldBeTrue();
        scriptRun.Index.ShouldBe(0);
        scriptRun.Length.ShouldBe(3);
    }

    [Test]
    public void should_match_script_run_buf_utf8()
    {
        var subject = "123\U0001D7CF\U0001D7D0\U0001D7D1"u8;

        var normal = new PcreRegexUtf8(@"\d+"u8, PcreOptions.Unicode).CreateMatchBuffer().Match(subject);
        normal.Success.ShouldBeTrue();
        normal.Index.ShouldBe(0);
        normal.Length.ShouldBe(subject.Length);

        var scriptRun = new PcreRegexUtf8(@"(*script_run:\d+)"u8, PcreOptions.Unicode).CreateMatchBuffer().Match(subject);
        scriptRun.Success.ShouldBeTrue();
        scriptRun.Index.ShouldBe(0);
        scriptRun.Length.ShouldBe(3);
    }

    [Test]
    public void should_match_empty_string()
    {
        var re = new PcreRegex(string.Empty);
        var match = re.Match(string.Empty);

        match.ShouldNotBeNull();
        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(0);
        match.Value.ShouldBe(string.Empty);
        match.Index.ShouldBe(0);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);

        match[0].ShouldNotBeNull();
        match[0].Success.ShouldBeTrue();
        match[0].IsDefined.ShouldBeTrue();
        match[0].Value.ShouldBe(string.Empty);
        match[0].Index.ShouldBe(0);
        match[0].EndIndex.ShouldBe(0);
        match[0].Length.ShouldBe(0);
    }

    [Test]
    public void should_match_empty_string_ref()
    {
        var re = new PcreRegex(string.Empty);
        var match = re.Match(default(ReadOnlySpan<char>));

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(0);
        match.Value.ToString().ShouldBeSameAs(string.Empty);
        match.Index.ShouldBe(0);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);

        match[0].Success.ShouldBeTrue();
        match[0].IsDefined.ShouldBeTrue();
        match[0].Value.ToString().ShouldBeSameAs(string.Empty);
        match[0].Index.ShouldBe(0);
        match[0].EndIndex.ShouldBe(0);
        match[0].Length.ShouldBe(0);
    }

    [Test]
    public void should_match_empty_string_buf()
    {
        var re = new PcreRegex(string.Empty);
        var match = re.CreateMatchBuffer().Match(default(ReadOnlySpan<char>));

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(0);
        match.Value.ToString().ShouldBeSameAs(string.Empty);
        match.Index.ShouldBe(0);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);

        match[0].Success.ShouldBeTrue();
        match[0].IsDefined.ShouldBeTrue();
        match[0].Value.ToString().ShouldBeSameAs(string.Empty);
        match[0].Index.ShouldBe(0);
        match[0].EndIndex.ShouldBe(0);
        match[0].Length.ShouldBe(0);
    }

    [Test]
    public void should_match_empty_string_utf8()
    {
        var re = new PcreRegexUtf8(""u8);
        var match = re.Match(default(ReadOnlySpan<byte>));

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(0);
        match.Value.IsEmpty.ShouldBeTrue();
        match.Index.ShouldBe(0);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);

        match[0].Success.ShouldBeTrue();
        match[0].IsDefined.ShouldBeTrue();
        match[0].Value.IsEmpty.ShouldBeTrue();
        match[0].Index.ShouldBe(0);
        match[0].EndIndex.ShouldBe(0);
        match[0].Length.ShouldBe(0);
    }

    [Test]
    public void should_match_empty_string_buf_utf8()
    {
        var re = new PcreRegexUtf8(""u8);
        var match = re.CreateMatchBuffer().Match(default(ReadOnlySpan<byte>));

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(0);
        match.Value.IsEmpty.ShouldBeTrue();
        match.Index.ShouldBe(0);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);

        match[0].Success.ShouldBeTrue();
        match[0].IsDefined.ShouldBeTrue();
        match[0].Value.IsEmpty.ShouldBeTrue();
        match[0].Index.ShouldBe(0);
        match[0].EndIndex.ShouldBe(0);
        match[0].Length.ShouldBe(0);
    }

    [Test]
    public void should_match_empty_string_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("".ToLatin1Bytes());
        var match = re.Match(default(ReadOnlySpan<byte>));

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(0);
        match.Value.IsEmpty.ShouldBeTrue();
        match.Index.ShouldBe(0);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);

        match[0].Success.ShouldBeTrue();
        match[0].IsDefined.ShouldBeTrue();
        match[0].Value.IsEmpty.ShouldBeTrue();
        match[0].Index.ShouldBe(0);
        match[0].EndIndex.ShouldBe(0);
        match[0].Length.ShouldBe(0);
    }

    [Test]
    public void should_match_empty_string_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("".ToLatin1Bytes());
        var match = re.CreateMatchBuffer().Match(default(ReadOnlySpan<byte>));

        match.Success.ShouldBeTrue();
        match.CaptureCount.ShouldBe(0);
        match.Value.IsEmpty.ShouldBeTrue();
        match.Index.ShouldBe(0);
        match.EndIndex.ShouldBe(0);
        match.Length.ShouldBe(0);

        match[0].Success.ShouldBeTrue();
        match[0].IsDefined.ShouldBeTrue();
        match[0].Value.IsEmpty.ShouldBeTrue();
        match[0].Index.ShouldBe(0);
        match[0].EndIndex.ShouldBe(0);
        match[0].Length.ShouldBe(0);
    }

    [Test]
    public void should_have_undefined_value_in_default_ref_group()
    {
        var group = default(PcreRefGroup);

        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.ToString().ShouldBeSameAs(string.Empty);
        group.Index.ShouldBe(-1);
        group.EndIndex.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_have_undefined_value_in_default_ref_group_8bit()
    {
        var group = default(PcreRefGroup8Bit);

        group.Success.ShouldBeFalse();
        group.IsDefined.ShouldBeFalse();
        group.Value.IsEmpty.ShouldBeTrue();
        group.Index.ShouldBe(-1);
        group.EndIndex.ShouldBe(-1);
        group.Length.ShouldBe(0);
    }

    [Test]
    public void should_return_singleton_for_no_match()
    {
        var re = new PcreRegex("foo");
        var matchA = re.Match("bar");
        var matchB = re.Match("baz");

        matchB.ShouldBeSameAs(matchA);
    }

    [Test]
    public void should_preserve_mark_on_no_match()
    {
        var re = new PcreRegex("a(*MARK:foo)b", PcreOptions.NoStartOptimize);

        var match = re.Match("ac");

        match.Success.ShouldBeFalse();
        match.Mark.ShouldBe("foo");
    }

    [Test]
    public void should_preserve_mark_on_no_match_ref()
    {
        var re = new PcreRegex("a(*MARK:foo)b", PcreOptions.NoStartOptimize);

        var match = re.Match("ac".AsSpan());

        match.Success.ShouldBeFalse();
        match.Mark.ShouldBe("foo");
    }

    [Test]
    public void should_preserve_mark_on_no_match_utf8()
    {
        var re = new PcreRegexUtf8("a(*MARK:foo)b"u8, PcreOptions.NoStartOptimize);

        var match = re.Match("ac"u8);

        match.Success.ShouldBeFalse();
        match.Mark.SequenceEqual("foo"u8).ShouldBeTrue();
    }

    [Test]
    public void should_preserve_mark_on_no_match_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("a(*MARK:foo)b".ToLatin1Bytes(), PcreOptions.NoStartOptimize);

        var match = re.Match("ac".ToLatin1Bytes());

        match.Success.ShouldBeFalse();
        match.Mark.SequenceEqual("foo".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    public void should_not_allocate_output_vector_for_no_match_ref()
    {
        var re = new PcreRegex("foo");
        var match = re.Match("bar".AsSpan());

        match.OutputVector.Length.ShouldBe(0);
    }

    [Test]
    public void should_not_allocate_output_vector_for_no_match_utf8()
    {
        var re = new PcreRegexUtf8("foo"u8);
        var match = re.Match("bar"u8);

        match.OutputVector.Length.ShouldBe(0);
    }

    [Test]
    public void should_not_allocate_output_vector_for_no_match_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("foo".ToLatin1Bytes());
        var match = re.Match("bar".ToLatin1Bytes());

        match.OutputVector.Length.ShouldBe(0);
    }

    [Test]
    public unsafe void should_use_buffer_output_vector_for_no_match()
    {
        var re = new PcreRegex("foo");
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("bar".AsSpan());

        Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]).ShouldBeTrue();
    }

    [Test]
    public unsafe void should_use_buffer_output_vector_for_no_match_utf8()
    {
        var re = new PcreRegexUtf8("foo"u8);
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("bar"u8);

        Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]).ShouldBeTrue();
    }

    [Test]
    public unsafe void should_use_buffer_output_vector_for_no_match_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("foo".ToLatin1Bytes());
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("bar".ToLatin1Bytes());

        Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]).ShouldBeTrue();
    }

    [Test]
    public void should_fix_issue_22()
    {
        var regex = new PcreRegex(@"[\w]*[CA]X*B", PcreOptions.Compiled);
        regex.IsMatch("ABC").ShouldBeTrue();
    }

    [Test]
    public void should_fix_pcre_issue_21()
    {
        var regex = new PcreRegex(@"(?P<size>\\d+)m|M", PcreOptions.Compiled);
        regex.Match("4M").Value.ShouldBe("M");
    }

    [Test]
    public void should_throw_on_null_subject()
    {
        var re = new PcreRegex("a");
        Should.Throw<ArgumentNullException>(() => re.Match(default(string)!));
    }

    [Test]
    public void should_throw_on_null_settings()
    {
        var re = new PcreRegex("a");
        Should.Throw<ArgumentNullException>(() => re.Match("a", 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
    }

    [Test]
    public void should_throw_on_null_settings_ref()
    {
        var re = new PcreRegex("a");
        Should.Throw<ArgumentNullException>(() => re.Match("a".AsSpan(), 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
    }

    [Test]
    public void should_throw_on_null_settings_utf8()
    {
        var re = new PcreRegexUtf8("a"u8);
        Should.Throw<ArgumentNullException>(() => re.Match("a"u8, 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
    }

    [Test]
    public void should_throw_on_null_settings_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("a".ToLatin1Bytes());
        Should.Throw<ArgumentNullException>(() => re.Match("a".ToLatin1Bytes(), 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index(int startIndex)
    {
        var re = new PcreRegex(@"a");
        Should.Throw<ArgumentOutOfRangeException>(() => re.Match("a", startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_ref(int startIndex)
    {
        var re = new PcreRegex(@"a");
        Should.Throw<ArgumentOutOfRangeException>(() => re.Match("a".AsSpan(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf(int startIndex)
    {
        var re = new PcreRegex(@"a");
        var buffer = re.CreateMatchBuffer();
        Should.Throw<ArgumentOutOfRangeException>(() => buffer.Match("a".AsSpan(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_utf8(int startIndex)
    {
        var re = new PcreRegexUtf8(@"a"u8);
        Should.Throw<ArgumentOutOfRangeException>(() => re.Match("a"u8, startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf_utf8(int startIndex)
    {
        var re = new PcreRegexUtf8(@"a"u8);
        var buffer = re.CreateMatchBuffer();
        Should.Throw<ArgumentOutOfRangeException>(() => buffer.Match("a"u8, startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_8bit(int startIndex)
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a".ToLatin1Bytes());
        Should.Throw<ArgumentOutOfRangeException>(() => re.Match("a".ToLatin1Bytes(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf_8bit(int startIndex)
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a".ToLatin1Bytes());
        var buffer = re.CreateMatchBuffer();
        Should.Throw<ArgumentOutOfRangeException>(() => buffer.Match("a".ToLatin1Bytes(), startIndex));
    }

    [Test]
    public void should_return_matched_string()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab");

        match.ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_ref()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab".AsSpan());

        match.ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var match = re.Match("ab"u8);

        match.ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(".".ToLatin1Bytes());
        var match = re.Match("ab".ToLatin1Bytes());

        match.ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_from_group()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab");

        match[0].ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_from_group_ref()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab".AsSpan());

        match[0].ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_from_group_buf()
    {
        var re = new PcreRegex(".");
        var buffer = re.CreateMatchBuffer();
        var match = buffer.Match("ab".AsSpan());

        match[0].ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_from_group_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var match = re.Match("ab"u8);

        match[0].ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_from_group_buf_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var buffer = re.CreateMatchBuffer();
        var match = buffer.Match("ab"u8);

        match[0].ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_from_group_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(".".ToLatin1Bytes());
        var match = re.Match("ab".ToLatin1Bytes());

        match[0].ToString().ShouldBe("a");
    }

    [Test]
    public void should_return_matched_string_from_group_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(".".ToLatin1Bytes());
        var buffer = re.CreateMatchBuffer();
        var match = buffer.Match("ab".ToLatin1Bytes());

        match[0].ToString().ShouldBe("a");
    }

    [Test]
    public void should_cast_group_to_string()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab");

        ((string)match[0]).ShouldBe("a");
    }

    [Test]
    public void should_cast_group_to_string_ref()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab".AsSpan());

        ((string)match[0]).ShouldBe("a");
    }

    [Test]
    public void should_cast_group_to_string_buf()
    {
        var re = new PcreRegex(".");
        var buffer = re.CreateMatchBuffer();
        var match = buffer.Match("ab".AsSpan());

        ((string)match[0]).ShouldBe("a");
    }

    [Test]
    public void should_cast_group_to_string_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var match = re.Match("ab"u8);

        ((string)match[0]).ShouldBe("a");
    }

    [Test]
    public void should_cast_group_to_string_buf_utf8()
    {
        var re = new PcreRegexUtf8("."u8);
        var buffer = re.CreateMatchBuffer();
        var match = buffer.Match("ab"u8);

        ((string)match[0]).ShouldBe("a");
    }

    [Test]
    [TestCase(new PcreOptimizationDirective[0], true)]
    [TestCase(new[] { PcreOptimizationDirective.None }, false)]
    [TestCase(new[] { PcreOptimizationDirective.Full }, true)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess }, true)]
    [TestCase(new[] { PcreOptimizationDirective.None, PcreOptimizationDirective.AutoPossess }, true)]
    [TestCase(new[] { PcreOptimizationDirective.Full, PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess, PcreOptimizationDirective.None }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff, PcreOptimizationDirective.Full }, true)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess, PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff, PcreOptimizationDirective.AutoPossess }, true)]
    public void should_use_optimization_settings(PcreOptimizationDirective[] directives, bool expectedAutoPossess)
    {
        var settings = new PcreRegexSettings(PcreOptions.AutoCallout | PcreOptions.NoStartOptimize);

        foreach (var directive in directives)
            settings.OptimizationDirectives.Add(directive);

        var re = new PcreRegex("^a+b", settings);

        var calloutCount = 0;
        re.Match("aac", _ =>
        {
            ++calloutCount;
            return PcreCalloutResult.Pass;
        });

        var autoPossess = calloutCount switch
        {
            3 => true,
            4 => false,
            _ => throw new InvalidOperationException($"Unexpected callout count: {calloutCount}.")
        };

        autoPossess.ShouldBe(expectedAutoPossess);
    }

    [Test]
    [TestCase(new PcreOptimizationDirective[0], true)]
    [TestCase(new[] { PcreOptimizationDirective.None }, false)]
    [TestCase(new[] { PcreOptimizationDirective.Full }, true)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess }, true)]
    [TestCase(new[] { PcreOptimizationDirective.None, PcreOptimizationDirective.AutoPossess }, true)]
    [TestCase(new[] { PcreOptimizationDirective.Full, PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess, PcreOptimizationDirective.None }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff, PcreOptimizationDirective.Full }, true)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess, PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff, PcreOptimizationDirective.AutoPossess }, true)]
    public void should_use_optimization_settings_utf8(PcreOptimizationDirective[] directives, bool expectedAutoPossess)
    {
        var settings = new PcreRegexSettings(PcreOptions.AutoCallout | PcreOptions.NoStartOptimize);

        foreach (var directive in directives)
            settings.OptimizationDirectives.Add(directive);

        var re = new PcreRegexUtf8("^a+b"u8, settings);

        var calloutCount = 0;
        re.Match("aac"u8, _ =>
        {
            ++calloutCount;
            return PcreCalloutResult.Pass;
        });

        var autoPossess = calloutCount switch
        {
            3 => true,
            4 => false,
            _ => throw new InvalidOperationException($"Unexpected callout count: {calloutCount}.")
        };

        autoPossess.ShouldBe(expectedAutoPossess);
    }

    [Test]
    [TestCase(new PcreOptimizationDirective[0], true)]
    [TestCase(new[] { PcreOptimizationDirective.None }, false)]
    [TestCase(new[] { PcreOptimizationDirective.Full }, true)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess }, true)]
    [TestCase(new[] { PcreOptimizationDirective.None, PcreOptimizationDirective.AutoPossess }, true)]
    [TestCase(new[] { PcreOptimizationDirective.Full, PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess, PcreOptimizationDirective.None }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff, PcreOptimizationDirective.Full }, true)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess, PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff, PcreOptimizationDirective.AutoPossess }, true)]
    public void should_use_optimization_settings_8bit(PcreOptimizationDirective[] directives, bool expectedAutoPossess)
    {
        var settings = new PcreRegexSettings(PcreOptions.AutoCallout | PcreOptions.NoStartOptimize);

        foreach (var directive in directives)
            settings.OptimizationDirectives.Add(directive);

        var re = TestSupport.CreatePcreRegex8Bit("^a+b".ToLatin1Bytes(), settings);

        var calloutCount = 0;
        re.Match("aac".ToLatin1Bytes(), _ =>
        {
            ++calloutCount;
            return PcreCalloutResult.Pass;
        });

        var autoPossess = calloutCount switch
        {
            3 => true,
            4 => false,
            _ => throw new InvalidOperationException($"Unexpected callout count: {calloutCount}.")
        };

        autoPossess.ShouldBe(expectedAutoPossess);
    }

    [Test]
    public void should_handle_zero_offset_limit()
    {
        var re = new PcreRegex("b", PcreOptions.UseOffsetLimit);

        var match = re.Match("ab", 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 0
        });

        match.Success.ShouldBeFalse();
    }

    [Test]
    public void pcre2_issue_976()
    {
        // This one should probably fail in a future version
        // https://github.com/PCRE2Project/pcre2/issues/976

        var regex = new PcreRegex("a(?C1)", PcreOptions.AutoCallout);
        var callouts = new List<(int Number, int InfoNumber, int PatternPosition)>();

        _ = regex.Match("a", callout =>
        {
            callouts.Add((callout.Number, callout.Info.Number, callout.PatternPosition));
            return PcreCalloutResult.Pass;
        });

        callouts.ShouldBe([
            (255, 255, 0),
            (1, 1, 6),
            (255, 255, 6)
        ]);
    }

    [Test]
    public void readme_json_example()
    {
        const string jsonPattern =
            """
            (?(DEFINE)
                # An object is an unordered set of name/value pairs.
                (?<object> \{
                    (?: (?&keyvalue) (?: , (?&keyvalue) )* )?
                (?&ws) \} )
                (?<keyvalue>
                    (?&ws) (?&string) (?&ws) : (?&value)
                )

                # An array is an ordered collection of values.
                (?<array> \[
                    (?: (?&value) (?: , (?&value) )* )?
                (?&ws) \] )

                # A value can be a string in double quotes, or a number,
                # or true or false or null, or an object or an array.
                (?<value> (?&ws)
                    (?: (?&string) | (?&number) | (?&object) | (?&array) | true | false | null )
                )

                # A string is a sequence of zero or more Unicode characters,
                # wrapped in double quotes, using backslash escapes.
                (?<string>
                    " (?: [^"\\\p{Cc}]++ | \\u[0-9A-Fa-f]{4} | \\ ["\\/bfnrt] )* "
                    # \p{Cc} matches control characters
                )

                # A number is very much like a C or Java number, except that the octal
                # and hexadecimal formats are not used.
                (?<number>
                    -? (?: 0 | [1-9][0-9]* ) (?: \. [0-9]+ )? (?: [Ee] [-+]? [0-9]+ )?
                )

                # Whitespace
                (?<ws> \s*+ )
            )

            \A (?&ws) (?&object) (?&ws) \z
            """;

        var regex = new PcreRegex(jsonPattern, PcreOptions.IgnorePatternWhitespace | PcreOptions.Compiled);

        //language=json
        const string subject =
            """
            {
                "hello": "world",
                "numbers": [4, 8, 15, 16, 23, 42],
                "foo": null,
                "bar": -2.42e+17,
                "baz": true
            }
            """;

        regex.IsMatch(subject).ShouldBeTrue();
        regex.IsMatch(subject.AsSpan()).ShouldBeTrue();
    }
}
