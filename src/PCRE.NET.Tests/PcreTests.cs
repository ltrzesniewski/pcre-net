using System;
using NUnit.Framework;

namespace PCRE.Tests
{
    [TestFixture]
    public class PcreTests
    {
        [Test]
        [TestCase(@"^A.*Z$")]
        [TestCase(@"Foo$")]
        public void should_compile_correct_pattern(string pattern)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Pcre(pattern);
            Assert.Pass();
        }

        [Test]
        [TestCase(@"A(B")]
        [TestCase(@"A{3,2}")]
        [TestCase(@"A[B")]
        [TestCase(@"\p{Foo}")]
        public void should_throw_on_invalid_pattern(string pattern)
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Pcre(pattern);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        [TestCase(@"^A.*Z$", "AfooZ")]
        [TestCase(@"^A(.*)Z$", "AfooZ")]
        [TestCase(@"^\p{L}+$", "Abçdë")]
        public void should_match_pattern(string pattern, string subject)
        {
            Assert.That(new Pcre(pattern).IsMatch(subject), Is.True);
            Assert.That(new Pcre(pattern, PcreOptions.Compiled).IsMatch(subject), Is.True);
        }

        [Test]
        [TestCase(@"^A.*Z$", "Afoo")]
        [TestCase(@"^\p{L}+$", "Abc123abc")]
        public void should_not_match_pattern(string pattern, string subject)
        {
            Assert.That(new Pcre(pattern).IsMatch(subject), Is.False);
            Assert.That(new Pcre(pattern, PcreOptions.Compiled).IsMatch(subject), Is.False);
        }

        [Test]
        public void should_handle_ignore_case()
        {
            var re = new Pcre("aBc");
            Assert.That(re.IsMatch("Abc"), Is.False);

            re = new Pcre("aBc", PcreOptions.IgnoreCase);
            Assert.That(re.IsMatch("Abc"), Is.True);
        }

        [Test]
        public void should_handle_ignore_whitespace()
        {
            var re = new Pcre("^a b$");
            Assert.That(re.IsMatch("ab"), Is.False);

            re = new Pcre("^a b$", PcreOptions.IgnorePatternWhitespace);
            Assert.That(re.IsMatch("ab"), Is.True);
        }

        [Test]
        public void should_handle_singleline()
        {
            var re = new Pcre("^a.*b$");
            Assert.That(re.IsMatch("a\r\nb"), Is.False);

            re = new Pcre("^a.*b$", PcreOptions.Singleline);
            Assert.That(re.IsMatch("a\r\nb"), Is.True);
        }

        [Test]
        public void should_handle_multiline()
        {
            var re = new Pcre("^aaa$");
            Assert.That(re.IsMatch("aaa\r\nbbb"), Is.False);

            re = new Pcre("^aaa$", PcreOptions.MultiLine);
            Assert.That(re.IsMatch("aaa\r\nbbb"), Is.True);
        }

        [Test]
        public void should_handle_ecmascript()
        {
            var re = new Pcre(@"^\w$");
            Assert.That(re.IsMatch("à"), Is.True);

            re = new Pcre(@"^\w$", PcreOptions.ECMAScript);
            Assert.That(re.IsMatch("à"), Is.False);
        }
    }
}
