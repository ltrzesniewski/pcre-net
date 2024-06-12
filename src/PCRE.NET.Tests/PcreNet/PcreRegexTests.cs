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
    }

    [Test]
    public void should_throw_on_null_settings()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new PcreRegex("a", default(PcreRegexSettings)!));
    }

    [Test]
    public void should_return_pattern_string()
    {
        var re = new PcreRegex("foo|bar");
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
    [TestCase(1000u, ExpectedResult = true)]
    [TestCase(2u, ExpectedResult = false)]
    public bool should_limit_max_compiled_pattern_length(uint maxLength)
    {
        var re = TryCompilePattern("foo", new PcreRegexSettings { MaxPatternCompiledLength = maxLength });
        if (re is null)
            return false;

        Console.WriteLine(re.PatternInfo.PatternSize);
        Assert.That(re.PatternInfo.PatternSize, Is.LessThanOrEqualTo(maxLength));
        return true;
    }

    private static PcreRegex? TryCompilePattern(string pattern, PcreRegexSettings settings)
    {
        try
        {
            return new PcreRegex(pattern, settings);
        }
        catch (PcrePatternException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}
