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
            var result = re.Replace("foo aabb bar aaabbab baz", "<$0><$1><$&>");

            Assert.That(result, Is.EqualTo("foo <aabb><bb><aabb> bar <aaabb><bb><aaabb><ab><b><ab> baz"));
        }

        [Test]
        public void should_replace_indexed_groups_with_braces()
        {
            var re = new PcreRegex(@"a+(?<grp>b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "<${1}>");

            Assert.That(result, Is.EqualTo("foo <bb> bar <bb><b> baz"));
        }

        [Test]
        public void should_replace_indexed_groups_without_braces()
        {
            var re = new PcreRegex(@"a+(?<grp>b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "<$1>");

            Assert.That(result, Is.EqualTo("foo <bb> bar <bb><b> baz"));
        }

        [Test]
        public void should_replace_indexed_groups_with_braces_at_end_of_pattern()
        {
            var re = new PcreRegex(@"a+(?<grp>b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "#${1}");

            Assert.That(result, Is.EqualTo("foo #bb bar #bb#b baz"));
        }

        [Test]
        public void should_replace_indexed_groups_without_braces_at_end_of_pattern()
        {
            var re = new PcreRegex(@"a+(?<grp>b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "#$1");

            Assert.That(result, Is.EqualTo("foo #bb bar #bb#b baz"));
        }

        [Test]
        public void should_replace_named_groups()
        {
            var re = new PcreRegex(@"a+(?<grp>b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "<${grp}>");

            Assert.That(result, Is.EqualTo("foo <bb> bar <bb><b> baz"));
        }

        [Test]
        public void should_insert_dollar()
        {
            var re = new PcreRegex(@"a+(b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "$$-$");

            Assert.That(result, Is.EqualTo("foo $-$ bar $-$$-$ baz"));
        }

        [Test]
        public void should_insert_pre_match()
        {
            var re = new PcreRegex(@"a+(b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "<$`>");

            Assert.That(result, Is.EqualTo("foo <foo > bar <foo aabb bar ><foo aabb bar aaabb> baz"));
        }

        [Test]
        public void should_insert_post_match()
        {
            var re = new PcreRegex(@"a+(b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "<$'>");

            Assert.That(result, Is.EqualTo("foo < bar aaabbab baz> bar <ab baz>< baz> baz"));
        }

        [Test]
        public void should_insert_subject()
        {
            var re = new PcreRegex(@"a+(b+)", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "<$_>");

            Assert.That(result, Is.EqualTo("foo <foo aabb bar aaabbab baz> bar <foo aabb bar aaabbab baz><foo aabb bar aaabbab baz> baz"));
        }

        [Test]
        public void should_insert_last_matched_group()
        {
            var re = new PcreRegex(@"a+(b+)(c+)?", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbabcc baz", "<$+>");

            Assert.That(result, Is.EqualTo("foo <bb> bar <bb><cc> baz"));
        }

        [Test]
        public void should_insert_nothing_if_no_group_matched()
        {
            var re = new PcreRegex(@"a+(b+)?");
            var result = re.Replace("foo aabb bar baz", "<$+>");

            Assert.That(result, Is.EqualTo("foo <bb> b<>r b<>z"));
        }

        [Test]
        public void should_insert_nothing_if_there_are_no_groups()
        {
            var re = new PcreRegex(@"a+");
            var result = re.Replace("foo bar", "<$+>");

            Assert.That(result, Is.EqualTo("foo b<>r"));
        }

        [Test]
        [TestCase("<$2$$$1$>", ExpectedResult = "foo <$2$bb$> bar <$2$bb$><$2$b$> baz")]
        [TestCase("<>${2", ExpectedResult = "foo <>${2 bar <>${2<>${2 baz")]
        [TestCase("<$42>", ExpectedResult = "foo <$42> bar <$42><$42> baz")]
        [TestCase("<$99999999999999999999>", ExpectedResult = "foo <$99999999999999999999> bar <$99999999999999999999><$99999999999999999999> baz")]
        [TestCase("<${x}>", ExpectedResult = "foo <${x}> bar <${x}><${x}> baz")]
        [TestCase("$42", ExpectedResult = "foo $42 bar $42$42 baz")]
        [TestCase("${x}", ExpectedResult = "foo ${x} bar ${x}${x} baz")]
        [TestCase("${x", ExpectedResult = "foo ${x bar ${x${x baz")]
        [TestCase("$", ExpectedResult = "foo $ bar $$ baz")]
        public string should_not_throw_on_invalid_replacement_patterns(string replacement)
        {
            var re = new PcreRegex(@"a+(?<n>b+)", PcreOptions.IgnoreCase);
            return re.Replace("foo aabb bar aaabbab baz", replacement);
        }

        [Test]
        public void should_only_replace_given_count()
        {
            var re = new PcreRegex(@"a+", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "X", 2);

            Assert.That(result, Is.EqualTo("foo Xbb bXr aaabbab baz"));
        }

        [Test]
        public void should_not_replace_when_count_is_zero()
        {
            var re = new PcreRegex(@"a+", PcreOptions.IgnoreCase);
            var result = re.Replace("foo aabb bar aaabbab baz", "X", 0);

            Assert.That(result, Is.EqualTo("foo aabb bar aaabbab baz"));
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

        [Test]
        [TestCase("#", ExpectedResult = "aab#ab#")]
        [TestCase("$_", ExpectedResult = "aabaabababaabab")]
        [TestCase("$`", ExpectedResult = "aabaababaabab")]
        [TestCase("$'", ExpectedResult = "aabaabababab")]
        [TestCase("$+", ExpectedResult = "aabab")]
        [TestCase("$&", ExpectedResult = "aabab")]
        public string should_handle_backslash_k_in_lookahead(string replacement)
        {
            var re = new PcreRegex(@"(?=a+b\K)");
            return re.Replace("aabab", replacement);
        }

        [Test]
        public void readme_replace_example()
        {
            var result = PcreRegex.Replace("hello, world!!!", @"\p{P}+", "<$&>");
            Assert.That(result, Is.EqualTo("hello<,> world<!!!>"));
        }
    }
}
