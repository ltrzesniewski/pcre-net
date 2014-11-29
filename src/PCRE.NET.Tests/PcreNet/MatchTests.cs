using System;
using System.Linq;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    public class MatchTests
    {
        [Test]
        public void should_match_pattern()
        {
            var re = new PcreRegex(@"a+(b+)c+");
            var match = re.Match("xxxaaabbccczzz");

            Assert.That(match, Is.Not.Null);
            Assert.That(match.CaptureCount, Is.EqualTo(1));
            Assert.That(match.Value, Is.EqualTo("aaabbccc"));
            Assert.That(match.Index, Is.EqualTo(3));
            Assert.That(match.Length, Is.EqualTo(8));

            Assert.That(match[1], Is.Not.Null);
            Assert.That(match[1].Value, Is.EqualTo("bb"));
            Assert.That(match[1].Index, Is.EqualTo(6));
            Assert.That(match[1].Length, Is.EqualTo(2));
        }

        [Test]
        public void should_support_multiple_groups()
        {
            var re = new PcreRegex(@"a+(b+)(c+)?(d+)e+", PcreOptions.Studied);
            var match = re.Match("xxxaaabbddeeezzz");

            Assert.That(match, Is.Not.Null);
            Assert.That(match.CaptureCount, Is.EqualTo(3));
            Assert.That(match.Value, Is.EqualTo("aaabbddeee"));
            Assert.That(match.Index, Is.EqualTo(3));
            Assert.That(match.Length, Is.EqualTo(10));

            Assert.That(match[1], Is.Not.Null);
            Assert.That(match[1].IsMatch, Is.True);
            Assert.That(match[1].Value, Is.EqualTo("bb"));
            Assert.That(match[1].Index, Is.EqualTo(6));
            Assert.That(match[1].Length, Is.EqualTo(2));

            Assert.That(match[2], Is.Not.Null);
            Assert.That(match[2].IsMatch, Is.False);
            Assert.That(match[2].Value, Is.SameAs(String.Empty));
            Assert.That(match[2].Index, Is.EqualTo(-1));
            Assert.That(match[2].Length, Is.EqualTo(0));

            Assert.That(match[3], Is.Not.Null);
            Assert.That(match[3].IsMatch, Is.True);
            Assert.That(match[3].Value, Is.EqualTo("dd"));
            Assert.That(match[3].Index, Is.EqualTo(8));
            Assert.That(match[3].Length, Is.EqualTo(2));
        }

        [Test]
        public void should_match_starting_at_end_of_string()
        {
            var re = new PcreRegex(@"(?<=a)");
            var match = re.Match("xxa", 3);

            Assert.That(match, Is.Not.Null);
        }

        [Test]
        public void should_handle_named_groups()
        {
            var re = new PcreRegex(@"a+(?<bees>b+)(c+)(?<dees>d+)e+");

            var match = re.Match("xxxaaabbcccddeeezzz");

            Assert.That(match, Is.Not.Null);
            Assert.That(match.CaptureCount, Is.EqualTo(3));
            Assert.That(match.Value, Is.EqualTo("aaabbcccddeee"));
            Assert.That(match.Index, Is.EqualTo(3));
            Assert.That(match.Length, Is.EqualTo(13));

            Assert.That(match["bees"], Is.Not.Null);
            Assert.That(match["bees"].Value, Is.EqualTo("bb"));
            Assert.That(match["bees"].Index, Is.EqualTo(6));
            Assert.That(match["bees"].Length, Is.EqualTo(2));

            Assert.That(match[2], Is.Not.Null);
            Assert.That(match[2].Value, Is.EqualTo("ccc"));
            Assert.That(match[2].Index, Is.EqualTo(8));
            Assert.That(match[2].Length, Is.EqualTo(3));

            Assert.That(match["dees"], Is.Not.Null);
            Assert.That(match["dees"].Value, Is.EqualTo("dd"));
            Assert.That(match["dees"].Index, Is.EqualTo(11));
            Assert.That(match["dees"].Length, Is.EqualTo(2));
        }

        [Test]
        public void should_handle_case_sensitive_group_names()
        {
            var re = new PcreRegex(@"a+(?<grp>b+)(?<GRP>c+)(?<GrP>d+)e+");

            var match = re.Match("xxxaaabbcccddeeezzz");

            Assert.That(match["grp"], Is.Not.Null);
            Assert.That(match["grp"].Value, Is.EqualTo("bb"));
            Assert.That(match["grp"].Index, Is.EqualTo(6));
            Assert.That(match["grp"].Length, Is.EqualTo(2));

            Assert.That(match["GRP"], Is.Not.Null);
            Assert.That(match["GRP"].Value, Is.EqualTo("ccc"));
            Assert.That(match["GRP"].Index, Is.EqualTo(8));
            Assert.That(match["GRP"].Length, Is.EqualTo(3));

            Assert.That(match["GrP"], Is.Not.Null);
            Assert.That(match["GrP"].Value, Is.EqualTo("dd"));
            Assert.That(match["GrP"].Index, Is.EqualTo(11));
            Assert.That(match["GrP"].Length, Is.EqualTo(2));
        }

        [Test]
        public void should_allow_duplicate_names()
        {
            var re = new PcreRegex(@"(?<g>a)?(?<g>b)?(?<g>c)?", PcreOptions.DuplicateNames);
            var match = re.Match("b");

            Assert.That(match, Is.Not.Null);
            Assert.That(match["g"].Value, Is.EqualTo("b"));

            Assert.That(match.GetGroups("g").Select(g => g.IsMatch), Is.EqualTo(new[] { false, true, false }));

            match = re.Match("bc");
            Assert.That(match, Is.Not.Null);
            Assert.That(match["g"].Value, Is.EqualTo("b"));

            Assert.That(match.GetGroups("g").Select(g => g.IsMatch), Is.EqualTo(new[] { false, true, true }));
        }

        [Test]
        public void should_return_marks()
        {
            var re = new PcreRegex(@"a(?:(*MARK:foo)b(*MARK:bar)|c)");
            var match = re.Match("ab");

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Mark, Is.EqualTo("bar"));

            match = re.Match("ac");
            Assert.That(match, Is.Not.Null);
            Assert.That(match.Mark, Is.Null);
        }

        [Test]
        public void should_execute_passing_callout()
        {
            var re = new PcreRegex(@"a(?C42)b");

            var calls = 0;

            var match = re.Match("ab", data =>
            {
                Assert.That(data.Number, Is.EqualTo(42));
                ++calls;
                return PcreCalloutResult.Pass;
            });

            Assert.That(match, Is.Not.Null);
            Assert.That(calls, Is.EqualTo(1));
        }

        [Test]
        public void should_execute_failing_callout()
        {
            var re = new PcreRegex(@".(?C42)");

            var first = true;

            var match = re.Match("ab", data =>
            {
                Assert.That(data.Number, Is.EqualTo(42));
                if (first)
                {
                    first = false;
                    return PcreCalloutResult.Fail;
                }
                return PcreCalloutResult.Pass;
            });

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Value, Is.EqualTo("b"));
        }

        [Test]
        public void should_execute_aborting_callout()
        {
            var re = new PcreRegex(@".(?C42)");

            var match = re.Match("ab", data =>
            {
                Assert.That(data.Number, Is.EqualTo(42));
                return PcreCalloutResult.Abort;
            });

            Assert.That(match, Is.Null);
        }

        [Test]
        public void should_auto_callout()
        {
            var re = new PcreRegex(@"a.c", PcreOptions.AutoCallout);

            var count = 0;

            var match = re.Match("abc", data =>
            {
                Assert.That(data.Number, Is.EqualTo(255));
                ++count;
                return PcreCalloutResult.Pass;
            });

            Assert.That(match, Is.Not.Null);
            Assert.That(count, Is.EqualTo(4));
        }
    }
}
