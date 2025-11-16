using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet;

[TestFixture]
[SuppressMessage("ReSharper", "ArrangeDefaultValueWhenTypeNotEvident")]
public class PcreRegexTests
{
    [Test]
    public void should_throw_on_null_pattern()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new PcreRegex(default(string)!));
        Assert.Throws<ArgumentNullException>(() => _ = new PcreRegex(default(string)!, PcreOptions.None));
        Assert.Throws<ArgumentNullException>(() => _ = new PcreRegex(default(string)!, new PcreRegexSettings()));
    }

    [Test]
    public void should_throw_on_null_pattern_utf8()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new PcreRegexUtf8(default(string)!));
        Assert.Throws<ArgumentNullException>(() => _ = new PcreRegexUtf8(default(string)!, PcreOptions.None));
        Assert.Throws<ArgumentNullException>(() => _ = new PcreRegexUtf8(default(string)!, new PcreRegexSettings()));
    }

    [Test]
    public void should_throw_on_null_settings()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new PcreRegex("a", default(PcreRegexSettings)!));
    }

    [Test]
    public void should_throw_on_null_settings_utf8()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new PcreRegexUtf8("a"u8, default(PcreRegexSettings)!));
        Assert.Throws<ArgumentNullException>(() => _ = new PcreRegexUtf8("a", default(PcreRegexSettings)!));
    }

    [Test]
    public void should_return_pattern_string()
    {
        var re = new PcreRegex("foo|bar");
        Assert.That(re.ToString(), Is.EqualTo("foo|bar"));
    }

    [Test]
    public void should_return_pattern_string_utf8()
    {
        var re = new PcreRegexUtf8("foo|bar"u8);
        Assert.That(re.ToString(), Is.EqualTo("foo|bar"));
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
    [TestCase(1000u, ExpectedResult = true)]
    [TestCase(2u, ExpectedResult = false)]
    public bool should_limit_max_compiled_pattern_length(uint maxLength)
    {
        var re = TryCompilePattern("foo", new PcreRegexSettings { MaxPatternCompiledLength = maxLength });
        if (re is null)
            return false;

        Assert.That(re.PatternInfo.PatternSize, Is.LessThanOrEqualTo(maxLength));
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

        Assert.That(re.PatternInfo.PatternSize, Is.LessThanOrEqualTo(maxLength));
        return true;
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
}
