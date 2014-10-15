using System;
using System.Linq;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    public class MatchesTests
    {
        [Test]
        public void should_return_all_matches()
        {
            var re = new PcreRegex(@"a(b)a");
            var matches = re.Matches("foo aba bar aba baz").ToList();

            Assert.That(matches.Count, Is.EqualTo(2));

            Assert.That(matches[0].Value, Is.EqualTo("aba"));
            Assert.That(matches[0].Index, Is.EqualTo(4));
            Assert.That(matches[0].Length, Is.EqualTo(3));

            Assert.That(matches[0][1].Value, Is.EqualTo("b"));
            Assert.That(matches[0][1].Index, Is.EqualTo(5));
            Assert.That(matches[0][1].Length, Is.EqualTo(1));

            Assert.That(matches[1].Value, Is.EqualTo("aba"));
            Assert.That(matches[1].Index, Is.EqualTo(12));
            Assert.That(matches[1].Length, Is.EqualTo(3));

            Assert.That(matches[1][1].Value, Is.EqualTo("b"));
            Assert.That(matches[1][1].Index, Is.EqualTo(13));
            Assert.That(matches[1][1].Length, Is.EqualTo(1));
        }

        [Test]
        public void should_handle_empty_matches()
        {
            var re = new PcreRegex(@"(?=(a))");
            var matches = re.Matches("aaabbaa").ToList();

            Assert.That(matches.Count, Is.EqualTo(5));

            Assert.That(matches.Select(m => m.Index), Is.EqualTo(new[] { 0, 1, 2, 5, 6 }));
            Assert.That(matches.Select(m => m.Length), Is.All.EqualTo(0));
            Assert.That(matches.Select(m => m.Value), Is.All.EqualTo(String.Empty));

            Assert.That(matches.Select(m => m[1].Index), Is.EqualTo(new[] { 0, 1, 2, 5, 6 }));
            Assert.That(matches.Select(m => m[1].Length), Is.All.EqualTo(1));
            Assert.That(matches.Select(m => m[1].Value), Is.All.EqualTo("a"));
        }
    }
}
