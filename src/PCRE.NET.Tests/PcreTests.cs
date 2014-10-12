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
            Assert.IsTrue(new Pcre(pattern).IsMatch(subject));
        }

        [Test]
        [TestCase(@"^A.*Z$", "Afoo")]
        [TestCase(@"^\p{L}+$", "Abc123abc")]
        public void should_not_match_pattern(string pattern, string subject)
        {
            Assert.IsFalse(new Pcre(pattern).IsMatch(subject));
        }
    }
}
