using NUnit.Framework;
using PCRE.Dfa;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    public class DfaMatchTests
    {
        [Test]
        public void should_match_with_dfa()
        {
            var re = new PcreRegex(@"<.*>");
            var match = re.DfaMatch("This is <something> <something else> <something further> no more");

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);

            Assert.That(match.Count, Is.EqualTo(3));
            Assert.That(match.Index, Is.EqualTo(8));

            Assert.That(match.LongestMatch, Is.Not.Null);
            Assert.That(match.LongestMatch.Value, Is.EqualTo("<something> <something else> <something further>"));

            Assert.That(match.LongestMatch, Is.SameAs(match[0]));
            Assert.That(match.ShortestMatch, Is.SameAs(match[2]));

            Assert.That(match.ShortestMatch, Is.Not.Null);
            Assert.That(match.ShortestMatch.Value, Is.EqualTo("<something>"));

            Assert.That(match[1], Is.Not.Null);
            Assert.That(match[1].Value, Is.EqualTo("<something> <something else>"));

            Assert.That(match[3], Is.Null);
        }

        [Test]
        public void should_get_shortest_match()
        {
            var re = new PcreRegex(@"<.*>");
            var match = re.DfaMatch("This is <something> <something else> <something further> no more", PcreDfaMatchOptions.ShortestMatch);

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);

            Assert.That(match.Count, Is.EqualTo(1));
            Assert.That(match.Index, Is.EqualTo(8));

            Assert.That(match.ShortestMatch, Is.Not.Null);
            Assert.That(match.ShortestMatch.Value, Is.EqualTo("<something>"));
        }

        [Test]
        public void should_get_max_matches()
        {
            var re = new PcreRegex(@"<.*>");
            var match = re.DfaMatch("This is <something> <something else> <something further> no more", new PcreDfaMatchSettings
            {
                MaxResults = 2
            });

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);

            Assert.That(match.Count, Is.EqualTo(2));
            Assert.That(match.Index, Is.EqualTo(8));

            Assert.That(match.LongestMatch, Is.Not.Null);
            Assert.That(match.LongestMatch.Value, Is.EqualTo("<something> <something else> <something further>"));

            Assert.That(match.ShortestMatch, Is.Not.Null);
            Assert.That(match.ShortestMatch.Value, Is.EqualTo("<something> <something else>"));
        }

        [Test]
        public void should_start_at_given_index()
        {
            var re = new PcreRegex(@"<.*>");
            var match = re.DfaMatch("This is <something> <something else> <something further> no more", 10);

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);

            Assert.That(match.Count, Is.EqualTo(2));
            Assert.That(match.Index, Is.EqualTo(20));

            Assert.That(match.LongestMatch, Is.Not.Null);
            Assert.That(match.LongestMatch.Value, Is.EqualTo("<something else> <something further>"));

            Assert.That(match.ShortestMatch, Is.Not.Null);
            Assert.That(match.ShortestMatch.Value, Is.EqualTo("<something else>"));
        }
    }
}
