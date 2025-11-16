using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet;

[TestFixture]
[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
public class IsMatchTests
{
    [Test]
    [TestCase(@"^A.*Z$")]
    [TestCase(@"Foo$")]
    public void should_compile_correct_pattern(string pattern)
    {
        _ = new PcreRegex(pattern);
        Assert.Pass();
    }

    [Test]
    [TestCase(@"^A.*Z$")]
    [TestCase(@"Foo$")]
    public void should_compile_correct_pattern_utf8(string pattern)
    {
        _ = new PcreRegexUtf8(pattern);
        Assert.Pass();
    }

    [Test]
    [TestCase(@"A(B")]
    [TestCase(@"A{3,2}")]
    [TestCase(@"A[B")]
    [TestCase(@"\p{Foo}")]
    public void should_throw_on_invalid_pattern(string pattern)
    {
        try
        {
            _ = new PcreRegex(pattern);
        }
        catch (PcrePatternException)
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    [Test]
    [TestCase(@"A(B")]
    [TestCase(@"A{3,2}")]
    [TestCase(@"A[B")]
    [TestCase(@"\p{Foo}")]
    public void should_throw_on_invalid_pattern_utf8(string pattern)
    {
        try
        {
            _ = new PcreRegexUtf8(pattern);
        }
        catch (PcrePatternException)
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    [Test]
    [TestCase(@"^A.*Z$", "AfooZ")]
    [TestCase(@"^A(.*)Z$", "AfooZ")]
    [TestCase(@"^\p{L}+$", "Abçdë")]
    public void should_match_pattern(string pattern, string subject)
    {
        Assert.That(new PcreRegex(pattern).IsMatch(subject), Is.True);
        Assert.That(new PcreRegex(pattern).IsMatch(subject.AsSpan()), Is.True);
        Assert.That(new PcreRegex(pattern).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.True);
        Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject), Is.True);
        Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject.AsSpan()), Is.True);
        Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.True);
    }

    [Test]
    [TestCase(@"^A.*Z$", "AfooZ")]
    [TestCase(@"^A(.*)Z$", "AfooZ")]
    [TestCase(@"^\p{L}+$", "Abçdë")]
    public void should_match_pattern_utf8(string pattern, string subjectString)
    {
        var subject = Encoding.UTF8.GetBytes(subjectString);

        Assert.That(new PcreRegexUtf8(pattern).IsMatch(subject), Is.True);
        Assert.That(new PcreRegexUtf8(pattern).IsMatch(subject.AsSpan()), Is.True);
        Assert.That(new PcreRegexUtf8(pattern).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.True);
        Assert.That(new PcreRegexUtf8(pattern, PcreOptions.Compiled).IsMatch(subject), Is.True);
        Assert.That(new PcreRegexUtf8(pattern, PcreOptions.Compiled).IsMatch(subject.AsSpan()), Is.True);
        Assert.That(new PcreRegexUtf8(pattern, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.True);
    }

    [Test]
    [TestCase(@"^A.*Z$", "Afoo")]
    [TestCase(@"^\p{L}+$", "Abc123abc")]
    public void should_not_match_pattern(string pattern, string subject)
    {
        Assert.That(new PcreRegex(pattern).IsMatch(subject), Is.False);
        Assert.That(new PcreRegex(pattern).IsMatch(subject.AsSpan()), Is.False);
        Assert.That(new PcreRegex(pattern).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.False);
        Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject), Is.False);
        Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject.AsSpan()), Is.False);
        Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.False);
    }

    [Test]
    [TestCase(@"^A.*Z$", "Afoo")]
    [TestCase(@"^\p{L}+$", "Abc123abc")]
    public void should_not_match_pattern_utf8(string pattern, string subjectString)
    {
        var subject = Encoding.UTF8.GetBytes(subjectString);

        Assert.That(new PcreRegexUtf8(pattern).IsMatch(subject), Is.False);
        Assert.That(new PcreRegexUtf8(pattern).IsMatch(subject.AsSpan()), Is.False);
        Assert.That(new PcreRegexUtf8(pattern).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.False);
        Assert.That(new PcreRegexUtf8(pattern, PcreOptions.Compiled).IsMatch(subject), Is.False);
        Assert.That(new PcreRegexUtf8(pattern, PcreOptions.Compiled).IsMatch(subject.AsSpan()), Is.False);
        Assert.That(new PcreRegexUtf8(pattern, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.False);
    }

    [Test]
    public void should_handle_ignore_case()
    {
        var re = new PcreRegex("aBc");
        Assert.That(re.IsMatch("Abc"), Is.False);
        Assert.That(re.IsMatch("Abc".AsSpan()), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("Abc".AsSpan()), Is.False);

        re = new PcreRegex("aBc", PcreOptions.IgnoreCase);
        Assert.That(re.IsMatch("Abc"), Is.True);
        Assert.That(re.IsMatch("Abc".AsSpan()), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("Abc".AsSpan()), Is.True);
    }

    [Test]
    public void should_handle_ignore_case_utf8()
    {
        var re = new PcreRegexUtf8("aBc"u8);
        Assert.That(re.IsMatch("Abc"u8), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("Abc"u8), Is.False);

        re = new PcreRegexUtf8("aBc"u8, PcreOptions.IgnoreCase);
        Assert.That(re.IsMatch("Abc"u8), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("Abc"u8), Is.True);
    }

    [Test]
    public void should_handle_ignore_whitespace()
    {
        var re = new PcreRegex("^a b$");
        Assert.That(re.IsMatch("ab"), Is.False);
        Assert.That(re.IsMatch("ab".AsSpan()), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("ab".AsSpan()), Is.False);

        re = new PcreRegex("^a b$", PcreOptions.IgnorePatternWhitespace);
        Assert.That(re.IsMatch("ab"), Is.True);
        Assert.That(re.IsMatch("ab".AsSpan()), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("ab".AsSpan()), Is.True);
    }

    [Test]
    public void should_handle_ignore_whitespace_utf8()
    {
        var re = new PcreRegexUtf8("^a b$"u8);
        Assert.That(re.IsMatch("ab"u8), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("ab"u8), Is.False);

        re = new PcreRegexUtf8("^a b$"u8, PcreOptions.IgnorePatternWhitespace);
        Assert.That(re.IsMatch("ab"u8), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("ab"u8), Is.True);
    }

    [Test]
    public void should_handle_singleline()
    {
        var re = new PcreRegex("^a.*b$");
        Assert.That(re.IsMatch("a\r\nb"), Is.False);
        Assert.That(re.IsMatch("a\r\nb".AsSpan()), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("a\r\nb".AsSpan()), Is.False);

        re = new PcreRegex("^a.*b$", PcreOptions.Singleline);
        Assert.That(re.IsMatch("a\r\nb"), Is.True);
        Assert.That(re.IsMatch("a\r\nb".AsSpan()), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("a\r\nb".AsSpan()), Is.True);
    }

    [Test]
    public void should_handle_singleline_utf8()
    {
        var re = new PcreRegexUtf8("^a.*b$"u8);
        Assert.That(re.IsMatch("a\r\nb"u8), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("a\r\nb"u8), Is.False);

        re = new PcreRegexUtf8("^a.*b$"u8, PcreOptions.Singleline);
        Assert.That(re.IsMatch("a\r\nb"u8), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("a\r\nb"u8), Is.True);
    }

    [Test]
    public void should_handle_multiline()
    {
        var re = new PcreRegex("^aaa$");
        Assert.That(re.IsMatch("aaa\r\nbbb"), Is.False);
        Assert.That(re.IsMatch("aaa\r\nbbb".AsSpan()), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("aaa\r\nbbb".AsSpan()), Is.False);

        re = new PcreRegex("^aaa$", PcreOptions.MultiLine);
        Assert.That(re.IsMatch("aaa\r\nbbb"), Is.True);
        Assert.That(re.IsMatch("aaa\r\nbbb".AsSpan()), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("aaa\r\nbbb".AsSpan()), Is.True);
    }

    [Test]
    public void should_handle_multiline_utf8()
    {
        var re = new PcreRegexUtf8("^aaa$"u8);
        Assert.That(re.IsMatch("aaa\r\nbbb"u8), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("aaa\r\nbbb"u8), Is.False);

        re = new PcreRegexUtf8("^aaa$"u8, PcreOptions.MultiLine);
        Assert.That(re.IsMatch("aaa\r\nbbb"u8), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("aaa\r\nbbb"u8), Is.True);
    }

    [Test]
    public void should_handle_javascript()
    {
        var re = new PcreRegex(@"^\U$", PcreOptions.JavaScript);
        Assert.That(re.IsMatch("U"), Is.True);
        Assert.That(re.IsMatch("U".AsSpan()), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("U".AsSpan()), Is.True);

        var ex = Assert.Throws<PcrePatternException>(() => _ = new PcreRegex(@"^\U$"));
        Assert.That(ex!.ErrorCode, Is.EqualTo(PcreErrorCode.UnsupportedEscapeSequence));
    }

    [Test]
    public void should_handle_javascript_utf8()
    {
        var re = new PcreRegexUtf8(@"^\U$"u8, PcreOptions.JavaScript);
        Assert.That(re.IsMatch("U"u8), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("U"u8), Is.True);

        var ex = Assert.Throws<PcrePatternException>(() => _ = new PcreRegexUtf8(@"^\U$"u8));
        Assert.That(ex!.ErrorCode, Is.EqualTo(PcreErrorCode.UnsupportedEscapeSequence));
    }

    [Test]
    public void should_handle_unicode_character_properties()
    {
        var re = new PcreRegex(@"^\w$");
        Assert.That(re.IsMatch("à"), Is.False);
        Assert.That(re.IsMatch("à".AsSpan()), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("à".AsSpan()), Is.False);

        re = new PcreRegex(@"^\w$", PcreOptions.Unicode);
        Assert.That(re.IsMatch("à"), Is.True);
        Assert.That(re.IsMatch("à".AsSpan()), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("à".AsSpan()), Is.True);
    }

    [Test]
    public void should_handle_unicode_character_properties_utf8()
    {
        var re = new PcreRegexUtf8(@"^\w$"u8);
        Assert.That(re.IsMatch("à"u8), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("à"u8), Is.False);

        re = new PcreRegexUtf8(@"^\w$"u8, PcreOptions.Unicode);
        Assert.That(re.IsMatch("à"u8), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("à"u8), Is.True);
    }

    [Test]
    public void should_match_from_index()
    {
        var re = new PcreRegex(@"a");
        Assert.That(re.IsMatch("foobar", 5), Is.False);
        Assert.That(re.IsMatch("foobar".AsSpan(), 5), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("foobar".AsSpan(), 5), Is.False);
    }

    [Test]
    public void should_match_from_index_utf8()
    {
        var re = new PcreRegexUtf8(@"a"u8);
        Assert.That(re.IsMatch("foobar"u8, 5), Is.False);
        Assert.That(re.CreateMatchBuffer().IsMatch("foobar"u8, 5), Is.False);
    }

    [Test]
    public void should_match_starting_at_end_of_string()
    {
        var re = new PcreRegex(@"(?<=a)");
        Assert.That(re.IsMatch("xxa", 3), Is.True);
        Assert.That(re.IsMatch("xxa".AsSpan(), 3), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("xxa".AsSpan(), 3), Is.True);
    }

    [Test]
    public void should_match_starting_at_end_of_string_utf8()
    {
        var re = new PcreRegexUtf8(@"(?<=a)"u8);
        Assert.That(re.IsMatch("xxa"u8, 3), Is.True);
        Assert.That(re.CreateMatchBuffer().IsMatch("xxa"u8, 3), Is.True);
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index(int startIndex)
    {
        var re = new PcreRegex(@"a");
        Assert.Throws<ArgumentOutOfRangeException>(() => re.IsMatch("a", startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_ref(int startIndex)
    {
        var re = new PcreRegex(@"a");
        Assert.Throws<ArgumentOutOfRangeException>(() => re.IsMatch("a".AsSpan(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf(int startIndex)
    {
        var re = new PcreRegex(@"a");
        var buffer = re.CreateMatchBuffer();
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.IsMatch("a".AsSpan(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_utf8(int startIndex)
    {
        var re = new PcreRegexUtf8(@"a"u8);
        Assert.Throws<ArgumentOutOfRangeException>(() => re.IsMatch("a"u8, startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf_utf8(int startIndex)
    {
        var re = new PcreRegexUtf8(@"a"u8);
        var buffer = re.CreateMatchBuffer();
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.IsMatch("a"u8, startIndex));
    }
}
