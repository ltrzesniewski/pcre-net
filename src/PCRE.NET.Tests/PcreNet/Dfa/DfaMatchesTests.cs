using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace PCRE.Tests.PcreNet.Dfa;

[TestFixture]
public class DfaMatchesTests
{
    [Test]
    public void should_return_all_matched_sets()
    {
        var re = new PcreRegex(@"<.*>");
        var matches = re.Dfa.Matches("This is <something> <something else> <something further> no more").ToList();

        matches.ShouldHaveCount(3);

        matches[0].Index.ShouldBe(8);
        matches[1].Index.ShouldBe(20);
        matches[2].Index.ShouldBe(37);

        matches[0].ShouldHaveCount(3);
        matches[1].ShouldHaveCount(2);
        matches[2].ShouldHaveSingleItem();

        matches[0].LongestMatch.Value.ShouldBe("<something> <something else> <something further>");
        matches[1].LongestMatch.Value.ShouldBe("<something else> <something further>");
        matches[2].LongestMatch.Value.ShouldBe("<something further>");

        matches[0].LongestMatch.ValueSpan.ShouldBe("<something> <something else> <something further>");
        matches[1].LongestMatch.ValueSpan.ShouldBe("<something else> <something further>");
        matches[2].LongestMatch.ValueSpan.ShouldBe("<something further>");

        matches[0].ShortestMatch.Value.ShouldBe("<something>");
        matches[1].ShortestMatch.Value.ShouldBe("<something else>");
        matches[2].ShortestMatch.Value.ShouldBe("<something further>");

        matches[0].ShortestMatch.ValueSpan.ShouldBe("<something>");
        matches[1].ShortestMatch.ValueSpan.ShouldBe("<something else>");
        matches[2].ShortestMatch.ValueSpan.ShouldBe("<something further>");
    }

    [Test]
    public void should_not_start_a_match_inside_a_surrogate_pair()
    {
        var re = new PcreRegex(@".");
        var matches = re.Dfa.Matches("foo\uD83D\uDE0Ebar").ToList();

        matches.ShouldHaveCount(7);

        matches[3].ShortestMatch.Index.ShouldBe(3);
        matches[3].ShortestMatch.Length.ShouldBe(2);

        matches[4].ShortestMatch.Index.ShouldBe(5);
        matches[4].ShortestMatch.Length.ShouldBe(1);
        matches[4].ShortestMatch.Value.ShouldBe("b");
        matches[4].ShortestMatch.ValueSpan.ShouldBe("b");
    }

    [Test]
    public void should_match_empty_pattern()
    {
        var re = new PcreRegex(@"");
        var matches = re.Dfa.Matches("foo").ToList();

        matches.ShouldHaveCount(4);
        matches.ShouldAllBe(i => i.LongestMatch.Length == 0);
    }
}
