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
}
