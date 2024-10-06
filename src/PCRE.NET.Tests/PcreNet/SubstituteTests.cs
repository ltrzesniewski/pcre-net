using System;
using NUnit.Framework;
using PCRE.Internal;

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

        Assert.That(re.Substitute(subject, replacement), Is.EqualTo(result));
        Assert.That(re.Substitute(_filler + subject, replacement), Is.EqualTo(_filler + result));

        Assert.That(re.Substitute(subject.AsSpan(), replacement.AsSpan()), Is.EqualTo(result));
        Assert.That(re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan()), Is.EqualTo(_filler + result));

        Assert.That(re.Substitute(subject, replacement, PcreSubstituteOptions.None, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(result));
        Assert.That(re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.None, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(_filler + result));

        Assert.That(re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.None, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(result));
        Assert.That(re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.None, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(_filler + result));
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

        Assert.That(re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteGlobal), Is.EqualTo(result));
        Assert.That(re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteGlobal), Is.EqualTo(_filler + result));

        Assert.That(re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteGlobal), Is.EqualTo(result));
        Assert.That(re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteGlobal), Is.EqualTo(_filler + result));

        Assert.That(re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteGlobal, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(result));
        Assert.That(re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteGlobal, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(_filler + result));

        Assert.That(re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteGlobal, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(result));
        Assert.That(re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteGlobal, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(_filler + result));
    }

    [Test]
    [TestCase("foo", "$1", "foo")]
    [TestCase("abbc", "$1", "$1")]
    [TestCase(" abbbc abc ", "$1", " $1 abc ")]
    public void should_substitute_literal(string subject, string replacement, string result)
    {
        var re = new PcreRegex("a(b+)c");

        Assert.That(re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteLiteral), Is.EqualTo(result));
        Assert.That(re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteLiteral), Is.EqualTo(_filler + result));

        Assert.That(re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteLiteral), Is.EqualTo(result));
        Assert.That(re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteLiteral), Is.EqualTo(_filler + result));

        Assert.That(re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteLiteral, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(result));
        Assert.That(re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteLiteral, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(_filler + result));

        Assert.That(re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteLiteral, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(result));
        Assert.That(re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteLiteral, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(_filler + result));
    }

    [Test]
    [TestCase("foo", "$1", "")]
    [TestCase("abbc", "$1", "bb")]
    [TestCase(" abbbc abc ", "$1", "bbb")]
    public void should_substitute_replacement_only(string subject, string replacement, string result)
    {
        var re = new PcreRegex("a(b+)c");

        Assert.That(re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteReplacementOnly), Is.EqualTo(result));
        Assert.That(re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteReplacementOnly), Is.EqualTo(result));

        Assert.That(re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteReplacementOnly), Is.EqualTo(result));
        Assert.That(re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteReplacementOnly), Is.EqualTo(result));

        Assert.That(re.Substitute(subject, replacement, PcreSubstituteOptions.SubstituteReplacementOnly, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(result));
        Assert.That(re.Substitute(_filler + subject, replacement, PcreSubstituteOptions.SubstituteReplacementOnly, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(result));

        Assert.That(re.Substitute(subject.AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteReplacementOnly, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(result));
        Assert.That(re.Substitute((_filler + subject).AsSpan(), replacement.AsSpan(), PcreSubstituteOptions.SubstituteReplacementOnly, _ => PcreSubstituteCalloutResult.Pass), Is.EqualTo(result));
    }

    [Test]
    public void should_substitute_mark()
    {
        Assert.That(
            new PcreRegex("(*MARK:pear)apple|(*MARK:orange)lemon").Substitute("apple lemon", "${*MARK}", PcreSubstituteOptions.SubstituteGlobal),
            Is.EqualTo("pear orange")
        );

        Assert.That(
            new PcreRegex("(*MARK:pear)apple|(*MARK:orange)lemon").Substitute("apple lemon", "${*MARK}", PcreSubstituteOptions.SubstituteGlobal, _ => PcreSubstituteCalloutResult.Pass),
            Is.EqualTo("pear orange")
        );
    }

    [Test]
    public void should_substitute_extended()
    {
        var re = new PcreRegex("(some)?(body)");

        Assert.That(
            re.Substitute("body", @"${1:+\U:\L}HeLLo", PcreSubstituteOptions.SubstituteExtended),
            Is.EqualTo("hello")
        );

        Assert.That(
            re.Substitute("somebody", @"${1:+\U:\L}HeLLo", PcreSubstituteOptions.SubstituteExtended),
            Is.EqualTo("HELLO")
        );

        Assert.That(
            re.Substitute("body", @"${1:+\U:\L}HeLLo", PcreSubstituteOptions.SubstituteExtended, _ => PcreSubstituteCalloutResult.Pass),
            Is.EqualTo("hello")
        );

        Assert.That(
            re.Substitute("somebody", @"${1:+\U:\L}HeLLo", PcreSubstituteOptions.SubstituteExtended, _ => PcreSubstituteCalloutResult.Pass),
            Is.EqualTo("HELLO")
        );
    }

    [Test]
    public void should_substitute_from_start_offset()
    {
        var re = new PcreRegex("a(b+)c");

        Assert.That(
            re.Substitute("abc abc abc", "match", 4, PcreSubstituteOptions.SubstituteGlobal),
            Is.EqualTo("abc match match")
        );

        Assert.That(
            re.Substitute("abc abc abc".AsSpan(), "match".AsSpan(), 4, PcreSubstituteOptions.SubstituteGlobal),
            Is.EqualTo("abc match match")
        );
    }

    [Test]
    public void should_return_same_subject_instance_if_possible()
    {
        var re = new PcreRegex("a(b+)c");
        var subject = new string('_', 3);

        Assert.That(re.Substitute(subject, "bar"), Is.SameAs(subject));
    }

    [Test]
    public void should_throw_on_invalid_replacement_syntax()
    {
        var re = new PcreRegex("a(b+)c");

        Assert.Throws<PcreSubstituteException>(() => _ = re.Substitute("abc", "${4}"));
    }

    [Test]
    public void should_handle_offset_limit()
    {
        var re = new PcreRegex(@"bar", PcreOptions.UseOffsetLimit);

        Assert.That(re.Substitute("foobar", "abc", 0, PcreSubstituteOptions.None, null, new PcreMatchSettings { OffsetLimit = 3 }), Is.EqualTo("fooabc"));
        Assert.That(re.Substitute("foobar", "abc", 0, PcreSubstituteOptions.None, null, new PcreMatchSettings { OffsetLimit = 2 }), Is.EqualTo("foobar"));
    }

    [Test]
    public void should_handle_substitution_callouts()
    {
        var re = new PcreRegex(@".");

        Assert.That(
            re.Substitute(
                "abcdefghijklmn",
                "#",
                PcreSubstituteOptions.SubstituteGlobal,
                data => data.SubstitutionCount > 10
                    ? PcreSubstituteCalloutResult.Abort
                    : data.SubstitutionCount % 3 == 0
                        ? PcreSubstituteCalloutResult.Pass
                        : PcreSubstituteCalloutResult.Fail),
            Is.EqualTo("ab#de#gh#jklmn")
        );
    }

    [Test]
    public void should_provide_correct_info_in_callout()
    {
        var re = new PcreRegex(@"foo(bar)?(lol)?baz");

        var result = re.Substitute(
            "abc foobarbaz def",
            "sub",
            PcreSubstituteOptions.None,
            data =>
            {
                Assert.That(data.Match.Success, Is.True);
                Assert.That(data.Match.Index, Is.EqualTo(4));
                Assert.That(data.Match.Length, Is.EqualTo(9));
                Assert.That(data.Match.EndIndex, Is.EqualTo(13));
                Assert.That(data.Match.CaptureCount, Is.EqualTo(2));
                Assert.That(data.Match.IsPartialMatch, Is.False);
                Assert.That(data.Match.Value.ToString(), Is.EqualTo("foobarbaz"));

                Assert.That(data.Match.Groups[0].Success, Is.True);
                Assert.That(data.Match.Groups[0].Index, Is.EqualTo(4));
                Assert.That(data.Match.Groups[0].Length, Is.EqualTo(9));
                Assert.That(data.Match.Groups[0].EndIndex, Is.EqualTo(13));
                Assert.That(data.Match.Groups[0].Value.ToString(), Is.EqualTo("foobarbaz"));

                Assert.That(data.Match.Groups[1].Success, Is.True);
                Assert.That(data.Match.Groups[1].Index, Is.EqualTo(7));
                Assert.That(data.Match.Groups[1].Length, Is.EqualTo(3));
                Assert.That(data.Match.Groups[1].EndIndex, Is.EqualTo(10));
                Assert.That(data.Match.Groups[1].Value.ToString(), Is.EqualTo("bar"));

                Assert.That(data.Match.Groups[2].Success, Is.False);
                Assert.That(data.Match.Groups[2].Index, Is.EqualTo(-1));
                Assert.That(data.Match.Groups[2].Length, Is.EqualTo(0));
                Assert.That(data.Match.Groups[2].EndIndex, Is.EqualTo(-1));
                Assert.That(data.Match.Groups[2].Value.Length, Is.Zero);

                Assert.That(data.Subject.ToString(), Is.EqualTo("abc foobarbaz def"));
                Assert.That(data.Output.ToString(), Is.EqualTo("abc sub"));
                Assert.That(data.Substitution.ToString(), Is.EqualTo("sub"));
                Assert.That(data.SubstitutionCount, Is.EqualTo(1));

                return PcreSubstituteCalloutResult.Pass;
            }
        );

        Assert.That(result, Is.EqualTo("abc sub def"));
    }

    [Test]
    public void should_throw_when_callout_throws()
    {
        var re = new PcreRegex(@".");

        var ex = Assert.Throws<PcreCalloutException>(() => _ = re.Substitute("abc", "def", PcreSubstituteOptions.None, _ => throw new DivideByZeroException("test")))!;

        Assert.That(ex.ErrorCode, Is.EqualTo(PcreErrorCode.Callout));
        Assert.That(ex.InnerException, Is.InstanceOf<DivideByZeroException>());
    }

    [Test]
    public void should_execute_each_callout_once()
    {
        var str = new string('a', InternalRegex.SubstituteBufferSizeInChars * (1 + 2 + 4 + 8 + 16) + 42);
        var re = new PcreRegex("a");

        var execCount = 0;

        var result = re.InternalRegex.Substitute(str.AsSpan(), null, "#:#:#:#".AsSpan(), null, 0, (uint)PcreSubstituteOptions.SubstituteGlobal, data =>
        {
            ++execCount;
            Assert.That(data.SubstitutionCount, Is.EqualTo(execCount));
            Assert.That(data.Match.Index, Is.EqualTo(execCount - 1));
            return execCount % 3 == 0 ? PcreSubstituteCalloutResult.Pass : PcreSubstituteCalloutResult.Fail;
        }, out var substituteCallCount);

        Assert.That(execCount, Is.EqualTo(str.Length));
        Assert.That(result, Is.EqualTo(str.Replace("aaa", "aa#:#:#:#")));
        Assert.That(substituteCallCount, Is.EqualTo(3));
    }

    [Test]
    public void should_call_substitute_once_or_twice_without_callouts()
    {
        var re = new PcreRegex("a");

        var shortStr = new string('a', InternalRegex.SubstituteBufferSizeInChars / 2);
        var longStr = new string('a', InternalRegex.SubstituteBufferSizeInChars * 2);

        re.InternalRegex.Substitute(shortStr.AsSpan(), null, "b".AsSpan(), null, 0, (uint)PcreSubstituteOptions.SubstituteGlobal, null, out var substituteCallCount);
        Assert.That(substituteCallCount, Is.EqualTo(1));

        re.InternalRegex.Substitute(longStr.AsSpan(), null, "b".AsSpan(), null, 0, (uint)PcreSubstituteOptions.SubstituteGlobal, null, out substituteCallCount);
        Assert.That(substituteCallCount, Is.EqualTo(2));
    }

    [Test]
    public void readme_replace_example()
    {
        var result = PcreRegex.Substitute("hello, world!!!", @"\p{P}+", "<$0>", PcreOptions.None, PcreSubstituteOptions.SubstituteGlobal);
        Assert.That(result, Is.EqualTo("hello<,> world<!!!>"));
    }
}
