using System.Linq;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet.Dfa
{
    [TestFixture]
    public class DfaMatchesTests
    {
        [Test]
        public void should_return_all_matched_sets()
        {
            var re = new PcreRegex(@"<.*>");
            var matches = re.Dfa.Matches("This is <something> <something else> <something further> no more").ToList();

            Assert.That(matches.Count, Is.EqualTo(3));

            Assert.That(matches[0].Index, Is.EqualTo(8));
            Assert.That(matches[1].Index, Is.EqualTo(20));
            Assert.That(matches[2].Index, Is.EqualTo(37));

            Assert.That(matches[0].Count, Is.EqualTo(3));
            Assert.That(matches[1].Count, Is.EqualTo(2));
            Assert.That(matches[2].Count, Is.EqualTo(1));

            Assert.That(matches[0].LongestMatch.Value, Is.EqualTo("<something> <something else> <something further>"));
            Assert.That(matches[1].LongestMatch.Value, Is.EqualTo("<something else> <something further>"));
            Assert.That(matches[2].LongestMatch.Value, Is.EqualTo("<something further>"));

            Assert.That(matches[0].ShortestMatch.Value, Is.EqualTo("<something>"));
            Assert.That(matches[1].ShortestMatch.Value, Is.EqualTo("<something else>"));
            Assert.That(matches[2].ShortestMatch.Value, Is.EqualTo("<something further>"));
        }

        [Test]
        public void should_not_start_a_match_inside_a_surrogate_pair()
        {
            var re = new PcreRegex(@".");
            var matches = re.Dfa.Matches("foo\uD83D\uDE0Ebar").ToList();

            Assert.That(matches.Count, Is.EqualTo(7));

            Assert.That(matches[3].ShortestMatch.Index, Is.EqualTo(3));
            Assert.That(matches[3].ShortestMatch.Length, Is.EqualTo(2));

            Assert.That(matches[4].ShortestMatch.Index, Is.EqualTo(5));
            Assert.That(matches[4].ShortestMatch.Length, Is.EqualTo(1));
            Assert.That(matches[4].ShortestMatch.Value, Is.EqualTo("b"));
        }

        [Test]
        public void should_match_empty_pattern()
        {
            var re = new PcreRegex(@"");
            var matches = re.Dfa.Matches("foo").ToList();

            Assert.That(matches.Count, Is.EqualTo(4));
            Assert.That(matches.Select(i => i.LongestMatch.Length), Is.All.Zero);
        }
    }
}
