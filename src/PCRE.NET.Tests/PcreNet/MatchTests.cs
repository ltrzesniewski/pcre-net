using System;
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
    }
}
