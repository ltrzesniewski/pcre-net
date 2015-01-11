using System.Linq;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    public class SplitTests
    {
        [Test]
        public void should_split_string()
        {
            var re = new PcreRegex(@"\s+");
            var result = re.Split("foo bar   baz").ToList();

            Assert.That(result, Is.EqualTo(new[] { "foo", "bar", "baz" }));
        }

        [Test]
        public void should_split_string_at_start_and_end()
        {
            var re = new PcreRegex(@"\s+");
            var result = re.Split("  foo bar   baz ").ToList();

            Assert.That(result, Is.EqualTo(new[] { string.Empty, "foo", "bar", "baz", string.Empty }));
        }

        [Test]
        public void should_split_string_up_to_given_part_count()
        {
            var re = new PcreRegex(@"\s+");
            var result = re.Split("foo bar   baz  abc", 2).ToList();

            Assert.That(result, Is.EqualTo(new[] { "foo", "bar", "baz  abc" }));
        }

        [Test]
        public void should_split_string_starting_at_given_index()
        {
            var re = new PcreRegex(@"\s+");
            var result = re.Split("foo bar   baz  abc def", -1, 12).ToList();

            Assert.That(result, Is.EqualTo(new[] { "foo bar   baz", "abc", "def" }));
        }

        [Test]
        public void should_split_string_starting_at_given_index_and_up_to_given_part_count()
        {
            var re = new PcreRegex(@"\s+");
            var result = re.Split("foo bar   baz  abc def", 2, 6).ToList();

            Assert.That(result, Is.EqualTo(new[] { "foo bar", "baz", "abc def" }));
        }

        [Test]
        public void should_not_split_when_given_count_is_zero()
        {
            var re = new PcreRegex(@"\s+");
            var result = re.Split("foo bar baz", 0).ToList();

            Assert.That(result, Is.EqualTo(new[] { "foo bar baz" }));
        }

        [Test]
        public void should_include_captured_groups_in_result()
        {
            var re = new PcreRegex(@"<(\d)?(.+?)>");
            var result = re.Split("foo<bar>baz<42>a", PcreSplitOptions.IncludeGroupValues).ToList();

            Assert.That(result, Is.EqualTo(new[] { "foo", "bar", "baz", "4", "2", "a" }));
        }

        [Test]
        public void should_not_include_captured_groups_in_result_if_not_specified()
        {
            var re = new PcreRegex(@"<(\d)?(.+?)>");
            var result = re.Split("foo<bar>baz<42>a").ToList();

            Assert.That(result, Is.EqualTo(new[] { "foo", "baz", "a" }));
        }

        [Test]
        public void should_include_empty_groups_in_result()
        {
            var re = new PcreRegex(@"<(\d?)(.+?)>");
            var result = re.Split("foo<bar>baz<42>a", PcreSplitOptions.IncludeGroupValues).ToList();

            Assert.That(result, Is.EqualTo(new[] { "foo", "", "bar", "baz", "4", "2", "a" }));
        }

        [Test]
        public void should_split_on_empty_pattern()
        {
            var re = new PcreRegex(@"");
            var result = re.Split("foo").ToList();

            Assert.That(result, Is.EqualTo(new[] { "", "f", "o", "o", "" }));
        }
    }
}
