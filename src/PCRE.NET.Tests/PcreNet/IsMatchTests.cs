using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NUnit.Framework;
using PCRE.Tests.Support;
using Shouldly;

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
        Should.NotThrow(() => new PcreRegex(pattern));
    }

    [Test]
    [TestCase(@"^A.*Z$")]
    [TestCase(@"Foo$")]
    public void should_compile_correct_pattern_utf8(string pattern)
    {
        Should.NotThrow(() => new PcreRegexUtf8(pattern));
    }

    [Test]
    [TestCase(@"^A.*Z$")]
    [TestCase(@"Foo$")]
    public void should_compile_correct_pattern_8bit(string pattern)
    {
        Should.NotThrow(() => new PcreRegex8Bit(pattern.ToLatin1Bytes(), TestSupport.Latin1Encoding));
    }

    [Test]
    [TestCase(@"A(B")]
    [TestCase(@"A{3,2}")]
    [TestCase(@"A[B")]
    [TestCase(@"\p{Foo}")]
    public void should_throw_on_invalid_pattern(string pattern)
    {
        Should.Throw<PcrePatternException>(() => new PcreRegex(pattern));
    }

    [Test]
    [TestCase(@"A(B")]
    [TestCase(@"A{3,2}")]
    [TestCase(@"A[B")]
    [TestCase(@"\p{Foo}")]
    public void should_throw_on_invalid_pattern_utf8(string pattern)
    {
        Should.Throw<PcrePatternException>(() => new PcreRegexUtf8(pattern));
    }

    [Test]
    [TestCase(@"A(B")]
    [TestCase(@"A{3,2}")]
    [TestCase(@"A[B")]
    [TestCase(@"\p{Foo}")]
    public void should_throw_on_invalid_pattern_8bit(string pattern)
    {
        Should.Throw<PcrePatternException>(() => new PcreRegex8Bit(pattern.ToLatin1Bytes(), TestSupport.Latin1Encoding));
    }

    [Test]
    [TestCase(@"^A.*Z$", "AfooZ")]
    [TestCase(@"^A(.*)Z$", "AfooZ")]
    [TestCase(@"^\p{L}+$", "Abçdë")]
    public void should_match_pattern(string pattern, string subject)
    {
        new PcreRegex(pattern).IsMatch(subject).ShouldBeTrue();
        new PcreRegex(pattern).IsMatch(subject.AsSpan()).ShouldBeTrue();
        new PcreRegex(pattern).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeTrue();
        new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject).ShouldBeTrue();
        new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject.AsSpan()).ShouldBeTrue();
        new PcreRegex(pattern, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeTrue();
    }

    [Test]
    [TestCase(@"^A.*Z$", "AfooZ")]
    [TestCase(@"^A(.*)Z$", "AfooZ")]
    [TestCase(@"^\p{L}+$", "Abçdë")]
    public void should_match_pattern_utf8(string pattern, string subjectString)
    {
        var subject = Encoding.UTF8.GetBytes(subjectString);

        new PcreRegexUtf8(pattern).IsMatch(subject).ShouldBeTrue();
        new PcreRegexUtf8(pattern).IsMatch(subject.AsSpan()).ShouldBeTrue();
        new PcreRegexUtf8(pattern).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeTrue();
        new PcreRegexUtf8(pattern, PcreOptions.Compiled).IsMatch(subject).ShouldBeTrue();
        new PcreRegexUtf8(pattern, PcreOptions.Compiled).IsMatch(subject.AsSpan()).ShouldBeTrue();
        new PcreRegexUtf8(pattern, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeTrue();
    }

    [Test]
    [TestCase(@"^A.*Z$", "AfooZ")]
    [TestCase(@"^A(.*)Z$", "AfooZ")]
    [TestCase(@"^\p{L}+$", "Abçdë")]
    public void should_match_pattern_8bit(string patternString, string subjectString)
    {
        var pattern = patternString.ToLatin1Bytes();
        var subject = subjectString.ToLatin1Bytes();

        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding).IsMatch(subject).ShouldBeTrue();
        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding).IsMatch(subject.AsSpan()).ShouldBeTrue();
        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeTrue();
        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding, PcreOptions.Compiled).IsMatch(subject).ShouldBeTrue();
        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding, PcreOptions.Compiled).IsMatch(subject.AsSpan()).ShouldBeTrue();
        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeTrue();
    }

    [Test]
    [TestCase(@"^A.*Z$", "Afoo")]
    [TestCase(@"^\p{L}+$", "Abc123abc")]
    public void should_not_match_pattern(string pattern, string subject)
    {
        new PcreRegex(pattern).IsMatch(subject).ShouldBeFalse();
        new PcreRegex(pattern).IsMatch(subject.AsSpan()).ShouldBeFalse();
        new PcreRegex(pattern).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeFalse();
        new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject).ShouldBeFalse();
        new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject.AsSpan()).ShouldBeFalse();
        new PcreRegex(pattern, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeFalse();
    }

    [Test]
    [TestCase(@"^A.*Z$", "Afoo")]
    [TestCase(@"^\p{L}+$", "Abc123abc")]
    public void should_not_match_pattern_utf8(string pattern, string subjectString)
    {
        var subject = Encoding.UTF8.GetBytes(subjectString);

        new PcreRegexUtf8(pattern).IsMatch(subject).ShouldBeFalse();
        new PcreRegexUtf8(pattern).IsMatch(subject.AsSpan()).ShouldBeFalse();
        new PcreRegexUtf8(pattern).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeFalse();
        new PcreRegexUtf8(pattern, PcreOptions.Compiled).IsMatch(subject).ShouldBeFalse();
        new PcreRegexUtf8(pattern, PcreOptions.Compiled).IsMatch(subject.AsSpan()).ShouldBeFalse();
        new PcreRegexUtf8(pattern, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeFalse();
    }

    [Test]
    [TestCase(@"^A.*Z$", "Afoo")]
    [TestCase(@"^\p{L}+$", "Abc123abc")]
    public void should_not_match_pattern_8bit(string patternString, string subjectString)
    {
        var pattern = patternString.ToLatin1Bytes();
        var subject = subjectString.ToLatin1Bytes();

        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding).IsMatch(subject).ShouldBeFalse();
        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding).IsMatch(subject.AsSpan()).ShouldBeFalse();
        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeFalse();
        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding, PcreOptions.Compiled).IsMatch(subject).ShouldBeFalse();
        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding, PcreOptions.Compiled).IsMatch(subject.AsSpan()).ShouldBeFalse();
        new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()).ShouldBeFalse();
    }

    [Test]
    public void should_handle_ignore_case()
    {
        var re = new PcreRegex("aBc");
        re.IsMatch("Abc").ShouldBeFalse();
        re.IsMatch("Abc".AsSpan()).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("Abc".AsSpan()).ShouldBeFalse();

        re = new PcreRegex("aBc", PcreOptions.IgnoreCase);
        re.IsMatch("Abc").ShouldBeTrue();
        re.IsMatch("Abc".AsSpan()).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("Abc".AsSpan()).ShouldBeTrue();
    }

    [Test]
    public void should_handle_ignore_case_utf8()
    {
        var re = new PcreRegexUtf8("aBc"u8);
        re.IsMatch("Abc"u8).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("Abc"u8).ShouldBeFalse();

        re = new PcreRegexUtf8("aBc"u8, PcreOptions.IgnoreCase);
        re.IsMatch("Abc"u8).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("Abc"u8).ShouldBeTrue();
    }

    [Test]
    public void should_handle_ignore_case_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("aBc");
        re.IsMatch("Abc".ToLatin1Bytes()).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("Abc".ToLatin1Bytes()).ShouldBeFalse();

        re = TestSupport.CreatePcreRegex8Bit("aBc", PcreOptions.IgnoreCase);
        re.IsMatch("Abc".ToLatin1Bytes()).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("Abc".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    public void should_handle_ignore_whitespace()
    {
        var re = new PcreRegex("^a b$");
        re.IsMatch("ab").ShouldBeFalse();
        re.IsMatch("ab".AsSpan()).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("ab".AsSpan()).ShouldBeFalse();

        re = new PcreRegex("^a b$", PcreOptions.IgnorePatternWhitespace);
        re.IsMatch("ab").ShouldBeTrue();
        re.IsMatch("ab".AsSpan()).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("ab".AsSpan()).ShouldBeTrue();
    }

    [Test]
    public void should_handle_ignore_whitespace_utf8()
    {
        var re = new PcreRegexUtf8("^a b$"u8);
        re.IsMatch("ab"u8).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("ab"u8).ShouldBeFalse();

        re = new PcreRegexUtf8("^a b$"u8, PcreOptions.IgnorePatternWhitespace);
        re.IsMatch("ab"u8).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("ab"u8).ShouldBeTrue();
    }

    [Test]
    public void should_handle_ignore_whitespace_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("^a b$");
        re.IsMatch("ab".ToLatin1Bytes()).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("ab".ToLatin1Bytes()).ShouldBeFalse();

        re = TestSupport.CreatePcreRegex8Bit("^a b$", PcreOptions.IgnorePatternWhitespace);
        re.IsMatch("ab".ToLatin1Bytes()).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("ab".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    public void should_handle_singleline()
    {
        var re = new PcreRegex("^a.*b$");
        re.IsMatch("a\r\nb").ShouldBeFalse();
        re.IsMatch("a\r\nb".AsSpan()).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("a\r\nb".AsSpan()).ShouldBeFalse();

        re = new PcreRegex("^a.*b$", PcreOptions.Singleline);
        re.IsMatch("a\r\nb").ShouldBeTrue();
        re.IsMatch("a\r\nb".AsSpan()).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("a\r\nb".AsSpan()).ShouldBeTrue();
    }

    [Test]
    public void should_handle_singleline_utf8()
    {
        var re = new PcreRegexUtf8("^a.*b$"u8);
        re.IsMatch("a\r\nb"u8).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("a\r\nb"u8).ShouldBeFalse();

        re = new PcreRegexUtf8("^a.*b$"u8, PcreOptions.Singleline);
        re.IsMatch("a\r\nb"u8).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("a\r\nb"u8).ShouldBeTrue();
    }

    [Test]
    public void should_handle_singleline_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("^a.*b$");
        re.IsMatch("a\r\nb".ToLatin1Bytes()).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("a\r\nb".ToLatin1Bytes()).ShouldBeFalse();

        re = TestSupport.CreatePcreRegex8Bit("^a.*b$", PcreOptions.Singleline);
        re.IsMatch("a\r\nb".ToLatin1Bytes()).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("a\r\nb".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    public void should_handle_multiline()
    {
        var re = new PcreRegex("^aaa$");
        re.IsMatch("aaa\r\nbbb").ShouldBeFalse();
        re.IsMatch("aaa\r\nbbb".AsSpan()).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("aaa\r\nbbb".AsSpan()).ShouldBeFalse();

        re = new PcreRegex("^aaa$", PcreOptions.MultiLine);
        re.IsMatch("aaa\r\nbbb").ShouldBeTrue();
        re.IsMatch("aaa\r\nbbb".AsSpan()).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("aaa\r\nbbb".AsSpan()).ShouldBeTrue();
    }

    [Test]
    public void should_handle_multiline_utf8()
    {
        var re = new PcreRegexUtf8("^aaa$"u8);
        re.IsMatch("aaa\r\nbbb"u8).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("aaa\r\nbbb"u8).ShouldBeFalse();

        re = new PcreRegexUtf8("^aaa$"u8, PcreOptions.MultiLine);
        re.IsMatch("aaa\r\nbbb"u8).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("aaa\r\nbbb"u8).ShouldBeTrue();
    }

    [Test]
    public void should_handle_multiline_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("^aaa$");
        re.IsMatch("aaa\r\nbbb".ToLatin1Bytes()).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("aaa\r\nbbb".ToLatin1Bytes()).ShouldBeFalse();

        re = TestSupport.CreatePcreRegex8Bit("^aaa$", PcreOptions.MultiLine);
        re.IsMatch("aaa\r\nbbb".ToLatin1Bytes()).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("aaa\r\nbbb".ToLatin1Bytes()).ShouldBeTrue();
    }

    [Test]
    public void should_handle_javascript()
    {
        var re = new PcreRegex(@"^\U$", PcreOptions.JavaScript);
        re.IsMatch("U").ShouldBeTrue();
        re.IsMatch("U".AsSpan()).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("U".AsSpan()).ShouldBeTrue();

        var ex = Should.Throw<PcrePatternException>(() => new PcreRegex(@"^\U$"));
        ex.ErrorCode.ShouldBe(PcreErrorCode.UnsupportedEscapeSequence);
    }

    [Test]
    public void should_handle_javascript_utf8()
    {
        var re = new PcreRegexUtf8(@"^\U$"u8, PcreOptions.JavaScript);
        re.IsMatch("U"u8).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("U"u8).ShouldBeTrue();

        var ex = Should.Throw<PcrePatternException>(() => new PcreRegexUtf8(@"^\U$"u8));
        ex.ErrorCode.ShouldBe(PcreErrorCode.UnsupportedEscapeSequence);
    }

    [Test]
    public void should_handle_javascript_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"^\U$", PcreOptions.JavaScript);
        re.IsMatch("U".ToLatin1Bytes()).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("U".ToLatin1Bytes()).ShouldBeTrue();

        var ex = Should.Throw<PcrePatternException>(() => TestSupport.CreatePcreRegex8Bit(@"^\U$"));
        ex.ErrorCode.ShouldBe(PcreErrorCode.UnsupportedEscapeSequence);
    }

    [Test]
    public void should_handle_unicode_character_properties()
    {
        var re = new PcreRegex(@"^\w$");
        re.IsMatch("à").ShouldBeFalse();
        re.IsMatch("à".AsSpan()).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("à".AsSpan()).ShouldBeFalse();

        re = new PcreRegex(@"^\w$", PcreOptions.Unicode);
        re.IsMatch("à").ShouldBeTrue();
        re.IsMatch("à".AsSpan()).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("à".AsSpan()).ShouldBeTrue();
    }

    [Test]
    public void should_handle_unicode_character_properties_utf8()
    {
        var re = new PcreRegexUtf8(@"^\w$"u8);
        re.IsMatch("à"u8).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("à"u8).ShouldBeFalse();

        re = new PcreRegexUtf8(@"^\w$"u8, PcreOptions.Unicode);
        re.IsMatch("à"u8).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("à"u8).ShouldBeTrue();
    }

    [Test]
    public void should_handle_unicode_character_properties_8bit()
    {
        var subject = "à".ToLatin1Bytes();

        var re = TestSupport.CreatePcreRegex8Bit(@"^\w$");
        re.IsMatch(subject).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch(subject).ShouldBeFalse();

        re = TestSupport.CreatePcreRegex8Bit(@"^\w$", PcreOptions.Unicode);
        re.IsMatch(subject).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch(subject).ShouldBeTrue();
    }

    [Test]
    public void should_match_from_index()
    {
        var re = new PcreRegex(@"a");
        re.IsMatch("foobar", 5).ShouldBeFalse();
        re.IsMatch("foobar".AsSpan(), 5).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("foobar".AsSpan(), 5).ShouldBeFalse();
    }

    [Test]
    public void should_match_from_index_utf8()
    {
        var re = new PcreRegexUtf8(@"a"u8);
        re.IsMatch("foobar"u8, 5).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("foobar"u8, 5).ShouldBeFalse();
    }

    [Test]
    public void should_match_from_index_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a");
        re.IsMatch("foobar".ToLatin1Bytes(), 5).ShouldBeFalse();
        re.CreateMatchBuffer().IsMatch("foobar".ToLatin1Bytes(), 5).ShouldBeFalse();
    }

    [Test]
    public void should_match_starting_at_end_of_string()
    {
        var re = new PcreRegex(@"(?<=a)");
        re.IsMatch("xxa", 3).ShouldBeTrue();
        re.IsMatch("xxa".AsSpan(), 3).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("xxa".AsSpan(), 3).ShouldBeTrue();
    }

    [Test]
    public void should_match_starting_at_end_of_string_utf8()
    {
        var re = new PcreRegexUtf8(@"(?<=a)"u8);
        re.IsMatch("xxa"u8, 3).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("xxa"u8, 3).ShouldBeTrue();
    }

    [Test]
    public void should_match_starting_at_end_of_string_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"(?<=a)");
        re.IsMatch("xxa".ToLatin1Bytes(), 3).ShouldBeTrue();
        re.CreateMatchBuffer().IsMatch("xxa".ToLatin1Bytes(), 3).ShouldBeTrue();
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index(int startIndex)
    {
        var re = new PcreRegex(@"a");
        Should.Throw<ArgumentOutOfRangeException>(() => re.IsMatch("a", startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_ref(int startIndex)
    {
        var re = new PcreRegex(@"a");
        Should.Throw<ArgumentOutOfRangeException>(() => re.IsMatch("a".AsSpan(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf(int startIndex)
    {
        var re = new PcreRegex(@"a");
        var buffer = re.CreateMatchBuffer();
        Should.Throw<ArgumentOutOfRangeException>(() => buffer.IsMatch("a".AsSpan(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_utf8(int startIndex)
    {
        var re = new PcreRegexUtf8(@"a"u8);
        Should.Throw<ArgumentOutOfRangeException>(() => re.IsMatch("a"u8, startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf_utf8(int startIndex)
    {
        var re = new PcreRegexUtf8(@"a"u8);
        var buffer = re.CreateMatchBuffer();
        Should.Throw<ArgumentOutOfRangeException>(() => buffer.IsMatch("a"u8, startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_8bit(int startIndex)
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a");
        Should.Throw<ArgumentOutOfRangeException>(() => re.IsMatch("a".ToLatin1Bytes(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf_8bit(int startIndex)
    {
        var re = TestSupport.CreatePcreRegex8Bit(@"a");
        var buffer = re.CreateMatchBuffer();
        Should.Throw<ArgumentOutOfRangeException>(() => buffer.IsMatch("a".ToLatin1Bytes(), startIndex));
    }
}
