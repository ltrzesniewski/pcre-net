using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using PCRE.Tests.Support;
using Shouldly;

namespace PCRE.Tests.PcreNet;

[TestFixture]
[SuppressMessage("ReSharper", "ArrangeDefaultValueWhenTypeNotEvident")]
public class PcreRegexTests
{
    [Test]
    public void should_throw_on_null_pattern()
    {
        Should.Throw<ArgumentNullException>(() => new PcreRegex(default(string)!));
        Should.Throw<ArgumentNullException>(() => new PcreRegex(default(string)!, PcreOptions.None));
        Should.Throw<ArgumentNullException>(() => new PcreRegex(default(string)!, new PcreRegexSettings()));
    }

    [Test]
    public void should_throw_on_null_pattern_utf8()
    {
        Should.Throw<ArgumentNullException>(() => new PcreRegexUtf8(default(string)!));
        Should.Throw<ArgumentNullException>(() => new PcreRegexUtf8(default(string)!, PcreOptions.None));
        Should.Throw<ArgumentNullException>(() => new PcreRegexUtf8(default(string)!, new PcreRegexSettings()));
    }

    [Test]
    public void should_throw_on_null_settings()
    {
        Should.Throw<ArgumentNullException>(() => new PcreRegex("a", default(PcreRegexSettings)!));
    }

    [Test]
    public void should_throw_on_null_settings_utf8()
    {
        Should.Throw<ArgumentNullException>(() => new PcreRegexUtf8("a"u8, default(PcreRegexSettings)!));
        Should.Throw<ArgumentNullException>(() => new PcreRegexUtf8("a", default(PcreRegexSettings)!));
    }

    [Test]
    public void should_throw_on_null_settings_8bit()
    {
        Should.Throw<ArgumentNullException>(() => TestSupport.CreatePcreRegex8Bit("a".ToLatin1Bytes(), default(PcreRegexSettings)!));
    }

    [Test]
    public void should_return_pattern_string()
    {
        var re = new PcreRegex("foo|bar");
        re.ToString().ShouldBe("foo|bar");
    }

    [Test]
    public void should_return_pattern_string_utf8()
    {
        var re = new PcreRegexUtf8("foo|bar"u8);
        re.ToString().ShouldBe("foo|bar");
    }

    [Test]
    public void should_return_pattern_string_8bit()
    {
        var re = TestSupport.CreatePcreRegex8Bit("foo|bar".ToLatin1Bytes());
        re.ToString().ShouldBe("foo|bar");
    }

    [Test]
    [TestCase(10u, ExpectedResult = true)]
    [TestCase(3u, ExpectedResult = true)]
    [TestCase(2u, ExpectedResult = false)]
    public bool should_limit_max_pattern_length(uint maxLength)
    {
        return TryCompilePattern("foo", new PcreRegexSettings { MaxPatternLength = maxLength }) is not null;
    }

    [Test]
    [TestCase(10u, ExpectedResult = true)]
    [TestCase(3u, ExpectedResult = true)]
    [TestCase(2u, ExpectedResult = false)]
    public bool should_limit_max_pattern_length_utf8(uint maxLength)
    {
        return TryCompilePatternUtf8("foo"u8, new PcreRegexSettings { MaxPatternLength = maxLength }) is not null;
    }

    [Test]
    [TestCase(10u, ExpectedResult = true)]
    [TestCase(3u, ExpectedResult = true)]
    [TestCase(2u, ExpectedResult = false)]
    public bool should_limit_max_pattern_length_8bit(uint maxLength)
    {
        return TryCompilePattern8Bit("foo".ToLatin1Bytes(), new PcreRegexSettings { MaxPatternLength = maxLength }) is not null;
    }

    [Test]
    [TestCase(1000u, ExpectedResult = true)]
    [TestCase(2u, ExpectedResult = false)]
    public bool should_limit_max_compiled_pattern_length(uint maxLength)
    {
        var re = TryCompilePattern("foo", new PcreRegexSettings { MaxPatternCompiledLength = maxLength });
        if (re is null)
            return false;

        re.PatternInfo.PatternSize.ShouldBeLessThanOrEqualTo(maxLength);
        return true;
    }

