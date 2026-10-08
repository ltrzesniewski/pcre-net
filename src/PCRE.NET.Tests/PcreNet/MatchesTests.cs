using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using PCRE.Tests.Support;
using Shouldly;

namespace PCRE.Tests.PcreNet;

[TestFixture]
[SuppressMessage("ReSharper", "ArrangeDefaultValueWhenTypeNotEvident")]
[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
public class MatchesTests
{
    [Test]
    public void should_return_all_matches()
    {
        var re = new PcreRegex(@"a(b)a");
        var matches = re.Matches("foo aba bar aba baz").ToList();

        matches.ShouldHaveCount(2);

        matches[0].Value.ShouldBe("aba");
        matches[0].ValueSpan.ShouldBe("aba");
        matches[0].Index.ShouldBe(4);
        matches[0].Length.ShouldBe(3);

        matches[0][1].Value.ShouldBe("b");
        matches[0][1].ValueSpan.ShouldBe("b");
        matches[0][1].Index.ShouldBe(5);
        matches[0][1].Length.ShouldBe(1);

        matches[1].Value.ShouldBe("aba");
        matches[1].ValueSpan.ShouldBe("aba");
        matches[1].Index.ShouldBe(12);
        matches[1].Length.ShouldBe(3);

        matches[1][1].Value.ShouldBe("b");
        matches[1][1].ValueSpan.ShouldBe("b");
        matches[1][1].Index.ShouldBe(13);
        matches[1][1].Length.ShouldBe(1);
    }

    [Test]
    public void should_return_all_matches_ref()
    {
        var re = new PcreRegex(@"a(b)a");
        var matches = re.Matches("foo aba bar aba baz".AsSpan())
                        .ToList(m => (Value: m.Value.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Value.ShouldBe("aba");
        matches[0].Index.ShouldBe(4);
        matches[0].Length.ShouldBe(3);

        matches[0].Groups[1].Value.ShouldBe("b");
        matches[0].Groups[1].Index.ShouldBe(5);
        matches[0].Groups[1].Length.ShouldBe(1);

        matches[1].Value.ShouldBe("aba");
        matches[1].Index.ShouldBe(12);
        matches[1].Length.ShouldBe(3);

        matches[1].Groups[1].Value.ShouldBe("b");
        matches[1].Groups[1].Index.ShouldBe(13);
        matches[1].Groups[1].Length.ShouldBe(1);
    }

    [Test]
    public void should_return_all_matches_buf()
    {
        var re = new PcreRegex(@"a(b)a");
        var matches = re.CreateMatchBuffer()
                        .Matches("foo aba bar aba baz".AsSpan())
                        .ToList(m => (Value: m.Value.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Value.ShouldBe("aba");
        matches[0].Index.ShouldBe(4);
        matches[0].Length.ShouldBe(3);

        matches[0].Groups[1].Value.ShouldBe("b");
        matches[0].Groups[1].Index.ShouldBe(5);
        matches[0].Groups[1].Length.ShouldBe(1);

        matches[1].Value.ShouldBe("aba");
        matches[1].Index.ShouldBe(12);
        matches[1].Length.ShouldBe(3);

        matches[1].Groups[1].Value.ShouldBe("b");
        matches[1].Groups[1].Index.ShouldBe(13);
        matches[1].Groups[1].Length.ShouldBe(1);
    }

    [Test]
    public void should_return_all_matches_utf8()
    {
        var re = new PcreRegexUtf8(@"a(b)a"u8);
        var matches = re.Matches("foo aba bar aba baz"u8)
                        .ToList(m => (Value: m.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Value.ShouldBe("aba");
        matches[0].Index.ShouldBe(4);
        matches[0].Length.ShouldBe(3);

        matches[0].Groups[1].Value.ShouldBe("b");
        matches[0].Groups[1].Index.ShouldBe(5);
        matches[0].Groups[1].Length.ShouldBe(1);

        matches[1].Value.ShouldBe("aba");
        matches[1].Index.ShouldBe(12);
        matches[1].Length.ShouldBe(3);

        matches[1].Groups[1].Value.ShouldBe("b");
        matches[1].Groups[1].Index.ShouldBe(13);
        matches[1].Groups[1].Length.ShouldBe(1);
    }

    [Test]
    public void should_return_all_matches_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"a(b)a"u8);
        var matches = re.CreateMatchBuffer()
                        .Matches("foo aba bar aba baz"u8)
                        .ToList(m => (Value: m.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Value.ShouldBe("aba");
        matches[0].Index.ShouldBe(4);
        matches[0].Length.ShouldBe(3);

        matches[0].Groups[1].Value.ShouldBe("b");
        matches[0].Groups[1].Index.ShouldBe(5);
        matches[0].Groups[1].Length.ShouldBe(1);

        matches[1].Value.ShouldBe("aba");
        matches[1].Index.ShouldBe(12);
        matches[1].Length.ShouldBe(3);

        matches[1].Groups[1].Value.ShouldBe("b");
        matches[1].Groups[1].Index.ShouldBe(13);
        matches[1].Groups[1].Length.ShouldBe(1);
    }

    [Test]
    public void should_return_all_matches_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a(b)a".ToLatin1Bytes());
        var matches = re.Matches("foo aba bar aba baz".ToLatin1Bytes())
                        .ToList(m => (Value: m.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Value.ShouldBe("aba");
        matches[0].Index.ShouldBe(4);
        matches[0].Length.ShouldBe(3);

        matches[0].Groups[1].Value.ShouldBe("b");
        matches[0].Groups[1].Index.ShouldBe(5);
        matches[0].Groups[1].Length.ShouldBe(1);

        matches[1].Value.ShouldBe("aba");
        matches[1].Index.ShouldBe(12);
        matches[1].Length.ShouldBe(3);

        matches[1].Groups[1].Value.ShouldBe("b");
        matches[1].Groups[1].Index.ShouldBe(13);
        matches[1].Groups[1].Length.ShouldBe(1);
    }

    [Test]
    public void should_return_all_matches_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a(b)a".ToLatin1Bytes());
        var matches = re.CreateMatchBuffer()
                        .Matches("foo aba bar aba baz".ToLatin1Bytes())
                        .ToList(m => (Value: m.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Value.ShouldBe("aba");
        matches[0].Index.ShouldBe(4);
        matches[0].Length.ShouldBe(3);

        matches[0].Groups[1].Value.ShouldBe("b");
        matches[0].Groups[1].Index.ShouldBe(5);
        matches[0].Groups[1].Length.ShouldBe(1);

        matches[1].Value.ShouldBe("aba");
        matches[1].Index.ShouldBe(12);
        matches[1].Length.ShouldBe(3);

        matches[1].Groups[1].Value.ShouldBe("b");
        matches[1].Groups[1].Index.ShouldBe(13);
        matches[1].Groups[1].Length.ShouldBe(1);
    }

    [Test]
    public void should_handle_empty_matches()
    {
        var re = new PcreRegex(@"(?=(a))");
        var matches = re.Matches("aaabbaa").ToList();

        matches.ShouldHaveCount(5);

        matches.Select(m => m.Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Length == 0);
        matches.ShouldAllBe(m => m.Value == string.Empty);
        matches.Select(m => m.ValueSpan.Length).ShouldAllBe(i => i == 0);

        matches.Select(m => m[1].Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m[1].Length == 1);
        matches.ShouldAllBe(m => m[1].Value == "a");
        matches.Select(m => m[1].ValueSpan.ToString()).ShouldAllBe(i => i == "a");
    }

    [Test]
    public void should_handle_empty_matches_ref()
    {
        var re = new PcreRegex(@"(?=(a))");
        var matches = re.Matches("aaabbaa".AsSpan())
                        .ToList(m => (Value: m.Value.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(5);

        matches.Select(m => m.Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Length == 0);
        matches.ShouldAllBe(m => m.Value == string.Empty);

        matches.Select(m => m.Groups[1].Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Groups[1].Length == 1);
        matches.ShouldAllBe(m => m.Groups[1].Value == "a");
    }

    [Test]
    public void should_handle_empty_matches_buf()
    {
        var re = new PcreRegex(@"(?=(a))");
        var matches = re.CreateMatchBuffer().Matches("aaabbaa".AsSpan())
                        .ToList(m => (Value: m.Value.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(5);

        matches.Select(m => m.Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Length == 0);
        matches.ShouldAllBe(m => m.Value == string.Empty);

        matches.Select(m => m.Groups[1].Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Groups[1].Length == 1);
        matches.ShouldAllBe(m => m.Groups[1].Value == "a");
    }

    [Test]
    public void should_handle_empty_matches_utf8()
    {
        var re = new PcreRegexUtf8(@"(?=(a))"u8);
        var matches = re.Matches("aaabbaa"u8)
                        .ToList(m => (Value: m.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(5);

        matches.Select(m => m.Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Length == 0);
        matches.ShouldAllBe(m => m.Value == string.Empty);

        matches.Select(m => m.Groups[1].Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Groups[1].Length == 1);
        matches.ShouldAllBe(m => m.Groups[1].Value == "a");
    }

    [Test]
    public void should_handle_empty_matches_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"(?=(a))"u8);
        var matches = re.CreateMatchBuffer().Matches("aaabbaa"u8)
                        .ToList(m => (Value: m.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(5);

        matches.Select(m => m.Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Length == 0);
        matches.ShouldAllBe(m => m.Value == string.Empty);

        matches.Select(m => m.Groups[1].Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Groups[1].Length == 1);
        matches.ShouldAllBe(m => m.Groups[1].Value == "a");
    }

    [Test]
    public void should_handle_empty_matches_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?=(a))".ToLatin1Bytes());
        var matches = re.Matches("aaabbaa".ToLatin1Bytes())
                        .ToList(m => (Value: m.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(5);

        matches.Select(m => m.Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Length == 0);
        matches.ShouldAllBe(m => m.Value == string.Empty);

        matches.Select(m => m.Groups[1].Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Groups[1].Length == 1);
        matches.ShouldAllBe(m => m.Groups[1].Value == "a");
    }

    [Test]
    public void should_handle_empty_matches_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?=(a))".ToLatin1Bytes());
        var matches = re.CreateMatchBuffer().Matches("aaabbaa".ToLatin1Bytes())
                        .ToList(m => (Value: m.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(5);

        matches.Select(m => m.Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Length == 0);
        matches.ShouldAllBe(m => m.Value == string.Empty);

        matches.Select(m => m.Groups[1].Index).ShouldBe([0, 1, 2, 5, 6]);
        matches.ShouldAllBe(m => m.Groups[1].Length == 1);
        matches.ShouldAllBe(m => m.Groups[1].Value == "a");
    }

    [Test]
    public void should_match_from_index()
    {
        var re = new PcreRegex(@"a");
        var matches = re.Matches("foo bar baz", 6).ToList();

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_from_index_ref()
    {
        var re = new PcreRegex(@"a");
        var matches = re.Matches("foo bar baz".AsSpan(), 6).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_from_index_buf()
    {
        var re = new PcreRegex(@"a");
        var matches = re.CreateMatchBuffer().Matches("foo bar baz".AsSpan(), 6).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_from_index_utf8()
    {
        var re = new PcreRegexUtf8(@"a"u8);
        var matches = re.Matches("foo bar baz"u8, 6).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_from_index_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"a"u8);
        var matches = re.CreateMatchBuffer().Matches("foo bar baz"u8, 6).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_from_index_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a".ToLatin1Bytes());
        var matches = re.Matches("foo bar baz".ToLatin1Bytes(), 6).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_from_index_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a".ToLatin1Bytes());
        var matches = re.CreateMatchBuffer().Matches("foo bar baz".ToLatin1Bytes(), 6).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_starting_at_end_of_string()
    {
        var re = new PcreRegex(@"(?<=a)");
        var matches = re.Matches("xxa", 3).ToList();

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_starting_at_end_of_string_ref()
    {
        var re = new PcreRegex(@"(?<=a)");
        var matches = re.Matches("xxa".AsSpan(), 3).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_starting_at_end_of_string_buf()
    {
        var re = new PcreRegex(@"(?<=a)");
        var matches = re.CreateMatchBuffer().Matches("xxa".AsSpan(), 3).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_starting_at_end_of_string_utf8()
    {
        var re = new PcreRegexUtf8(@"(?<=a)"u8);
        var matches = re.Matches("xxa"u8, 3).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_starting_at_end_of_string_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"(?<=a)"u8);
        var matches = re.CreateMatchBuffer().Matches("xxa"u8, 3).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_starting_at_end_of_string_8bit()
    {
        var re = new PcreRegexUtf8(@"(?<=a)".ToLatin1Bytes());
        var matches = re.Matches("xxa".ToLatin1Bytes(), 3).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_match_starting_at_end_of_string_buf_8bit()
    {
        var re = new PcreRegexUtf8(@"(?<=a)".ToLatin1Bytes());
        var matches = re.CreateMatchBuffer().Matches("xxa".ToLatin1Bytes(), 3).ToList(_ => true);

        matches.ShouldHaveSingleItem();
    }

    [Test]
    public void should_handle_end_before_start()
    {
        var re = new PcreRegex(@"(?=a+b\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });
        var matches = re.Matches("aaabab").ToList();

        matches.ShouldHaveCount(2);

        matches[0].ShouldNotBeNull();
        matches[0].Index.ShouldBe(4);
        matches[0].EndIndex.ShouldBe(0);
        matches[0].Length.ShouldBe(0);
        matches[0].Value.ShouldBeEmpty();
        matches[0].ValueSpan.IsEmpty.ShouldBeTrue();

        matches[1].ShouldNotBeNull();
        matches[1].Index.ShouldBe(6);
        matches[1].EndIndex.ShouldBe(4);
        matches[1].Length.ShouldBe(0);
        matches[1].Value.ShouldBeEmpty();
        matches[1].ValueSpan.IsEmpty.ShouldBeTrue();
    }

    [Test]
    public void should_handle_end_before_start_ref()
    {
        var re = new PcreRegex(@"(?=a+b\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });
        var matches = re.Matches("aaabab".AsSpan())
                        .ToList(m => (Value: m.Value.ToString(), m.Index, m.EndIndex, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Index.ShouldBe(4);
        matches[0].EndIndex.ShouldBe(0);
        matches[0].Length.ShouldBe(0);
        matches[0].Value.ShouldBeEmpty();

        matches[1].Index.ShouldBe(6);
        matches[1].EndIndex.ShouldBe(4);
        matches[1].Length.ShouldBe(0);
        matches[1].Value.ShouldBeEmpty();
    }

    [Test]
    public void should_handle_end_before_start_buf()
    {
        var re = new PcreRegex(@"(?=a+b\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });
        var matches = re.CreateMatchBuffer().Matches("aaabab".AsSpan())
                        .ToList(m => (Value: m.Value.ToString(), m.Index, m.EndIndex, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Index.ShouldBe(4);
        matches[0].EndIndex.ShouldBe(0);
        matches[0].Length.ShouldBe(0);
        matches[0].Value.ShouldBeEmpty();

        matches[1].Index.ShouldBe(6);
        matches[1].EndIndex.ShouldBe(4);
        matches[1].Length.ShouldBe(0);
        matches[1].Value.ShouldBeEmpty();
    }

    [Test]
    public void should_handle_end_before_start_utf8()
    {
        var re = new PcreRegexUtf8(@"(?=a+b\K)"u8, new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });
        var matches = re.Matches("aaabab"u8)
                        .ToList(m => (Value: m.ToString(), m.Index, m.EndIndex, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Index.ShouldBe(4);
        matches[0].EndIndex.ShouldBe(0);
        matches[0].Length.ShouldBe(0);
        matches[0].Value.ShouldBeEmpty();

        matches[1].Index.ShouldBe(6);
        matches[1].EndIndex.ShouldBe(4);
        matches[1].Length.ShouldBe(0);
        matches[1].Value.ShouldBeEmpty();
    }

    [Test]
    public void should_handle_end_before_start_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"(?=a+b\K)"u8, new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });
        var matches = re.CreateMatchBuffer().Matches("aaabab"u8)
                        .ToList(m => (Value: m.ToString(), m.Index, m.EndIndex, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Index.ShouldBe(4);
        matches[0].EndIndex.ShouldBe(0);
        matches[0].Length.ShouldBe(0);
        matches[0].Value.ShouldBeEmpty();

        matches[1].Index.ShouldBe(6);
        matches[1].EndIndex.ShouldBe(4);
        matches[1].Length.ShouldBe(0);
        matches[1].Value.ShouldBeEmpty();
    }

    [Test]
    public void should_handle_end_before_start_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?=a+b\K)".ToLatin1Bytes(), new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });
        var matches = re.Matches("aaabab".ToLatin1Bytes())
                        .ToList(m => (Value: m.ToString(), m.Index, m.EndIndex, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Index.ShouldBe(4);
        matches[0].EndIndex.ShouldBe(0);
        matches[0].Length.ShouldBe(0);
        matches[0].Value.ShouldBeEmpty();

        matches[1].Index.ShouldBe(6);
        matches[1].EndIndex.ShouldBe(4);
        matches[1].Length.ShouldBe(0);
        matches[1].Value.ShouldBeEmpty();
    }

    [Test]
    public void should_handle_end_before_start_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?=a+b\K)".ToLatin1Bytes(), new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });
        var matches = re.CreateMatchBuffer().Matches("aaabab".ToLatin1Bytes())
                        .ToList(m => (Value: m.ToString(), m.Index, m.EndIndex, m.Length, Groups: m.Groups.ToList(g => (Value: g.ToString(), g.Index, g.Length))));

        matches.ShouldHaveCount(2);

        matches[0].Index.ShouldBe(4);
        matches[0].EndIndex.ShouldBe(0);
        matches[0].Length.ShouldBe(0);
        matches[0].Value.ShouldBeEmpty();

        matches[1].Index.ShouldBe(6);
        matches[1].EndIndex.ShouldBe(4);
        matches[1].Length.ShouldBe(0);
        matches[1].Value.ShouldBeEmpty();
    }

    [Test]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public void should_report_callout_exception()
    {
        var re = new PcreRegex(@"a(?C1)");

        var resultCount = 0;

        var seq = re.Matches(
            "aaa",
            0,
            callout =>
            {
                if (callout.StartOffset >= 2)
                    throw new InvalidOperationException("Simulated exception");

                return PcreCalloutResult.Pass;
            }
        ).Select(i =>
            {
                ++resultCount;
                return i;
            }
        );

        Should.Throw<PcreCalloutException>(() => seq.ToList());
        resultCount.ShouldBe(2);
    }

    [Test]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public void should_report_callout_exception_ref()
    {
        var re = new PcreRegex(@"a(?C1)");

        var resultCount = 0;

        Should.Throw<PcreCalloutException>(() =>
            {
                re.Matches(
                    "aaa".AsSpan(),
                    0,
                    callout =>
                    {
                        if (callout.StartOffset >= 2)
                            throw new InvalidOperationException("Simulated exception");

                        return PcreCalloutResult.Pass;
                    }
                ).ToList(_ =>
                    {
                        ++resultCount;
                        return true;
                    }
                );
            }
        );

        resultCount.ShouldBe(2);
    }

    [Test]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public void should_report_callout_exception_buf()
    {
        var re = new PcreRegex(@"a(?C1)");
        var buffer = re.CreateMatchBuffer();

        var resultCount = 0;

        Should.Throw<PcreCalloutException>(() =>
            {
                buffer.Matches(
                    "aaa".AsSpan(),
                    0,
                    callout =>
                    {
                        if (callout.StartOffset >= 2)
                            throw new InvalidOperationException("Simulated exception");

                        return PcreCalloutResult.Pass;
                    }
                ).ToList(_ =>
                    {
                        ++resultCount;
                        return true;
                    }
                );
            }
        );

        resultCount.ShouldBe(2);
    }

    [Test]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public void should_report_callout_exception_utf8()
    {
        var re = new PcreRegexUtf8(@"a(?C1)"u8);

        var resultCount = 0;

        Should.Throw<PcreCalloutException>(() =>
            {
                re.Matches(
                    "aaa"u8,
                    0,
                    callout =>
                    {
                        if (callout.StartOffset >= 2)
                            throw new InvalidOperationException("Simulated exception");

                        return PcreCalloutResult.Pass;
                    }
                ).ToList(_ =>
                    {
                        ++resultCount;
                        return true;
                    }
                );
            }
        );

        resultCount.ShouldBe(2);
    }

    [Test]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public void should_report_callout_exception_buf_utf8()
    {
        var re = new PcreRegexUtf8(@"a(?C1)"u8);
        var buffer = re.CreateMatchBuffer();

        var resultCount = 0;

        Should.Throw<PcreCalloutException>(() =>
            {
                buffer.Matches(
                    "aaa"u8,
                    0,
                    callout =>
                    {
                        if (callout.StartOffset >= 2)
                            throw new InvalidOperationException("Simulated exception");

                        return PcreCalloutResult.Pass;
                    }
                ).ToList(_ =>
                    {
                        ++resultCount;
                        return true;
                    }
                );
            }
        );

        resultCount.ShouldBe(2);
    }

    [Test]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public void should_report_callout_exception_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a(?C1)".ToLatin1Bytes());

        var resultCount = 0;

        Should.Throw<PcreCalloutException>(() =>
            {
                re.Matches(
                    "aaa".ToLatin1Bytes(),
                    0,
                    callout =>
                    {
                        if (callout.StartOffset >= 2)
                            throw new InvalidOperationException("Simulated exception");

                        return PcreCalloutResult.Pass;
                    }
                ).ToList(_ =>
                    {
                        ++resultCount;
                        return true;
                    }
                );
            }
        );

        resultCount.ShouldBe(2);
    }

    [Test]
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    public void should_report_callout_exception_buf_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a(?C1)".ToLatin1Bytes());
        var buffer = re.CreateMatchBuffer();

        var resultCount = 0;

        Should.Throw<PcreCalloutException>(() =>
            {
                buffer.Matches(
                    "aaa".ToLatin1Bytes(),
                    0,
                    callout =>
                    {
                        if (callout.StartOffset >= 2)
                            throw new InvalidOperationException("Simulated exception");

                        return PcreCalloutResult.Pass;
                    }
                ).ToList(_ =>
                    {
                        ++resultCount;
                        return true;
                    }
                );
            }
        );

        resultCount.ShouldBe(2);
    }

    [Test]
    public void should_throw_on_null_subject()
    {
        var re = new PcreRegex("a");
        Should.Throw<ArgumentNullException>(() => re.Matches(default(string)!));
    }

    [Test]
    public void should_throw_on_null_settings()
    {
        var re = new PcreRegex("a");
        Should.Throw<ArgumentNullException>(() => re.Matches("a", 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
    }

    [Test]
    public void should_throw_on_null_settings_ref()
    {
        var re = new PcreRegex("a");
        Should.Throw<ArgumentNullException>(() => re.Matches("a".AsSpan(), 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
    }

    [Test]
    public void should_throw_on_null_settings_utf8()
    {
        var re = new PcreRegexUtf8("a"u8);
        Should.Throw<ArgumentNullException>(() => re.Matches("a"u8, 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
    }

    [Test]
    public void should_throw_on_null_settings_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("a"u8);
        Should.Throw<ArgumentNullException>(() => re.Matches("a".ToLatin1Bytes(), 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index(int startIndex)
    {
        var re = new PcreRegex(@"a");
        Should.Throw<ArgumentOutOfRangeException>(() => re.Matches("a", startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_ref(int startIndex)
    {
        var re = new PcreRegex(@"a");
        Should.Throw<ArgumentOutOfRangeException>(() => re.Matches("a".AsSpan(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf(int startIndex)
    {
        var re = new PcreRegex(@"a");
        var buffer = re.CreateMatchBuffer();
        Should.Throw<ArgumentOutOfRangeException>(() => buffer.Matches("a".AsSpan(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_utf8(int startIndex)
    {
        var re = new PcreRegexUtf8(@"a"u8);
        Should.Throw<ArgumentOutOfRangeException>(() => re.Matches("a"u8, startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf_utf8(int startIndex)
    {
        var re = new PcreRegexUtf8(@"a"u8);
        var buffer = re.CreateMatchBuffer();
        Should.Throw<ArgumentOutOfRangeException>(() => buffer.Matches("a"u8, startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_8bit(int startIndex)
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a".ToLatin1Bytes());
        Should.Throw<ArgumentOutOfRangeException>(() => re.Matches("a".ToLatin1Bytes(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf_8bit(int startIndex)
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a".ToLatin1Bytes());
        var buffer = re.CreateMatchBuffer();
        Should.Throw<ArgumentOutOfRangeException>(() => buffer.Matches("a".ToLatin1Bytes(), startIndex));
    }

    [Test]
    public void readme_backtracking_verbs_example()
    {
        var matches = PcreRegex.Matches("(foo) bar (baz) 42", @"\(\w+\)(*SKIP)(*FAIL)|\w+")
                               .Select(m => m.Value)
                               .ToList();

        matches.ShouldBe(["bar", "42"]);
    }
}
