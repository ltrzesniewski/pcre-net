using System;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    public class PcreRefMatchTests
    {
        [Test]
        public void should_have_undefined_value_in_default_ref_match()
        {
            var match = default(PcreRefMatch);

            Assert.That(match.Success, Is.False);
            Assert.That(match.CaptureCount, Is.EqualTo(0));
            Assert.That(match.Value.ToString(), Is.SameAs(string.Empty));
            Assert.That(match.Index, Is.EqualTo(-1));
            Assert.That(match.EndIndex, Is.EqualTo(-1));
            Assert.That(match.Length, Is.EqualTo(0));
            Assert.That(match.IsPartialMatch, Is.False);
            Assert.That(match.Mark.Length, Is.EqualTo(0));

            Assert.That(match[0].Success, Is.False);
            Assert.That(match[0].IsDefined, Is.False);
            Assert.That(match[0].Value.ToString(), Is.SameAs(string.Empty));
            Assert.That(match[0].Index, Is.EqualTo(-1));
            Assert.That(match[0].EndIndex, Is.EqualTo(-1));
            Assert.That(match[0].Length, Is.EqualTo(0));
        }

        [Test]
        public void should_reuse_ref_match_output_vector()
        {
            var re = new PcreRegex(".");
            var enumerator = re.Matches("ab".AsSpan()).GetEnumerator();

            Assert.That(enumerator.MoveNext(), Is.True);

            var match = enumerator.Current;
            var copy = match;

            Assert.That(match.Value.ToString(), Is.EqualTo("a"));
            Assert.That(copy.Value.ToString(), Is.EqualTo("a"));

            Assert.That(enumerator.MoveNext(), Is.True);

            Assert.That(match.Value.ToString(), Is.EqualTo("b"));
            Assert.That(copy.Value.ToString(), Is.EqualTo("b"));
        }

        [Test]
        public void should_copy_ref_match()
        {
            var re = new PcreRegex(".");
            var enumerator = re.Matches("ab".AsSpan()).GetEnumerator();

            Assert.That(enumerator.MoveNext(), Is.True);

            var match = enumerator.Current;
            var copy = match.Copy();

            Assert.That(match.Value.ToString(), Is.EqualTo("a"));
            Assert.That(copy.Value.ToString(), Is.EqualTo("a"));

            Assert.That(enumerator.MoveNext(), Is.True);

            Assert.That(match.Value.ToString(), Is.EqualTo("b"));
            Assert.That(copy.Value.ToString(), Is.EqualTo("a"));
        }
    }
}
