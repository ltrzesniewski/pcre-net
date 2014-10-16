using System.Globalization;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    public class ReplaceTests
    {
        [Test]
        public void should_replace_matches_with_callback()
        {
            var re = new PcreRegex(@"a+", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aaa bar aAAa baz", match => match.Length.ToString(CultureInfo.InvariantCulture));

            Assert.That(result, Is.EqualTo("foo 3 b1r 4 b1z"));
        }

        [Test]
        public void should_replace_matches_with_pattern()
        {
            var re = new PcreRegex(@"a+(b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "<$0><$1>");

            Assert.That(result, Is.EqualTo("foo <aabb><bb> bar <aaabb><bb><ab><b> baz"));
        }

        [Test]
        public void should_not_throw_on_invalid_replacement_patterns()
        {
            var re = new PcreRegex(@"a+(b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "<$2$$1$>");

            Assert.That(result, Is.EqualTo("foo <$2$bb$> bar <$2$bb$><$2$b$> baz"));
        }

        [Test]
        public void should_only_replace_given_count()
        {
            var re = new PcreRegex(@"a+", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "X", 2);

            Assert.That(result, Is.EqualTo("foo Xbb bXr aaabbab baz"));
        }

        [Test]
        public void should_start_at_given_index()
        {
            var re = new PcreRegex(@"a+", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "X", -1, 12);

            Assert.That(result, Is.EqualTo("foo aabb bar XbbXb bXz"));
        }

        [Test]
        public void should_start_at_given_index_and_replace_count()
        {
            var re = new PcreRegex(@"a+", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "X", 2, 8);

            Assert.That(result, Is.EqualTo("foo aabb bXr Xbbab baz"));
        }
    }
}
