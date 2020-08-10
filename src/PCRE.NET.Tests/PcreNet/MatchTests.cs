﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            Assert.That(match.Success, Is.True);
            Assert.That(match.CaptureCount, Is.EqualTo(1));
            Assert.That(match.Value, Is.EqualTo("aaabbccc"));
            Assert.That(match.Index, Is.EqualTo(3));
            Assert.That(match.EndIndex, Is.EqualTo(11));
            Assert.That(match.Length, Is.EqualTo(8));

            Assert.That(match[1], Is.Not.Null);
            Assert.That(match[1].Success, Is.True);
            Assert.That(match[1].Value, Is.EqualTo("bb"));
            Assert.That(match[1].Index, Is.EqualTo(6));
            Assert.That(match[1].Length, Is.EqualTo(2));

            Assert.That(match.Groups[1], Is.SameAs(match[1]));
        }

        [Test]
        public void should_match_pattern_ref()
        {
            var re = new PcreRegex(@"a+(b+)c+");
            var match = re.Match("xxxaaabbccczzz".AsSpan());

            Assert.That(match.Success, Is.True);
            Assert.That(match.CaptureCount, Is.EqualTo(1));
            Assert.That(match.Value.ToString(), Is.EqualTo("aaabbccc"));
            Assert.That(match.Index, Is.EqualTo(3));
            Assert.That(match.EndIndex, Is.EqualTo(11));
            Assert.That(match.Length, Is.EqualTo(8));

            Assert.That(match[1].Success, Is.True);
            Assert.That(match[1].Value.ToString(), Is.EqualTo("bb"));
            Assert.That(match[1].Index, Is.EqualTo(6));
            Assert.That(match[1].Length, Is.EqualTo(2));

            Assert.That(match[2].Success, Is.False);
        }

        [Test]
        public void should_support_multiple_groups()
        {
            var re = new PcreRegex(@"a+(b+)(c+)?(d+)e+");
            var match = re.Match("xxxaaabbddeeezzz");

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);
            Assert.That(match.CaptureCount, Is.EqualTo(3));
            Assert.That(match.Value, Is.EqualTo("aaabbddeee"));
            Assert.That(match.Index, Is.EqualTo(3));
            Assert.That(match.Length, Is.EqualTo(10));

            Assert.That(match[1], Is.Not.Null);
            Assert.That(match[1].Success, Is.True);
            Assert.That(match[1].Value, Is.EqualTo("bb"));
            Assert.That(match[1].Index, Is.EqualTo(6));
            Assert.That(match[1].Length, Is.EqualTo(2));

            Assert.That(match[2], Is.Not.Null);
            Assert.That(match[2].Success, Is.False);
            Assert.That(match[2].Value, Is.SameAs(string.Empty));
            Assert.That(match[2].Index, Is.EqualTo(-1));
            Assert.That(match[2].Length, Is.EqualTo(0));

            Assert.That(match[3], Is.Not.Null);
            Assert.That(match[3].Success, Is.True);
            Assert.That(match[3].Value, Is.EqualTo("dd"));
            Assert.That(match[3].Index, Is.EqualTo(8));
            Assert.That(match[3].Length, Is.EqualTo(2));

            Assert.That(match[4], Is.Null);
        }

        [Test]
        public void should_support_multiple_groups_ref()
        {
            var re = new PcreRegex(@"a+(b+)(c+)?(d+)e+");
            var match = re.Match("xxxaaabbddeeezzz".AsSpan());

            Assert.That(match.Success, Is.True);
            Assert.That(match.CaptureCount, Is.EqualTo(3));
            Assert.That(match.Value.ToString(), Is.EqualTo("aaabbddeee"));
            Assert.That(match.Index, Is.EqualTo(3));
            Assert.That(match.Length, Is.EqualTo(10));

            Assert.That(match[1].Success, Is.True);
            Assert.That(match[1].Value.ToString(), Is.EqualTo("bb"));
            Assert.That(match[1].Index, Is.EqualTo(6));
            Assert.That(match[1].Length, Is.EqualTo(2));

            Assert.That(match[2].Success, Is.False);
            Assert.That(match[2].Value.ToString(), Is.SameAs(string.Empty));
            Assert.That(match[2].Index, Is.EqualTo(-1));
            Assert.That(match[2].Length, Is.EqualTo(0));

            Assert.That(match[3].Success, Is.True);
            Assert.That(match[3].Value.ToString(), Is.EqualTo("dd"));
            Assert.That(match[3].Index, Is.EqualTo(8));
            Assert.That(match[3].Length, Is.EqualTo(2));

            Assert.That(match[4].Success, Is.False);
        }

        [Test]
        public void should_support_unmatched_groups_before_matched_groups()
        {
            var re = new PcreRegex(@"(a|(z))(bc)");
            var match = re.Match("abc");

            Assert.That(match.Success, Is.True);
            Assert.That(match.CaptureCount, Is.EqualTo(3));

            Assert.That(match[1].Success, Is.True);
            Assert.That(match[1].Index, Is.EqualTo(0));
            Assert.That(match[1].EndIndex, Is.EqualTo(1));
            Assert.That(match[1].Value, Is.EqualTo("a"));

            Assert.That(match[2].Success, Is.False);
            Assert.That(match[2].Index, Is.EqualTo(-1));
            Assert.That(match[2].EndIndex, Is.EqualTo(-1));
            Assert.That(match[2].Value, Is.SameAs(string.Empty));

            Assert.That(match[3].Success, Is.True);
            Assert.That(match[3].Index, Is.EqualTo(1));
            Assert.That(match[3].Length, Is.EqualTo(2));
            Assert.That(match[3].Value, Is.EqualTo("bc"));
        }

        [Test]
        public void should_support_unmatched_groups_before_matched_groups_ref()
        {
            var re = new PcreRegex(@"(a|(z))(bc)");
            var match = re.Match("abc".AsSpan());

            Assert.That(match.Success, Is.True);
            Assert.That(match.CaptureCount, Is.EqualTo(3));

            Assert.That(match[1].Success, Is.True);
            Assert.That(match[1].Index, Is.EqualTo(0));
            Assert.That(match[1].EndIndex, Is.EqualTo(1));
            Assert.That(match[1].Value.ToString(), Is.EqualTo("a"));

            Assert.That(match[2].Success, Is.False);
            Assert.That(match[2].Index, Is.EqualTo(-1));
            Assert.That(match[2].EndIndex, Is.EqualTo(-1));
            Assert.That(match[2].Value.ToString(), Is.SameAs(string.Empty));

            Assert.That(match[3].Success, Is.True);
            Assert.That(match[3].Index, Is.EqualTo(1));
            Assert.That(match[3].Length, Is.EqualTo(2));
            Assert.That(match[3].Value.ToString(), Is.EqualTo("bc"));
        }

        [Test]
        public void should_match_starting_at_end_of_string()
        {
            var re = new PcreRegex(@"(?<=a)");
            var match = re.Match("xxa", 3);

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);
        }

        [Test]
        public void should_match_starting_at_end_of_string_ref()
        {
            var re = new PcreRegex(@"(?<=a)");
            var match = re.Match("xxa".AsSpan(), 3);

            Assert.That(match.Success, Is.True);
        }

        [Test]
        public void should_handle_named_groups()
        {
            var re = new PcreRegex(@"a+(?<bees>b+)(c+)(?<dees>d+)e+");

            var match = re.Match("xxxaaabbcccddeeezzz");

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);
            Assert.That(match.CaptureCount, Is.EqualTo(3));
            Assert.That(match.Value, Is.EqualTo("aaabbcccddeee"));
            Assert.That(match.Index, Is.EqualTo(3));
            Assert.That(match.Length, Is.EqualTo(13));

            Assert.That(match["bees"], Is.Not.Null);
            Assert.That(match["bees"].Value, Is.EqualTo("bb"));
            Assert.That(match["bees"].Index, Is.EqualTo(6));
            Assert.That(match["bees"].Length, Is.EqualTo(2));

            Assert.That(match.Groups["bees"], Is.SameAs(match["bees"]));

            Assert.That(match[2], Is.Not.Null);
            Assert.That(match[2].Value, Is.EqualTo("ccc"));
            Assert.That(match[2].Index, Is.EqualTo(8));
            Assert.That(match[2].Length, Is.EqualTo(3));

            Assert.That(match["dees"], Is.Not.Null);
            Assert.That(match["dees"].Value, Is.EqualTo("dd"));
            Assert.That(match["dees"].Index, Is.EqualTo(11));
            Assert.That(match["dees"].Length, Is.EqualTo(2));

            Assert.That(match["nope"], Is.Null);
        }

        [Test]
        public void should_handle_named_groups_ref()
        {
            var re = new PcreRegex(@"a+(?<bees>b+)(c+)(?<dees>d+)e+");

            var match = re.Match("xxxaaabbcccddeeezzz".AsSpan());

            Assert.That(match.Success, Is.True);
            Assert.That(match.CaptureCount, Is.EqualTo(3));
            Assert.That(match.Value.ToString(), Is.EqualTo("aaabbcccddeee"));
            Assert.That(match.Index, Is.EqualTo(3));
            Assert.That(match.Length, Is.EqualTo(13));

            Assert.That(match["bees"].Value.ToString(), Is.EqualTo("bb"));
            Assert.That(match["bees"].Index, Is.EqualTo(6));
            Assert.That(match["bees"].Length, Is.EqualTo(2));

            Assert.That(match[2].Value.ToString(), Is.EqualTo("ccc"));
            Assert.That(match[2].Index, Is.EqualTo(8));
            Assert.That(match[2].Length, Is.EqualTo(3));

            Assert.That(match["dees"].Value.ToString(), Is.EqualTo("dd"));
            Assert.That(match["dees"].Index, Is.EqualTo(11));
            Assert.That(match["dees"].Length, Is.EqualTo(2));

            Assert.That(match["nope"].Success, Is.False);
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
        public void should_handle_case_sensitive_group_names_ref()
        {
            var re = new PcreRegex(@"a+(?<grp>b+)(?<GRP>c+)(?<GrP>d+)e+");

            var match = re.Match("xxxaaabbcccddeeezzz".AsSpan());

            Assert.That(match["grp"].Value.ToString(), Is.EqualTo("bb"));
            Assert.That(match["grp"].Index, Is.EqualTo(6));
            Assert.That(match["grp"].Length, Is.EqualTo(2));

            Assert.That(match["GRP"].Value.ToString(), Is.EqualTo("ccc"));
            Assert.That(match["GRP"].Index, Is.EqualTo(8));
            Assert.That(match["GRP"].Length, Is.EqualTo(3));

            Assert.That(match["GrP"].Value.ToString(), Is.EqualTo("dd"));
            Assert.That(match["GrP"].Index, Is.EqualTo(11));
            Assert.That(match["GrP"].Length, Is.EqualTo(2));
        }

        [Test]
        public void should_allow_duplicate_names()
        {
            var re = new PcreRegex(@"(?<g>a)?(?<g>b)?(?<g>c)?", PcreOptions.DupNames);
            var match = re.Match("b");

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);
            Assert.That(match["g"].Value, Is.EqualTo("b"));

            Assert.That(match.GetDuplicateNamedGroups("g").Select(g => g.Success), Is.EqualTo(new[] { false, true, false }));

            match = re.Match("bc");
            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);
            Assert.That(match["g"].Value, Is.EqualTo("b"));

            Assert.That(match.GetDuplicateNamedGroups("g").Select(g => g.Success), Is.EqualTo(new[] { false, true, true }));
        }

        [Test]
        public void should_allow_duplicate_names_ref()
        {
            var re = new PcreRegex(@"(?<g>a)?(?<g>b)?(?<g>c)?", PcreOptions.DupNames);
            var match = re.Match("b".AsSpan());

            Assert.That(match.Success, Is.True);
            Assert.That(match["g"].Value.ToString(), Is.EqualTo("b"));

            Assert.That(GetDuplicateNamedGroupsSuccesses(match, "g"), Is.EqualTo(new[] { false, true, false }));

            match = re.Match("bc".AsSpan());
            Assert.That(match.Success, Is.True);
            Assert.That(match["g"].Value.ToString(), Is.EqualTo("b"));

            Assert.That(GetDuplicateNamedGroupsSuccesses(match, "g"), Is.EqualTo(new[] { false, true, true }));
        }

        [Test]
        public void should_detect_duplicate_names()
        {
            var re = new PcreRegex(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?");

            var match = re.Match("bc");
            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);
            Assert.That(match["g"].Value, Is.EqualTo("b"));

            Assert.That(match.GetDuplicateNamedGroups("g").Select(g => g.Success), Is.EqualTo(new[] { false, true, true }));
        }

        [Test]
        public void should_detect_duplicate_names_ref()
        {
            var re = new PcreRegex(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?");

            var match = re.Match("bc".AsSpan());
            Assert.That(match.Success, Is.True);
            Assert.That(match["g"].Value.ToString(), Is.EqualTo("b"));

            Assert.That(GetDuplicateNamedGroupsSuccesses(match, "g"), Is.EqualTo(new[] { false, true, true }));
        }

        private static List<bool> GetDuplicateNamedGroupsSuccesses(PcreRefMatch match, string groupName)
        {
            var result = new List<bool>();

            foreach (var group in match.GetDuplicateNamedGroups(groupName))
                result.Add(group.Success);

            return result;
        }

        [Test]
        public void should_return_marks()
        {
            var re = new PcreRegex(@"a(?:(*MARK:foo)b(*MARK:bar)|c)");
            var match = re.Match("ab");

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);
            Assert.That(match.Mark, Is.EqualTo("bar"));

            match = re.Match("ac");
            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);
            Assert.That(match.Mark, Is.Null);
        }

        [Test]
        public void should_return_marks_ref()
        {
            var re = new PcreRegex(@"a(?:(*MARK:foo)b(*MARK:bar)|c)");
            var match = re.Match("ab".AsSpan());

            Assert.That(match.Success, Is.True);
            Assert.That(match.Mark.ToString(), Is.EqualTo("bar"));

            match = re.Match("ac".AsSpan());
            Assert.That(match.Success, Is.True);
            Assert.That(match.Mark.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void should_use_callout_result()
        {
            var regex = new PcreRegex(@"(\d+)(*SKIP)(?C1):\s*(\w+)");

            var match = regex.Match(
                "1542: not_this, 1764: hello",
                data => data.Number == 1
                        && int.Parse(data.Match[1].Value) % 42 == 0
                    ? PcreCalloutResult.Pass
                    : PcreCalloutResult.Fail);

            Assert.That(match[2].Value, Is.EqualTo("hello"));
        }

        [Test]
        public void should_use_callout_result_ref()
        {
            var regex = new PcreRegex(@"(\d+)(*SKIP)(?C1):\s*(\w+)");

            var match = regex.Match(
                "1542: not_this, 1764: hello".AsSpan(),
                data => data.Number == 1
                        && int.Parse(data.Match[1].Value.ToString()) % 42 == 0
                    ? PcreCalloutResult.Pass
                    : PcreCalloutResult.Fail);

            Assert.That(match[2].Value.ToString(), Is.EqualTo("hello"));
        }

        [Test]
        public void should_execute_passing_callout()
        {
            const string pattern = @"(a)(*MARK:foo)(x)?(?C42)(bc)";
            var re = new PcreRegex(pattern);

            var calls = 0;

            var match = re.Match("abc", data =>
            {
                Assert.That(data.Number, Is.EqualTo(42));
                Assert.That(data.CurrentOffset, Is.EqualTo(1));
                Assert.That(data.PatternPosition, Is.EqualTo(pattern.IndexOf("(?C42)", StringComparison.Ordinal) + 6));
                Assert.That(data.StartOffset, Is.EqualTo(0));
                Assert.That(data.LastCapture, Is.EqualTo(1));
                Assert.That(data.MaxCapture, Is.EqualTo(2));
                Assert.That(data.NextPatternItemLength, Is.EqualTo(1));
                Assert.That(data.StringOffset, Is.EqualTo(0));
                Assert.That(data.String, Is.Null);

                Assert.That(data.Match.Value, Is.EqualTo("a"));
                Assert.That(data.Match[1].Value, Is.EqualTo("a"));
                Assert.That(data.Match[2].Success, Is.False);
                Assert.That(data.Match[2].Value, Is.SameAs(string.Empty));
                Assert.That(data.Match[3].Success, Is.False);
                Assert.That(data.Match[3].Value, Is.SameAs(string.Empty));

                Assert.That(data.Match.Mark, Is.EqualTo("foo"));

                ++calls;
                return PcreCalloutResult.Pass;
            });

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);
            Assert.That(calls, Is.EqualTo(1));
        }

        [Test]
        public void should_execute_passing_callout_ref()
        {
            const string pattern = @"(a)(*MARK:foo)(x)?(?C42)(bc)";
            var re = new PcreRegex(pattern);

            var calls = 0;

            var match = re.Match("abc".AsSpan(), data =>
            {
                Assert.That(data.Number, Is.EqualTo(42));
                Assert.That(data.CurrentOffset, Is.EqualTo(1));
                Assert.That(data.PatternPosition, Is.EqualTo(pattern.IndexOf("(?C42)", StringComparison.Ordinal) + 6));
                Assert.That(data.StartOffset, Is.EqualTo(0));
                Assert.That(data.LastCapture, Is.EqualTo(1));
                Assert.That(data.MaxCapture, Is.EqualTo(2));
                Assert.That(data.NextPatternItemLength, Is.EqualTo(1));
                Assert.That(data.StringOffset, Is.EqualTo(0));
                Assert.That(data.String, Is.Null);

                Assert.That(data.Match.Value.ToString(), Is.EqualTo("a"));
                Assert.That(data.Match[1].Value.ToString(), Is.EqualTo("a"));
                Assert.That(data.Match[2].Success, Is.False);
                Assert.That(data.Match[2].Value.ToString(), Is.SameAs(string.Empty));
                Assert.That(data.Match[3].Success, Is.False);
                Assert.That(data.Match[3].Value.ToString(), Is.SameAs(string.Empty));

                Assert.That(data.Match.Mark.ToString(), Is.EqualTo("foo"));

                ++calls;
                return PcreCalloutResult.Pass;
            });

            Assert.That(match.Success, Is.True);
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
            Assert.That(match.Success, Is.True);
            Assert.That(match.Value, Is.EqualTo("b"));
        }

        [Test]
        public void should_execute_failing_callout_ref()
        {
            var re = new PcreRegex(@".(?C42)");

            var first = true;

            var match = re.Match("ab".AsSpan(), data =>
            {
                Assert.That(data.Number, Is.EqualTo(42));
                if (first)
                {
                    first = false;
                    return PcreCalloutResult.Fail;
                }

                return PcreCalloutResult.Pass;
            });

            Assert.That(match.Success, Is.True);
            Assert.That(match.Value.ToString(), Is.EqualTo("b"));
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

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.False);
        }

        [Test]
        public void should_execute_aborting_callout_ref()
        {
            var re = new PcreRegex(@".(?C42)");

            var match = re.Match("ab".AsSpan(), data =>
            {
                Assert.That(data.Number, Is.EqualTo(42));
                return PcreCalloutResult.Abort;
            });

            Assert.That(match.Success, Is.False);
        }

        [Test]
        public void should_throw_when_callout_throws()
        {
            var re = new PcreRegex(@".(?C42)");

            var ex = Assert.Throws<PcreCalloutException>(() => re.Match("ab", data => { throw new DivideByZeroException("test"); }));

            Assert.That(ex.InnerException, Is.InstanceOf<DivideByZeroException>());
        }

        [Test]
        public void should_throw_when_callout_throws_ref()
        {
            var re = new PcreRegex(@".(?C42)");

            var ex = Assert.Throws<PcreCalloutException>(() => re.Match("ab".AsSpan(), data => { throw new DivideByZeroException("test"); }));

            Assert.That(ex.InnerException, Is.InstanceOf<DivideByZeroException>());
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
            Assert.That(match.Success, Is.True);
            Assert.That(count, Is.EqualTo(4));
        }

        [Test]
        public void should_auto_callout_ref()
        {
            var re = new PcreRegex(@"a.c", PcreOptions.AutoCallout);

            var count = 0;

            var match = re.Match("abc".AsSpan(), data =>
            {
                Assert.That(data.Number, Is.EqualTo(255));
                ++count;
                return PcreCalloutResult.Pass;
            });

            Assert.That(match.Success, Is.True);
            Assert.That(count, Is.EqualTo(4));
        }

        [Test]
        public void should_provide_callout_flags()
        {
            var re = new PcreRegex(@"a(?C1)(?:(?C2)(*FAIL)|b)(?C3)");

            var startMatchList = new List<bool>();
            var backtrackList = new List<bool>();

            re.Match("abc", data =>
            {
                startMatchList.Add(data.StartMatch);
                backtrackList.Add(data.Backtrack);
                return PcreCalloutResult.Pass;
            });

            Assert.That(startMatchList, Is.EqualTo(new[] { true, false, false }));
            Assert.That(backtrackList, Is.EqualTo(new[] { false, false, true }));
        }

        [Test]
        public void should_provide_callout_flags_ref()
        {
            var re = new PcreRegex(@"a(?C1)(?:(?C2)(*FAIL)|b)(?C3)");

            var startMatchList = new List<bool>();
            var backtrackList = new List<bool>();

            re.Match("abc".AsSpan(), data =>
            {
                startMatchList.Add(data.StartMatch);
                backtrackList.Add(data.Backtrack);
                return PcreCalloutResult.Pass;
            });

            Assert.That(startMatchList, Is.EqualTo(new[] { true, false, false }));
            Assert.That(backtrackList, Is.EqualTo(new[] { false, false, true }));
        }

        [Test]
        public void should_execute_string_callout()
        {
            const string pattern = @"(a)(*MARK:foo)(x)?(?C{bar})(bc)";
            var re = new PcreRegex(pattern);

            var calls = 0;

            var match = re.Match("abc", data =>
            {
                Assert.That(data.Number, Is.EqualTo(0));
                Assert.That(data.CurrentOffset, Is.EqualTo(1));
                Assert.That(data.PatternPosition, Is.EqualTo(pattern.IndexOf("(?C{bar})", StringComparison.Ordinal) + 9));
                Assert.That(data.StartOffset, Is.EqualTo(0));
                Assert.That(data.LastCapture, Is.EqualTo(1));
                Assert.That(data.MaxCapture, Is.EqualTo(2));
                Assert.That(data.NextPatternItemLength, Is.EqualTo(1));
                Assert.That(data.StringOffset, Is.EqualTo(pattern.IndexOf("(?C{bar})", StringComparison.Ordinal) + 4));
                Assert.That(data.String, Is.EqualTo("bar"));

                Assert.That(data.Match.Value, Is.EqualTo("a"));
                Assert.That(data.Match[1].Value, Is.EqualTo("a"));
                Assert.That(data.Match[2].Success, Is.False);
                Assert.That(data.Match[2].Value, Is.SameAs(string.Empty));
                Assert.That(data.Match[3].Success, Is.False);
                Assert.That(data.Match[3].Value, Is.SameAs(string.Empty));

                Assert.That(data.Match.Mark, Is.EqualTo("foo"));

                ++calls;
                return PcreCalloutResult.Pass;
            });

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);
            Assert.That(calls, Is.EqualTo(1));
        }

        [Test]
        public void should_execute_string_callout_ref()
        {
            const string pattern = @"(a)(*MARK:foo)(x)?(?C{bar})(bc)";
            var re = new PcreRegex(pattern);

            var calls = 0;

            var match = re.Match("abc".AsSpan(), data =>
            {
                Assert.That(data.Number, Is.EqualTo(0));
                Assert.That(data.CurrentOffset, Is.EqualTo(1));
                Assert.That(data.PatternPosition, Is.EqualTo(pattern.IndexOf("(?C{bar})", StringComparison.Ordinal) + 9));
                Assert.That(data.StartOffset, Is.EqualTo(0));
                Assert.That(data.LastCapture, Is.EqualTo(1));
                Assert.That(data.MaxCapture, Is.EqualTo(2));
                Assert.That(data.NextPatternItemLength, Is.EqualTo(1));
                Assert.That(data.StringOffset, Is.EqualTo(pattern.IndexOf("(?C{bar})", StringComparison.Ordinal) + 4));
                Assert.That(data.String, Is.EqualTo("bar"));

                Assert.That(data.Match.Value.ToString(), Is.EqualTo("a"));
                Assert.That(data.Match[1].Value.ToString(), Is.EqualTo("a"));
                Assert.That(data.Match[2].Success, Is.False);
                Assert.That(data.Match[2].Value.ToString(), Is.SameAs(string.Empty));
                Assert.That(data.Match[3].Success, Is.False);
                Assert.That(data.Match[3].Value.ToString(), Is.SameAs(string.Empty));

                Assert.That(data.Match.Mark.ToString(), Is.EqualTo("foo"));

                ++calls;
                return PcreCalloutResult.Pass;
            });

            Assert.That(match.Success, Is.True);
            Assert.That(calls, Is.EqualTo(1));
        }

        [Test]
        public void should_support_unmatched_groups_before_matched_groups_in_callout()
        {
            var re = new PcreRegex(@"(a|(z))(bc)(?C42)");
            var calls = 0;

            re.Match("abc", data =>
            {
                Assert.That(data.Match[1].Success, Is.True);
                Assert.That(data.Match[1].Index, Is.EqualTo(0));
                Assert.That(data.Match[1].Length, Is.EqualTo(1));
                Assert.That(data.Match[1].Value, Is.EqualTo("a"));

                Assert.That(data.Match[2].Success, Is.False);
                Assert.That(data.Match[2].Index, Is.EqualTo(-1));
                Assert.That(data.Match[2].EndIndex, Is.EqualTo(-1));
                Assert.That(data.Match[2].Value, Is.SameAs(string.Empty));

                Assert.That(data.Match[3].Success, Is.True);
                Assert.That(data.Match[3].Index, Is.EqualTo(1));
                Assert.That(data.Match[3].Length, Is.EqualTo(2));
                Assert.That(data.Match[3].Value, Is.EqualTo("bc"));

                ++calls;
                return PcreCalloutResult.Pass;
            });

            Assert.That(calls, Is.EqualTo(1));
        }

        [Test]
        public void should_support_unmatched_groups_before_matched_groups_in_callout_ref()
        {
            var re = new PcreRegex(@"(a|(z))(bc)(?C42)");
            var calls = 0;

            re.Match("abc".AsSpan(), data =>
            {
                Assert.That(data.Match[1].Success, Is.True);
                Assert.That(data.Match[1].Index, Is.EqualTo(0));
                Assert.That(data.Match[1].Length, Is.EqualTo(1));
                Assert.That(data.Match[1].Value.ToString(), Is.EqualTo("a"));

                Assert.That(data.Match[2].Success, Is.False);
                Assert.That(data.Match[2].Index, Is.EqualTo(-1));
                Assert.That(data.Match[2].EndIndex, Is.EqualTo(-1));
                Assert.That(data.Match[2].Value.ToString(), Is.SameAs(string.Empty));

                Assert.That(data.Match[3].Success, Is.True);
                Assert.That(data.Match[3].Index, Is.EqualTo(1));
                Assert.That(data.Match[3].Length, Is.EqualTo(2));
                Assert.That(data.Match[3].Value.ToString(), Is.EqualTo("bc"));

                ++calls;
                return PcreCalloutResult.Pass;
            });

            Assert.That(calls, Is.EqualTo(1));
        }

        [Test]
        public void should_handle_end_before_start()
        {
            var re = new PcreRegex(@"(?=a+\K)");

            var match = re.Match("aaa");

            Assert.That(match, Is.Not.Null);
            Assert.That(match.Success, Is.True);
            Assert.That(match.Index, Is.EqualTo(3));
            Assert.That(match.EndIndex, Is.EqualTo(0));
            Assert.That(match.Length, Is.EqualTo(0));
            Assert.That(match.Value, Is.EqualTo(string.Empty));
        }

        [Test]
        public void should_handle_end_before_start_ref()
        {
            var re = new PcreRegex(@"(?=a+\K)");

            var match = re.Match("aaa".AsSpan());

            Assert.That(match.Success, Is.True);
            Assert.That(match.Index, Is.EqualTo(3));
            Assert.That(match.EndIndex, Is.EqualTo(0));
            Assert.That(match.Length, Is.EqualTo(0));
            Assert.That(match.Value.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void should_handle_additional_options()
        {
            var re = new PcreRegex(@"bar");

            var match = re.Match("foobar", PcreMatchOptions.None);

            Assert.That(match.Success, Is.True);

            match = re.Match("foobar", PcreMatchOptions.Anchored);

            Assert.That(match.Success, Is.False);
        }

        [Test]
        public void should_handle_additional_options_ref()
        {
            var re = new PcreRegex(@"bar");

            var match = re.Match("foobar".AsSpan(), PcreMatchOptions.None);

            Assert.That(match.Success, Is.True);

            match = re.Match("foobar".AsSpan(), PcreMatchOptions.Anchored);

            Assert.That(match.Success, Is.False);
        }

        [Test]
        public void should_handle_extra_options()
        {
            var re = new PcreRegex(@"bar", new PcreRegexSettings(PcreOptions.Literal)
            {
                ExtraCompileOptions = PcreExtraCompileOptions.MatchWord
            });

            Assert.That(re.IsMatch("foo bar baz"), Is.True);
            Assert.That(re.IsMatch("foobar baz"), Is.False);
        }

        [Test]
        public void should_handle_extra_options_ref()
        {
            var re = new PcreRegex(@"bar", new PcreRegexSettings(PcreOptions.Literal)
            {
                ExtraCompileOptions = PcreExtraCompileOptions.MatchWord
            });

            Assert.That(re.IsMatch("foo bar baz".AsSpan()), Is.True);
            Assert.That(re.IsMatch("foobar baz".AsSpan()), Is.False);
        }

        [Test]
        [TestCase(PcreMatchOptions.PartialSoft)]
        [TestCase(PcreMatchOptions.PartialHard)]
        public void should_match_partially(PcreMatchOptions options)
        {
            var re = new PcreRegex(@"(?<=abc)123");

            var match = re.Match("xyzabc12", options);

            Assert.That(match.Success, Is.False);
            Assert.That(match.IsPartialMatch, Is.True);
            Assert.That(match.Index, Is.EqualTo(6));
            Assert.That(match.EndIndex, Is.EqualTo(8));
            Assert.That(match.Length, Is.EqualTo(2));
            Assert.That(match.Value, Is.EqualTo("12"));
        }

        [Test]
        [TestCase(PcreMatchOptions.PartialSoft)]
        [TestCase(PcreMatchOptions.PartialHard)]
        public void should_match_partially_ref(PcreMatchOptions options)
        {
            var re = new PcreRegex(@"(?<=abc)123");

            var match = re.Match("xyzabc12".AsSpan(), options);

            Assert.That(match.Success, Is.False);
            Assert.That(match.IsPartialMatch, Is.True);
            Assert.That(match.Index, Is.EqualTo(6));
            Assert.That(match.EndIndex, Is.EqualTo(8));
            Assert.That(match.Length, Is.EqualTo(2));
            Assert.That(match.Value.ToString(), Is.EqualTo("12"));
        }

        [Test]
        public void should_differentiate_soft_and_hard_partial_matching()
        {
            var re = new PcreRegex(@"dog(sbody)?");

            var softMatch = re.Match("dog", PcreMatchOptions.PartialSoft);
            var hardMatch = re.Match("dog", PcreMatchOptions.PartialHard);

            Assert.That(softMatch.Success, Is.True);
            Assert.That(softMatch.IsPartialMatch, Is.False);

            Assert.That(hardMatch.Success, Is.False);
            Assert.That(hardMatch.IsPartialMatch, Is.True);
        }

        [Test]
        public void should_differentiate_soft_and_hard_partial_matching_ref()
        {
            var re = new PcreRegex(@"dog(sbody)?");

            var softMatch = re.Match("dog".AsSpan(), PcreMatchOptions.PartialSoft);
            var hardMatch = re.Match("dog".AsSpan(), PcreMatchOptions.PartialHard);

            Assert.That(softMatch.Success, Is.True);
            Assert.That(softMatch.IsPartialMatch, Is.False);

            Assert.That(hardMatch.Success, Is.False);
            Assert.That(hardMatch.IsPartialMatch, Is.True);
        }

        [Test]
        public void should_check_pattern_utf_validity()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var ex = Assert.Throws<ArgumentException>(() => new PcreRegex("A\uD800B"));
            Assert.That(ex.Message, Contains.Substring("invalid low surrogate"));
        }

        [Test]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void should_check_subject_utf_validity()
        {
            var re = new PcreRegex(@"A");
            var ex = Assert.Throws<PcreMatchException>(() => re.Match("A\uD800B"));
            Assert.That(ex.Message, Contains.Substring("invalid low surrogate"));
        }

        [Test]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void should_check_subject_utf_validity_ref()
        {
            var re = new PcreRegex(@"A");
            var ex = Assert.Throws<PcreMatchException>(() => re.Match("A\uD800B".AsSpan()));
            Assert.That(ex.Message, Contains.Substring("invalid low surrogate"));
        }

        [Test]
        public void should_handle_offset_limit()
        {
            var re = new PcreRegex(@"bar", PcreOptions.UseOffsetLimit);

            var match = re.Match("foobar");
            Assert.That(match.Success, Is.True);

            match = re.Match("foobar", new PcreMatchSettings
            {
                OffsetLimit = 3
            });
            Assert.That(match.Success, Is.True);

            match = re.Match("foobar", new PcreMatchSettings
            {
                OffsetLimit = 2
            });
            Assert.That(match.Success, Is.False);
        }

        [Test]
        public void should_handle_offset_limit_ref()
        {
            var re = new PcreRegex(@"bar", PcreOptions.UseOffsetLimit);

            var match = re.Match("foobar".AsSpan());
            Assert.That(match.Success, Is.True);

            match = re.Match("foobar".AsSpan(), new PcreMatchSettings
            {
                OffsetLimit = 3
            });
            Assert.That(match.Success, Is.True);

            match = re.Match("foobar".AsSpan(), new PcreMatchSettings
            {
                OffsetLimit = 2
            });
            Assert.That(match.Success, Is.False);
        }

        [Test]
        public void should_detect_invalid_offset_limit_usage()
        {
            var re = new PcreRegex(@"bar");
            Assert.Throws<PcreMatchException>(() => re.Match("foobar", new PcreMatchSettings
            {
                OffsetLimit = 3
            }));
        }

        [Test]
        public void should_detect_invalid_offset_limit_usage_ref()
        {
            var re = new PcreRegex(@"bar");
            Assert.Throws<PcreMatchException>(() => re.Match("foobar".AsSpan(), new PcreMatchSettings
            {
                OffsetLimit = 3
            }));
        }

        [Test]
        public void should_match_script_run()
        {
            const string subject = "123\U0001D7CF\U0001D7D0\U0001D7D1";

            var normal = PcreRegex.Match(subject, @"\d+", PcreOptions.Unicode);
            Assert.That(normal.Success, Is.True);
            Assert.That(normal.Index, Is.EqualTo(0));
            Assert.That(normal.Length, Is.EqualTo(subject.Length));

            var scriptRun = PcreRegex.Match(subject, @"(*script_run:\d+)", PcreOptions.Unicode);
            Assert.That(scriptRun.Success, Is.True);
            Assert.That(scriptRun.Index, Is.EqualTo(0));
            Assert.That(scriptRun.Length, Is.EqualTo(3));
        }

        [Test]
        public void should_match_script_run_ref()
        {
            const string subject = "123\U0001D7CF\U0001D7D0\U0001D7D1";

            var normal = new PcreRegex(@"\d+", PcreOptions.Unicode).Match(subject.AsSpan());
            Assert.That(normal.Success, Is.True);
            Assert.That(normal.Index, Is.EqualTo(0));
            Assert.That(normal.Length, Is.EqualTo(subject.Length));

            var scriptRun = new PcreRegex(@"(*script_run:\d+)", PcreOptions.Unicode).Match(subject.AsSpan());
            Assert.That(scriptRun.Success, Is.True);
            Assert.That(scriptRun.Index, Is.EqualTo(0));
            Assert.That(scriptRun.Length, Is.EqualTo(3));
        }

        [Test]
        public void readme_json_example()
        {
            const string jsonPattern = @"
                (?(DEFINE)
                    # An object is an unordered set of name/value pairs.
                    (?<object> \{
                        (?: (?&keyvalue) (?: , (?&keyvalue) )* )?
                    (?&ws) \} )
                    (?<keyvalue>
                        (?&ws) (?&string) (?&ws) : (?&value)
                    )

                    # An array is an ordered collection of values.
                    (?<array> \[
                        (?: (?&value) (?: , (?&value) )* )?
                    (?&ws) \] )

                    # A value can be a string in double quotes, or a number,
                    # or true or false or null, or an object or an array.
                    (?<value> (?&ws)
                        (?: (?&string) | (?&number) | (?&object) | (?&array) | true | false | null )
                    )

                    # A string is a sequence of zero or more Unicode characters,
                    # wrapped in double quotes, using backslash escapes.
                    (?<string>
                        "" (?: [^""\\\p{Cc}]++ | \\u[0-9A-Fa-f]{4} | \\ [""\\/bfnrt] )* ""
                        # \p{Cc} matches control characters
                    )

                    # A number is very much like a C or Java number, except that the octal
                    # and hexadecimal formats are not used.
                    (?<number>
                        -? (?: 0 | [1-9][0-9]* ) (?: \. [0-9]+ )? (?: [Ee] [-+]? [0-9]+ )?
                    )

                    # Whitespace
                    (?<ws> \s*+ )
                )

                \A (?&ws) (?&object) (?&ws) \z
            ";

            var regex = new PcreRegex(jsonPattern, PcreOptions.IgnorePatternWhitespace | PcreOptions.Compiled);

            const string subject = @"{
                ""hello"": ""world"",
                ""numbers"": [4, 8, 15, 16, 23, 42],
                ""foo"": null,
                ""bar"": -2.42e+17,
                ""baz"": true
            }";

            Assert.That(regex.IsMatch(subject), Is.True);
            Assert.That(regex.IsMatch(subject.AsSpan()), Is.True);
        }
    }
}
