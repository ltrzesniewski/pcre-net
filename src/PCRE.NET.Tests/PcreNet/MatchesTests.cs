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

            Assert.That(matches, Has.Count.EqualTo(2));

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

            Assert.That(matches, Has.Count.EqualTo(5));

            Assert.That(matches.Select(m => m.Index), Is.EqualTo(new[] { 0, 1, 2, 5, 6 }));
            Assert.That(matches.Select(m => m.Length), Is.All.EqualTo(0));
            Assert.That(matches.Select(m => m.Value), Is.All.EqualTo(String.Empty));

            Assert.That(matches.Select(m => m[1].Index), Is.EqualTo(new[] { 0, 1, 2, 5, 6 }));
            Assert.That(matches.Select(m => m[1].Length), Is.All.EqualTo(1));
            Assert.That(matches.Select(m => m[1].Value), Is.All.EqualTo("a"));
        }

        [Test]
        public void should_match_from_index()
        {
            var re = new PcreRegex(@"a");
            var matches = re.Matches("foo bar baz", 6).ToList();

            Assert.That(matches, Has.Count.EqualTo(1));
        }

        [Test]
        public void should_match_starting_at_end_of_string()
        {
            var re = new PcreRegex(@"(?<=a)");
            var matches = re.Matches("xxa", 3).ToList();

            Assert.That(matches, Has.Count.EqualTo(1));
        }

        [Test]
        public void should_handle_end_before_start()
        {
            var re = new PcreRegex(@"(?=a+b\K)");
            var matches = re.Matches("aaabab").ToList();

            Assert.That(matches, Has.Count.EqualTo(2));

            Assert.That(matches[0], Is.Not.Null);
            Assert.That(matches[0].Index, Is.EqualTo(4));
            Assert.That(matches[0].EndIndex, Is.EqualTo(0));
            Assert.That(matches[0].Length, Is.EqualTo(0));
            Assert.That(matches[0].Value, Is.EqualTo(string.Empty));

            Assert.That(matches[1], Is.Not.Null);
            Assert.That(matches[1].Index, Is.EqualTo(6));
            Assert.That(matches[1].EndIndex, Is.EqualTo(4));
            Assert.That(matches[1].Length, Is.EqualTo(0));
            Assert.That(matches[1].Value, Is.EqualTo(string.Empty));
        }

        [Test]
        public void should_report_callout_exception()
        {
            var re = new PcreRegex(@"a(?C1)");

            var resultCount = 0;

            var seq = re.Matches("aaa", 0, callout =>
            {
                if (callout.StartOffset >= 2)
                    throw new InvalidOperationException("Simulated exception");

                return PcreCalloutResult.Pass;
            }).Select(i =>
            {
                ++resultCount;
                return i;
            });

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<PcreCalloutException>(() => seq.ToList());
            Assert.That(resultCount, Is.EqualTo(2));
        }

        [Test]
        public void readme_backtracking_verbs_example()
        {
            var matches = PcreRegex.Matches("(foo) bar (baz) 42", @"\(\w+\)(*SKIP)(*FAIL)|\w+")
                .Select(m => m.Value)
                .ToList();

            Assert.That(matches, Is.EqualTo(new[] { "bar", "42" }));
        }
    }
}
