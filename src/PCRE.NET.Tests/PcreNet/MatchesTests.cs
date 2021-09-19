using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using PCRE.Tests.Support;

namespace PCRE.Tests.PcreNet
{
    [TestFixture]
    [SuppressMessage("ReSharper", "ArrangeDefaultValueWhenTypeNotEvident")]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
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
        public void should_return_all_matches_ref()
        {
            var re = new PcreRegex(@"a(b)a");
            var matches = re.Matches("foo aba bar aba baz".AsSpan())
                            .ToList(m => (Value: m.Value.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

            Assert.That(matches, Has.Count.EqualTo(2));

            Assert.That(matches[0].Value, Is.EqualTo("aba"));
            Assert.That(matches[0].Index, Is.EqualTo(4));
            Assert.That(matches[0].Length, Is.EqualTo(3));

            Assert.That(matches[0].Groups[1].Value, Is.EqualTo("b"));
            Assert.That(matches[0].Groups[1].Index, Is.EqualTo(5));
            Assert.That(matches[0].Groups[1].Length, Is.EqualTo(1));

            Assert.That(matches[1].Value, Is.EqualTo("aba"));
            Assert.That(matches[1].Index, Is.EqualTo(12));
            Assert.That(matches[1].Length, Is.EqualTo(3));

            Assert.That(matches[1].Groups[1].Value, Is.EqualTo("b"));
            Assert.That(matches[1].Groups[1].Index, Is.EqualTo(13));
            Assert.That(matches[1].Groups[1].Length, Is.EqualTo(1));
        }

        [Test]
        public void should_return_all_matches_buf()
        {
            var re = new PcreRegex(@"a(b)a");
            var matches = re.CreateMatchBuffer().Matches("foo aba bar aba baz".AsSpan())
                            .ToList(m => (Value: m.Value.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

            Assert.That(matches, Has.Count.EqualTo(2));

            Assert.That(matches[0].Value, Is.EqualTo("aba"));
            Assert.That(matches[0].Index, Is.EqualTo(4));
            Assert.That(matches[0].Length, Is.EqualTo(3));

            Assert.That(matches[0].Groups[1].Value, Is.EqualTo("b"));
            Assert.That(matches[0].Groups[1].Index, Is.EqualTo(5));
            Assert.That(matches[0].Groups[1].Length, Is.EqualTo(1));

            Assert.That(matches[1].Value, Is.EqualTo("aba"));
            Assert.That(matches[1].Index, Is.EqualTo(12));
            Assert.That(matches[1].Length, Is.EqualTo(3));

            Assert.That(matches[1].Groups[1].Value, Is.EqualTo("b"));
            Assert.That(matches[1].Groups[1].Index, Is.EqualTo(13));
            Assert.That(matches[1].Groups[1].Length, Is.EqualTo(1));
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
        public void should_handle_empty_matches_ref()
        {
            var re = new PcreRegex(@"(?=(a))");
            var matches = re.Matches("aaabbaa".AsSpan())
                            .ToList(m => (Value: m.Value.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

            Assert.That(matches, Has.Count.EqualTo(5));

            Assert.That(matches.Select(m => m.Index), Is.EqualTo(new[] { 0, 1, 2, 5, 6 }));
            Assert.That(matches.Select(m => m.Length), Is.All.EqualTo(0));
            Assert.That(matches.Select(m => m.Value), Is.All.EqualTo(String.Empty));

            Assert.That(matches.Select(m => m.Groups[1].Index), Is.EqualTo(new[] { 0, 1, 2, 5, 6 }));
            Assert.That(matches.Select(m => m.Groups[1].Length), Is.All.EqualTo(1));
            Assert.That(matches.Select(m => m.Groups[1].Value), Is.All.EqualTo("a"));
        }

        [Test]
        public void should_handle_empty_matches_buf()
        {
            var re = new PcreRegex(@"(?=(a))");
            var matches = re.CreateMatchBuffer().Matches("aaabbaa".AsSpan())
                            .ToList(m => (Value: m.Value.ToString(), m.Index, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

            Assert.That(matches, Has.Count.EqualTo(5));

            Assert.That(matches.Select(m => m.Index), Is.EqualTo(new[] { 0, 1, 2, 5, 6 }));
            Assert.That(matches.Select(m => m.Length), Is.All.EqualTo(0));
            Assert.That(matches.Select(m => m.Value), Is.All.EqualTo(String.Empty));

            Assert.That(matches.Select(m => m.Groups[1].Index), Is.EqualTo(new[] { 0, 1, 2, 5, 6 }));
            Assert.That(matches.Select(m => m.Groups[1].Length), Is.All.EqualTo(1));
            Assert.That(matches.Select(m => m.Groups[1].Value), Is.All.EqualTo("a"));
        }

        [Test]
        public void should_match_from_index()
        {
            var re = new PcreRegex(@"a");
            var matches = re.Matches("foo bar baz", 6).ToList();

            Assert.That(matches, Has.Count.EqualTo(1));
        }

        [Test]
        public void should_match_from_index_ref()
        {
            var re = new PcreRegex(@"a");
            var matches = re.Matches("foo bar baz".AsSpan(), 6).ToList(_ => true);

            Assert.That(matches, Has.Count.EqualTo(1));
        }

        [Test]
        public void should_match_from_index_buf()
        {
            var re = new PcreRegex(@"a");
            var matches = re.CreateMatchBuffer().Matches("foo bar baz".AsSpan(), 6).ToList(_ => true);

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
        public void should_match_starting_at_end_of_string_ref()
        {
            var re = new PcreRegex(@"(?<=a)");
            var matches = re.Matches("xxa".AsSpan(), 3).ToList(_ => true);

            Assert.That(matches, Has.Count.EqualTo(1));
        }

        [Test]
        public void should_match_starting_at_end_of_string_buf()
        {
            var re = new PcreRegex(@"(?<=a)");
            var matches = re.CreateMatchBuffer().Matches("xxa".AsSpan(), 3).ToList(_ => true);

            Assert.That(matches, Has.Count.EqualTo(1));
        }

        [Test]
        public void should_handle_end_before_start()
        {
            var re = new PcreRegex(@"(?=a+b\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });
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
        public void should_handle_end_before_start_ref()
        {
            var re = new PcreRegex(@"(?=a+b\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });
            var matches = re.Matches("aaabab".AsSpan())
                            .ToList(m => (Value: m.Value.ToString(), m.Index, m.EndIndex, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

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
        public void should_handle_end_before_start_buf()
        {
            var re = new PcreRegex(@"(?=a+b\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });
            var matches = re.CreateMatchBuffer().Matches("aaabab".AsSpan())
                            .ToList(m => (Value: m.Value.ToString(), m.Index, m.EndIndex, m.Length, Groups: m.Groups.ToList(g => (Value: g.Value.ToString(), g.Index, g.Length))));

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
        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
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

            Assert.Throws<PcreCalloutException>(() => seq.ToList());
            Assert.That(resultCount, Is.EqualTo(2));
        }

        [Test]
        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        public void should_report_callout_exception_ref()
        {
            var re = new PcreRegex(@"a(?C1)");

            var resultCount = 0;

            Assert.Throws<PcreCalloutException>(() =>
            {
                re.Matches("aaa".AsSpan(), 0, callout =>
                {
                    if (callout.StartOffset >= 2)
                        throw new InvalidOperationException("Simulated exception");

                    return PcreCalloutResult.Pass;
                }).ToList(_ =>
                {
                    ++resultCount;
                    return true;
                });
            });

            Assert.That(resultCount, Is.EqualTo(2));
        }

        [Test]
        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
        public void should_report_callout_exception_buf()
        {
            var re = new PcreRegex(@"a(?C1)");
            var buffer = re.CreateMatchBuffer();

            var resultCount = 0;

            Assert.Throws<PcreCalloutException>(() =>
            {
                buffer.Matches("aaa".AsSpan(), 0, callout =>
                {
                    if (callout.StartOffset >= 2)
                        throw new InvalidOperationException("Simulated exception");

                    return PcreCalloutResult.Pass;
                }).ToList(_ =>
                {
                    ++resultCount;
                    return true;
                });
            });

            Assert.That(resultCount, Is.EqualTo(2));
        }

        [Test]
        public void should_throw_on_null_subject()
        {
            var re = new PcreRegex("a");
            Assert.Throws<ArgumentNullException>(() => re.Matches(default(string)!));
        }

        [Test]
        public void should_throw_on_null_settings()
        {
            var re = new PcreRegex("a");
            Assert.Throws<ArgumentNullException>(() => re.Matches("a", 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
        }

        [Test]
        public void should_throw_on_null_settings_ref()
        {
            var re = new PcreRegex("a");
            Assert.Throws<ArgumentNullException>(() => re.Matches("a".AsSpan(), 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(2)]
        public void should_throw_on_invalid_start_index(int startIndex)
        {
            var re = new PcreRegex(@"a");
            Assert.Throws<ArgumentOutOfRangeException>(() => re.Matches("a", startIndex));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(2)]
        public void should_throw_on_invalid_start_index_ref(int startIndex)
        {
            var re = new PcreRegex(@"a");
            Assert.Throws<ArgumentOutOfRangeException>(() => re.Matches("a".AsSpan(), startIndex));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(2)]
        public void should_throw_on_invalid_start_index_buf(int startIndex)
        {
            var re = new PcreRegex(@"a");
            var buffer = re.CreateMatchBuffer();
            Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Matches("a".AsSpan(), startIndex));
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
