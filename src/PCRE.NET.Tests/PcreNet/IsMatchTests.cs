﻿using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public class IsMatchTests
    {
        [Test]
        [TestCase(@"^A.*Z$")]
        [TestCase(@"Foo$")]
        public void should_compile_correct_pattern(string pattern)
        {
            _ = new PcreRegex(pattern);
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
                _ = new PcreRegex(pattern);
            }
            catch (PcrePatternException ex)
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
            Assert.That(new PcreRegex(pattern).IsMatch(subject), Is.True);
            Assert.That(new PcreRegex(pattern).IsMatch(subject.AsSpan()), Is.True);
            Assert.That(new PcreRegex(pattern).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.True);
            Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject), Is.True);
            Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject.AsSpan()), Is.True);
            Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.True);
        }

        [Test]
        [TestCase(@"^A.*Z$", "Afoo")]
        [TestCase(@"^\p{L}+$", "Abc123abc")]
        public void should_not_match_pattern(string pattern, string subject)
        {
            Assert.That(new PcreRegex(pattern).IsMatch(subject), Is.False);
            Assert.That(new PcreRegex(pattern).IsMatch(subject.AsSpan()), Is.False);
            Assert.That(new PcreRegex(pattern).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.False);
            Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject), Is.False);
            Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).IsMatch(subject.AsSpan()), Is.False);
            Assert.That(new PcreRegex(pattern, PcreOptions.Compiled).CreateMatchBuffer().IsMatch(subject.AsSpan()), Is.False);
        }

        [Test]
        public void should_handle_ignore_case()
        {
            var re = new PcreRegex("aBc");
            Assert.That(re.IsMatch("Abc"), Is.False);
            Assert.That(re.IsMatch("Abc".AsSpan()), Is.False);
            Assert.That(re.CreateMatchBuffer().IsMatch("Abc".AsSpan()), Is.False);

            re = new PcreRegex("aBc", PcreOptions.IgnoreCase);
            Assert.That(re.IsMatch("Abc"), Is.True);
            Assert.That(re.IsMatch("Abc".AsSpan()), Is.True);
            Assert.That(re.CreateMatchBuffer().IsMatch("Abc".AsSpan()), Is.True);
        }

        [Test]
        public void should_handle_ignore_whitespace()
        {
            var re = new PcreRegex("^a b$");
            Assert.That(re.IsMatch("ab"), Is.False);
            Assert.That(re.IsMatch("ab".AsSpan()), Is.False);
            Assert.That(re.CreateMatchBuffer().IsMatch("ab".AsSpan()), Is.False);

            re = new PcreRegex("^a b$", PcreOptions.IgnorePatternWhitespace);
            Assert.That(re.IsMatch("ab"), Is.True);
            Assert.That(re.IsMatch("ab".AsSpan()), Is.True);
            Assert.That(re.CreateMatchBuffer().IsMatch("ab".AsSpan()), Is.True);
        }

        [Test]
        public void should_handle_singleline()
        {
            var re = new PcreRegex("^a.*b$");
            Assert.That(re.IsMatch("a\r\nb"), Is.False);
            Assert.That(re.IsMatch("a\r\nb".AsSpan()), Is.False);
            Assert.That(re.CreateMatchBuffer().IsMatch("a\r\nb".AsSpan()), Is.False);

            re = new PcreRegex("^a.*b$", PcreOptions.Singleline);
            Assert.That(re.IsMatch("a\r\nb"), Is.True);
            Assert.That(re.IsMatch("a\r\nb".AsSpan()), Is.True);
            Assert.That(re.CreateMatchBuffer().IsMatch("a\r\nb".AsSpan()), Is.True);
        }

        [Test]
        public void should_handle_multiline()
        {
            var re = new PcreRegex("^aaa$");
            Assert.That(re.IsMatch("aaa\r\nbbb"), Is.False);
            Assert.That(re.IsMatch("aaa\r\nbbb".AsSpan()), Is.False);
            Assert.That(re.CreateMatchBuffer().IsMatch("aaa\r\nbbb".AsSpan()), Is.False);

            re = new PcreRegex("^aaa$", PcreOptions.MultiLine);
            Assert.That(re.IsMatch("aaa\r\nbbb"), Is.True);
            Assert.That(re.IsMatch("aaa\r\nbbb".AsSpan()), Is.True);
            Assert.That(re.CreateMatchBuffer().IsMatch("aaa\r\nbbb".AsSpan()), Is.True);
        }

        [Test]
        public void should_handle_javascript()
        {
            var re = new PcreRegex(@"^\U$", PcreOptions.JavaScript);
            Assert.That(re.IsMatch("U"), Is.True);
            Assert.That(re.IsMatch("U".AsSpan()), Is.True);
            Assert.That(re.CreateMatchBuffer().IsMatch("U".AsSpan()), Is.True);

            var ex = Assert.Throws<PcrePatternException>(() => _ = new PcreRegex(@"^\U$"));
            Assert.That(ex!.ErrorCode, Is.EqualTo(PcreErrorCode.UnsupportedEscapeSequence));
        }

        [Test]
        public void should_handle_unicode_character_properties()
        {
            var re = new PcreRegex(@"^\w$");
            Assert.That(re.IsMatch("à"), Is.False);
            Assert.That(re.IsMatch("à".AsSpan()), Is.False);
            Assert.That(re.CreateMatchBuffer().IsMatch("à".AsSpan()), Is.False);

            re = new PcreRegex(@"^\w$", PcreOptions.Unicode);
            Assert.That(re.IsMatch("à"), Is.True);
            Assert.That(re.IsMatch("à".AsSpan()), Is.True);
            Assert.That(re.CreateMatchBuffer().IsMatch("à".AsSpan()), Is.True);
        }

        [Test]
        public void should_match_from_index()
        {
            var re = new PcreRegex(@"a");
            Assert.That(re.IsMatch("foobar", 5), Is.False);
            Assert.That(re.IsMatch("foobar".AsSpan(), 5), Is.False);
            Assert.That(re.CreateMatchBuffer().IsMatch("foobar".AsSpan(), 5), Is.False);
        }

        [Test]
        public void should_match_starting_at_end_of_string()
        {
            var re = new PcreRegex(@"(?<=a)");
            Assert.That(re.IsMatch("xxa", 3), Is.True);
            Assert.That(re.IsMatch("xxa".AsSpan(), 3), Is.True);
            Assert.That(re.CreateMatchBuffer().IsMatch("xxa".AsSpan(), 3), Is.True);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(2)]
        public void should_throw_on_invalid_start_index(int startIndex)
        {
            var re = new PcreRegex(@"a");
            Assert.Throws<ArgumentOutOfRangeException>(() => re.IsMatch("a", startIndex));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(2)]
        public void should_throw_on_invalid_start_index_ref(int startIndex)
        {
            var re = new PcreRegex(@"a");
            Assert.Throws<ArgumentOutOfRangeException>(() => re.IsMatch("a".AsSpan(), startIndex));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(2)]
        public void should_throw_on_invalid_start_index_buf(int startIndex)
        {
            var re = new PcreRegex(@"a");
            var buffer = re.CreateMatchBuffer();
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.IsMatch("a".AsSpan(), startIndex));
        }
    }
}