    [Test]
    [TestCase(1000u, ExpectedResult = true)]
    [TestCase(2u, ExpectedResult = false)]
    public bool should_limit_max_compiled_pattern_length_utf8(uint maxLength)
    {
        var re = TryCompilePatternUtf8("foo"u8, new PcreRegexSettings { MaxPatternCompiledLength = maxLength });
        if (re is null)
            return false;

        re.PatternInfo.PatternSize.ShouldBeLessThanOrEqualTo(maxLength);
        return true;
    }

    [Test]
    [TestCase(1000u, ExpectedResult = true)]
    [TestCase(2u, ExpectedResult = false)]
    public bool should_limit_max_compiled_pattern_length_8bit(uint maxLength)
    {
        var re = TryCompilePattern8Bit("foo".ToLatin1Bytes(), new PcreRegexSettings { MaxPatternCompiledLength = maxLength });
        if (re is null)
            return false;

        re.PatternInfo.PatternSize.ShouldBeLessThanOrEqualTo(maxLength);
        return true;
    }

    [Test]
    [TestCase("(?:(?C1)a){2}", PcreOptions.None)]
    [TestCase("(a){2}", PcreOptions.AutoCallout)]
    [TestCase("a(?C1)", PcreOptions.AutoCallout)]
    public void should_ignore_duplicate_callout_offsets(string pattern, PcreOptions options)
        => _ = new PcreRegex(pattern, options).PatternInfo.Callouts;

    [Test]
    public void should_allow_null_in_callout_names()
    {
        var re = new PcreRegex("(?C'foo\0bar🙂baz')");
        var name = re.PatternInfo.Callouts[0].String;

        var matched = "";

        re.Match("a", callout =>
        {
            matched = callout.String;
            return PcreCalloutResult.Pass;
        });

        name.ShouldBe("foo\0bar🙂baz");
        matched.ShouldBe(name);
    }

    [Test]
    public void should_allow_null_in_callout_names_8bit()
    {
        var re = new PcreRegex8Bit("(?C'foo\0bâr')".ToLatin1Bytes(), TestSupport.Latin1Encoding);
        var name = re.PatternInfo.Callouts[0].String;

        var matched = "";

        re.Match("a".ToLatin1Bytes(), callout =>
        {
            matched = callout.String;
            return PcreCalloutResult.Pass;
        });

        name.ShouldBe("foo\0bâr");
        matched.ShouldBe(name);
    }

    [Test]
    public void should_allow_null_in_callout_names_utf8()
    {
        var re = new PcreRegexUtf8("(?C'foo\0bar🙂baz')"u8);
        var name = re.PatternInfo.Callouts[0].String;

        var matched = "";

        re.Match("a"u8, callout =>
        {
            matched = callout.String;
            return PcreCalloutResult.Pass;
        });

        name.ShouldBe("foo\0bar🙂baz");
        matched.ShouldBe(name);
    }

    private static PcreRegex? TryCompilePattern(string pattern, PcreRegexSettings settings)
    {
        try
        {
            return new PcreRegex(pattern, settings);
        }
        catch (PcrePatternException)
        {
            return null;
        }
    }

    private static PcreRegexUtf8? TryCompilePatternUtf8(ReadOnlySpan<byte> pattern, PcreRegexSettings settings)
    {
        try
        {
            return new PcreRegexUtf8(pattern, settings);
        }
        catch (PcrePatternException)
        {
            return null;
        }
    }

    private static PcreRegex8Bit? TryCompilePattern8Bit(ReadOnlySpan<byte> pattern, PcreRegexSettings settings)
    {
        try
        {
            return new PcreRegex8Bit(pattern, TestSupport.Latin1Encoding, settings);
        }
        catch (PcrePatternException)
        {
            return null;
        }
    }
}
