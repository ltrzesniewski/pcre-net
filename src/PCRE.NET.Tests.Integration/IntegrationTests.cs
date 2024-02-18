using NUnit.Framework;

namespace PCRE.Tests.Integration;

[TestFixture(Category = "Integration")]
public class IntegrationTests
{
    [Test]
    [TestCase(PcreOptions.None)]
    [TestCase(PcreOptions.Compiled)]
    public void should_match_expected_values(PcreOptions options)
    {
        var re = new PcreRegex("a+(b+)c+", options);
        var match = re.Match("xxxaaabbccczzz");

        Assert.That(match, Is.Not.Null);
        Assert.That(match.Success, Is.True);
        Assert.That(match.CaptureCount, Is.EqualTo(1));
        Assert.That(match.Value, Is.EqualTo("aaabbccc"));
        Assert.That(match.ValueSpan.ToString(), Is.EqualTo("aaabbccc"));
        Assert.That(match.Index, Is.EqualTo(3));
        Assert.That(match.EndIndex, Is.EqualTo(11));
        Assert.That(match.Length, Is.EqualTo(8));

        Assert.That(match[1], Is.Not.Null);
        Assert.That(match[1].Success, Is.True);
        Assert.That(match[1].Value, Is.EqualTo("bb"));
        Assert.That(match[1].ValueSpan.ToString(), Is.EqualTo("bb"));
        Assert.That(match[1].Index, Is.EqualTo(6));
        Assert.That(match[1].Length, Is.EqualTo(2));

        Assert.That(match.Groups[1], Is.SameAs(match[1]));

#if !PCRENET_INTEGRATION_TEST
        Assert.Fail("Not in integration tests mode");
#endif
    }
}
