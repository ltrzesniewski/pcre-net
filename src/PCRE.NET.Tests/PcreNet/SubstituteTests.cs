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
    }

    [Test]
    public void should_substitute_mark()
    {
        Assert.That(
            new PcreRegex("(*MARK:pear)apple|(*MARK:orange)lemon").Substitute("apple lemon", "${*MARK}", PcreSubstituteOptions.SubstituteGlobal),
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
    }

    [Test]
    public void should_substitute_from_start_offset()
    {
        var re = new PcreRegex("a(b+)c");

        Assert.That(
            re.Substitute("abc abc abc", "match", PcreSubstituteOptions.SubstituteGlobal, 4),
            Is.EqualTo("abc match match")
        );

        Assert.That(
            re.Substitute("abc abc abc".AsSpan(), "match".AsSpan(), PcreSubstituteOptions.SubstituteGlobal, 4),
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
}
