using System;
using NUnit.Framework;
using PCRE.Internal;
using Shouldly;

namespace PCRE.Tests.PcreNet;

[TestFixture]
public class SubstituteTests
{
    private static readonly string _filler = new(':', InternalRegex.SubstituteBufferSizeInChars * 2);

    [Test]
    [TestCase("foo", "bar", "foo")]
    [TestCase("abbc", "bar", "bar")]
    [TestCase(" abc abc ", "bar", " bar abc ")]
    [TestCase(" abbbc abc ", "$1", " bbb abc ")]
    public void should_substitute_default(string subject, string replacement, string result)
    {
        var re = new PcreRegex("a(b+)c");

        re.Substitute(subject, replacement).ShouldBe(result);
        re.Substitute(_filler + subject, replacement).ShouldBe(_filler + result);

        re.Substitute(subject.AsSpan(), replacement.AsSpan()).ShouldBe(result);
        re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan()).ShouldBe(_filler + result);

        re.Substitute(subject, replacement, PcreSubstituteOptions.None, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(result);
        re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.None, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(_filler + result);

        re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.None, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(result);
        re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.None, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(_filler + result);
    }

    [Test]
    [TestCase("foo", "bar", "foo")]
    [TestCase("abbc", "bar", "bar")]
    [TestCase(" abc abc ", "bar", " bar bar ")]
    [TestCase(" abbbc abc ", "$1", " bbb b ")]
    [TestCase(" abbbc abc ", "$1$$", " bbb$ b$ ")]
    public void should_substitute_global(string subject, string replacement, string result)
    {
        var re = new PcreRegex("a(b+)c");

        re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteGlobal).ShouldBe(result);
        re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteGlobal).ShouldBe(_filler + result);

        re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteGlobal).ShouldBe(result);
        re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteGlobal).ShouldBe(_filler + result);

        re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteGlobal, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(result);
        re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteGlobal, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(_filler + result);

        re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteGlobal, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(result);
        re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteGlobal, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(_filler + result);
    }

    [Test]
    [TestCase("foo", "$1", "foo")]
    [TestCase("abbc", "$1", "$1")]
    [TestCase(" abbbc abc ", "$1", " $1 abc ")]
    public void should_substitute_literal(string subject, string replacement, string result)
    {
        var re = new PcreRegex("a(b+)c");

        re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteLiteral).ShouldBe(result);
        re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteLiteral).ShouldBe(_filler + result);

        re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteLiteral).ShouldBe(result);
        re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteLiteral).ShouldBe(_filler + result);

        re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteLiteral, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(result);
        re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteLiteral, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(_filler + result);

        re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteLiteral, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(result);
        re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteLiteral, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(_filler + result);
    }

    [Test]
    [TestCase("foo", "$1", "")]
    [TestCase("abbc", "$1", "bb")]
    [TestCase(" abbbc abc ", "$1", "bbb")]
    public void should_substitute_replacement_only(string subject, string replacement, string result)
    {
        var re = new PcreRegex("a(b+)c");

        re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteReplacementOnly).ShouldBe(result);
        re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteReplacementOnly).ShouldBe(result);

        re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteReplacementOnly).ShouldBe(result);
        re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteReplacementOnly).ShouldBe(result);

        re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteReplacementOnly, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(result);
        re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteReplacementOnly, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(result);

        re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteReplacementOnly, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(result);
        re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteReplacementOnly, _ => PcreSubstituteCalloutResult.Pass).ShouldBe(result);
    }

    [Test]
    public void should_substitute_mark()
    {
        new PcreRegex("(*MARK:pear)apple|(*MARK:orange)lemon")
            .Substitute("apple lemon", "${*MARK}", PcreSubstituteOptions.SubstituteGlobal)
            .ShouldBe("pear orange");

        new PcreRegex("(*MARK:pear)apple|(*MARK:orange)lemon")
            .Substitute("apple lemon", "${*MARK}", PcreSubstituteOptions.SubstituteGlobal, _ => PcreSubstituteCalloutResult.Pass)
            .ShouldBe("pear orange");
    }

    [Test]
    public void should_substitute_extended()
    {
        var re = new PcreRegex("(some)?(body)");

        re.Substitute("body", @"${1:+\U:\L}HeLLo", PcreSubstituteOptions.SubstituteExtended).ShouldBe("hello");
        re.Substitute("somebody", @"${1:+\U:\L}HeLLo", PcreSubstituteOptions.SubstituteExtended).ShouldBe("HELLO");
        re.Substitute("body", @"${1:+\U:\L}HeLLo", PcreSubstituteOptions.SubstituteExtended, _ => PcreSubstituteCalloutResult.Pass).ShouldBe("hello");
        re.Substitute("somebody", @"${1:+\U:\L}HeLLo", PcreSubstituteOptions.SubstituteExtended, _ => PcreSubstituteCalloutResult.Pass).ShouldBe("HELLO");
    }

    [Test]
    public void should_substitute_from_start_offset()
    {
        var re = new PcreRegex("a(b+)c");

        re.Substitute("abc abc abc", "match", 4, PcreSubstituteOptions.SubstituteGlobal).ShouldBe("abc match match");
        re.Substitute("abc abc abc".AsSpan(), "match".AsSpan(), 4, PcreSubstituteOptions.SubstituteGlobal).ShouldBe("abc match match");
    }

    [Test]
    public void should_return_same_subject_instance_if_possible()
    {
        var re = new PcreRegex("a(b+)c");
        var subject = new string('_', 3);

        re.Substitute(subject, "bar").ShouldBeSameAs(subject);
    }

    [Test]
    public void should_throw_on_invalid_replacement_syntax()
    {
        var re = new PcreRegex("a(b+)c");

        Should.Throw<PcreSubstituteException>(() => re.Substitute("abc", "${4}"));
    }

    [Test]
    public void should_handle_offset_limit()
    {
        var re = new PcreRegex(@"bar", PcreOptions.UseOffsetLimit);

        re.Substitute("foobar", "abc", 0, PcreSubstituteOptions.None, null, new PcreMatchSettings { OffsetLimit = 3 }).ShouldBe("fooabc");
        re.Substitute("foobar", "abc", 0, PcreSubstituteOptions.None, null, new PcreMatchSettings { OffsetLimit = 2 }).ShouldBe("foobar");
    }

    [Test]
    public void should_handle_match_callouts()
    {
        var re = new PcreRegex(@".(?C1)");

        re.Substitute(
            "abcdefghijklmn",
            "#",
            0,
            PcreSubstituteOptions.SubstituteGlobal,
            data => data.CurrentOffset > 10
                ? PcreCalloutResult.Abort
                : data.CurrentOffset % 3 == 0
                    ? PcreCalloutResult.Pass
                    : PcreCalloutResult.Fail,
            null,
            null
        ).ShouldBe("ab#de#gh#jklmn");
    }

    [Test]
    public void should_handle_substitution_callouts()
    {
        var re = new PcreRegex(@".");

        re.Substitute(
            "abcdefghijklmn",
            "#",
            PcreSubstituteOptions.SubstituteGlobal,
            data => data.SubstitutionCount > 10
                ? PcreSubstituteCalloutResult.Abort
                : data.SubstitutionCount % 3 == 0
                    ? PcreSubstituteCalloutResult.Pass
                    : PcreSubstituteCalloutResult.Fail
        ).ShouldBe("ab#de#gh#jklmn");
    }

    [Test]
    public void should_handle_case_substitution_callouts()
    {
        var re = new PcreRegex(@"f(\w+)");

        re.Substitute(
            "abc foo def foo ghi",
            @"F\U$1",
            0,
            PcreSubstituteOptions.SubstituteGlobal | PcreSubstituteOptions.SubstituteExtended,
            null,
            null,
            (input, targetCase) =>
            {
                input.ShouldBe("oo");
                targetCase.ShouldBe(PcreSubstituteCase.Upper);
                return "00";
            },
            null
        ).ShouldBe("abc F00 def F00 ghi");
    }

    [Test]
    public void should_handle_case_substitution_callouts_with_long_result()
    {
        var re = new PcreRegex(@"f(\w+)");
        var substitution = new string('0', 1024 * 1024);

        re.Substitute(
            "abc foo def foo ghi",
            @"F\U$1",
            0,
            PcreSubstituteOptions.SubstituteGlobal | PcreSubstituteOptions.SubstituteExtended,
            null,
            null,
            (input, targetCase) =>
            {
                input.ShouldBe("oo");
                targetCase.ShouldBe(PcreSubstituteCase.Upper);
                return substitution;
            },
            null
        ).ShouldBe($"abc F{substitution} def F{substitution} ghi");
    }

    [Test]
    public void should_provide_correct_info_in_match_callout()
    {
        var re = new PcreRegex(@"foo(bar)?(lol)?baz(?C42)");

        var result = re.Substitute(
            "abc foobarbaz def",
            "sub",
            0,
            PcreSubstituteOptions.None,
            data =>
            {
                data.Match.Success.ShouldBeTrue();
                data.Match.Index.ShouldBe(4);
                data.Match.Length.ShouldBe(9);
                data.Match.EndIndex.ShouldBe(13);
                data.Match.CaptureCount.ShouldBe(2);
                data.Match.IsPartialMatch.ShouldBeFalse();
                data.Match.Value.ShouldBe("foobarbaz");

                data.Match.Groups[0].Success.ShouldBeTrue();
                data.Match.Groups[0].Index.ShouldBe(4);
                data.Match.Groups[0].Length.ShouldBe(9);
                data.Match.Groups[0].EndIndex.ShouldBe(13);
                data.Match.Groups[0].Value.ShouldBe("foobarbaz");

                data.Match.Groups[1].Success.ShouldBeTrue();
                data.Match.Groups[1].Index.ShouldBe(7);
                data.Match.Groups[1].Length.ShouldBe(3);
                data.Match.Groups[1].EndIndex.ShouldBe(10);
                data.Match.Groups[1].Value.ShouldBe("bar");

                data.Match.Groups[2].Success.ShouldBeFalse();
                data.Match.Groups[2].Index.ShouldBe(-1);
                data.Match.Groups[2].Length.ShouldBe(0);
                data.Match.Groups[2].EndIndex.ShouldBe(-1);
                data.Match.Groups[2].Value.Length.ShouldBe(0);

                data.Number.ShouldBe(42);
                data.CurrentOffset.ShouldBe(13);

                return PcreCalloutResult.Pass;
            },
            null,
            null
        );

        result.ShouldBe("abc sub def");
    }

    [Test]
    public void should_provide_correct_info_in_substitute_callout()
    {
        var re = new PcreRegex(@"foo(bar)?(lol)?baz");

        var result = re.Substitute(
            "abc foobarbaz def",
            "sub",
            PcreSubstituteOptions.None,
            data =>
            {
                data.Match.Success.ShouldBeTrue();
                data.Match.Index.ShouldBe(4);
                data.Match.Length.ShouldBe(9);
                data.Match.EndIndex.ShouldBe(13);
                data.Match.CaptureCount.ShouldBe(2);
                data.Match.IsPartialMatch.ShouldBeFalse();
                data.Match.Value.ShouldBe("foobarbaz");

                data.Match.Groups[0].Success.ShouldBeTrue();
                data.Match.Groups[0].Index.ShouldBe(4);
                data.Match.Groups[0].Length.ShouldBe(9);
                data.Match.Groups[0].EndIndex.ShouldBe(13);
                data.Match.Groups[0].Value.ShouldBe("foobarbaz");

                data.Match.Groups[1].Success.ShouldBeTrue();
                data.Match.Groups[1].Index.ShouldBe(7);
                data.Match.Groups[1].Length.ShouldBe(3);
                data.Match.Groups[1].EndIndex.ShouldBe(10);
                data.Match.Groups[1].Value.ShouldBe("bar");

                data.Match.Groups[2].Success.ShouldBeFalse();
                data.Match.Groups[2].Index.ShouldBe(-1);
                data.Match.Groups[2].Length.ShouldBe(0);
                data.Match.Groups[2].EndIndex.ShouldBe(-1);
                data.Match.Groups[2].Value.Length.ShouldBe(0);

                data.Subject.ShouldBe("abc foobarbaz def");
                data.Output.ShouldBe("abc sub");
                data.Substitution.ShouldBe("sub");
                data.SubstitutionCount.ShouldBe(1);

                return PcreSubstituteCalloutResult.Pass;
            }
        );

        result.ShouldBe("abc sub def");
    }

    [Test]
    public void should_throw_when_match_callout_throws()
    {
        var re = new PcreRegex(@"(?C1).");

        var ex = Should.Throw<PcreCalloutException>(() => re.Substitute("abc", "def", 0, PcreSubstituteOptions.None, _ => throw new DivideByZeroException("test"), null, null));

        ex.ErrorCode.ShouldBe(PcreErrorCode.Callout);
        ex.InnerException.ShouldBeAssignableTo<DivideByZeroException>();
    }

    [Test]
    public void should_throw_when_substitute_callout_throws()
    {
        var re = new PcreRegex(@".");

        var ex = Should.Throw<PcreCalloutException>(() => re.Substitute("abc", "def", PcreSubstituteOptions.None, _ => throw new DivideByZeroException("test")));

        ex.ErrorCode.ShouldBe(PcreErrorCode.Callout);
        ex.InnerException.ShouldBeAssignableTo<DivideByZeroException>();
    }

    [Test]
    public void should_throw_when_case_substitute_callout_throws()
    {
        var re = new PcreRegex(@".");

        var ex = Should.Throw<PcreCalloutException>(() => re.Substitute("abc", @"\U$&", 0, PcreSubstituteOptions.SubstituteExtended, null, null, (_, _) => throw new DivideByZeroException("test"), null));

        ex.ErrorCode.ShouldBe(PcreErrorCode.ReplaceCase);
        ex.InnerException.ShouldBeAssignableTo<DivideByZeroException>();
    }

    [Test]
    public void should_execute_each_match_callout_once()
    {
        var str = new string('a', InternalRegex.SubstituteBufferSizeInChars * (1 + 2 + 4 + 8 + 16) + 42);
        var re = new PcreRegex("(?C1)a");

        var execCount = 0;

        var result = re.InternalRegex.Substitute(
            str.AsSpan(),
            null,
            "#:#:#:#".AsSpan(),
            null,
            0,
            (uint)PcreSubstituteOptions.SubstituteGlobal,
            data =>
            {
                ++execCount;
                data.Match.Index.ShouldBe(execCount - 1);
                return execCount % 3 == 0 ? PcreCalloutResult.Pass : PcreCalloutResult.Fail;
            },
            null,
            null,
            out var substituteCallCount
        );

        execCount.ShouldBe(str.Length);
        result.ShouldBe(str.Replace("aaa", "aa#:#:#:#"));
        substituteCallCount.ShouldBe(2u);
    }

    [Test]
    public void should_execute_each_substitute_callout_once()
    {
        var str = new string('a', InternalRegex.SubstituteBufferSizeInChars * (1 + 2 + 4 + 8 + 16) + 42);
        var re = new PcreRegex("a");

        var execCount = 0;

        var result = re.InternalRegex.Substitute(
            str.AsSpan(),
            null,
            "#:#:#:#".AsSpan(),
            null,
            0,
            (uint)PcreSubstituteOptions.SubstituteGlobal,
            null,
            data =>
            {
                ++execCount;
                data.SubstitutionCount.ShouldBe(execCount);
                data.Match.Index.ShouldBe(execCount - 1);
                return execCount % 3 == 0 ? PcreSubstituteCalloutResult.Pass : PcreSubstituteCalloutResult.Fail;
            },
            null,
            out var substituteCallCount
        );

        execCount.ShouldBe(str.Length);
        result.ShouldBe(str.Replace("aaa", "aa#:#:#:#"));
        substituteCallCount.ShouldBe(2u);
    }

    [Test]
    public void should_call_substitute_once_or_twice_without_callouts()
    {
        var re = new PcreRegex("a");

        var shortStr = new string('a', InternalRegex.SubstituteBufferSizeInChars / 2);
        var longStr = new string('a', InternalRegex.SubstituteBufferSizeInChars * 2);

        re.InternalRegex.Substitute(shortStr.AsSpan(), null, "b".AsSpan(), null, 0, (uint)PcreSubstituteOptions.SubstituteGlobal, null, null, null, out var substituteCallCount);
        substituteCallCount.ShouldBe(1u);

        re.InternalRegex.Substitute(longStr.AsSpan(), null, "b".AsSpan(), null, 0, (uint)PcreSubstituteOptions.SubstituteGlobal, null, null, null, out substituteCallCount);
        substituteCallCount.ShouldBe(2u);
    }

    [Test]
    public void should_stop_replacing()
    {
        var re = new PcreRegex(".");

        var calls = 0;
        var result = re.Substitute("abc", "#", PcreSubstituteOptions.SubstituteGlobal, _ => ++calls == 1 ? PcreSubstituteCalloutResult.Abort : PcreSubstituteCalloutResult.Pass);

        result.ShouldBe("abc");
        calls.ShouldBe(1);
    }

    [Test]
    public void readme_replace_example()
    {
        var result = PcreRegex.Substitute("hello, world!!!", @"\p{P}+", "<$0>", PcreOptions.None, PcreSubstituteOptions.SubstituteGlobal);
        result.ShouldBe("hello<,> world<!!!>");
    }
}
