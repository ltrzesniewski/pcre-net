using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using PCRE.Internal;

namespace PCRE.Tests.PcreNet;

[TestFixture]
[SuppressMessage("ReSharper", "ArrangeDefaultValueWhenTypeNotEvident")]
[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
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
        Assert.That(match.ValueSpan.ToString(), Is.EqualTo("aaabbccc"));
        Assert.That(match.Index, Is.EqualTo(3));
        Assert.That(match.EndIndex, Is.EqualTo(11));
        Assert.That(match.Length, Is.EqualTo(8));

        Assert.That(match[1], Is.Not.Null);
        Assert.That(match[1].Success, Is.True);
        Assert.That(match[1].Value, Is.EqualTo("bb"));
        Assert.That(match[1].ValueSpan.ToString(), Is.EqualTo("bb"));
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
    public void should_match_pattern_buf()
    {
        var re = new PcreRegex(@"a+(b+)c+");
        var match = re.CreateMatchBuffer().Match("xxxaaabbccczzz".AsSpan());

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
        Assert.That(match.ValueSpan.ToString(), Is.EqualTo("aaabbddeee"));
        Assert.That(match.Index, Is.EqualTo(3));
        Assert.That(match.Length, Is.EqualTo(10));

        Assert.That(match[1], Is.Not.Null);
        Assert.That(match[1].Success, Is.True);
        Assert.That(match[1].IsDefined, Is.True);
        Assert.That(match[1].Value, Is.EqualTo("bb"));
        Assert.That(match[1].ValueSpan.ToString(), Is.EqualTo("bb"));
        Assert.That(match[1].Index, Is.EqualTo(6));
        Assert.That(match[1].Length, Is.EqualTo(2));

        Assert.That(match[2], Is.Not.Null);
        Assert.That(match[2].Success, Is.False);
        Assert.That(match[2].IsDefined, Is.True);
        Assert.That(match[2].Value, Is.SameAs(string.Empty));
        Assert.That(match[2].ValueSpan.Length, Is.EqualTo(0));
        Assert.That(match[2].Index, Is.EqualTo(-1));
        Assert.That(match[2].Length, Is.EqualTo(0));

        Assert.That(match[3], Is.Not.Null);
        Assert.That(match[3].Success, Is.True);
        Assert.That(match[3].IsDefined, Is.True);
        Assert.That(match[3].Value, Is.EqualTo("dd"));
        Assert.That(match[3].ValueSpan.ToString(), Is.EqualTo("dd"));
        Assert.That(match[3].Index, Is.EqualTo(8));
        Assert.That(match[3].Length, Is.EqualTo(2));

        Assert.That(match[4], Is.Not.Null);
        Assert.That(match[4].Success, Is.False);
        Assert.That(match[4].IsDefined, Is.False);
        Assert.That(match[4].Value, Is.SameAs(string.Empty));
        Assert.That(match[4].ValueSpan.Length, Is.EqualTo(0));
        Assert.That(match[4].Index, Is.EqualTo(-1));
        Assert.That(match[4].Length, Is.EqualTo(0));

        Assert.That(match.TryGetGroup(1, out var group), Is.True);
        Assert.That(group, Is.SameAs(match[1]));

        Assert.That(match.TryGetGroup(4, out group), Is.False);
        Assert.That(group, Is.Null);
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
        Assert.That(match[1].IsDefined, Is.True);
        Assert.That(match[1].Value.ToString(), Is.EqualTo("bb"));
        Assert.That(match[1].Index, Is.EqualTo(6));
        Assert.That(match[1].Length, Is.EqualTo(2));

        Assert.That(match[2].Success, Is.False);
        Assert.That(match[2].IsDefined, Is.True);
        Assert.That(match[2].Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match[2].Index, Is.EqualTo(-1));
        Assert.That(match[2].Length, Is.EqualTo(0));

        Assert.That(match[3].Success, Is.True);
        Assert.That(match[3].IsDefined, Is.True);
        Assert.That(match[3].Value.ToString(), Is.EqualTo("dd"));
        Assert.That(match[3].Index, Is.EqualTo(8));
        Assert.That(match[3].Length, Is.EqualTo(2));

        Assert.That(match[4].Success, Is.False);
        Assert.That(match[4].IsDefined, Is.False);
        Assert.That(match[4].Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match[4].Index, Is.EqualTo(-1));
        Assert.That(match[4].Length, Is.EqualTo(0));

        Assert.That(match.TryGetGroup(1, out var group), Is.True);
        Assert.That(group.Success, Is.True);
        Assert.That(group.IsDefined, Is.True);
        Assert.That(group.Value.ToString(), Is.EqualTo("bb"));
        Assert.That(group.Index, Is.EqualTo(6));
        Assert.That(group.Length, Is.EqualTo(2));

        Assert.That(match.TryGetGroup(4, out group), Is.False);
        Assert.That(group.Success, Is.False);
        Assert.That(group.IsDefined, Is.False);
        Assert.That(group.Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(group.Index, Is.EqualTo(-1));
        Assert.That(group.Length, Is.EqualTo(0));
    }

    [Test]
    public void should_support_multiple_groups_buf()
    {
        var re = new PcreRegex(@"a+(b+)(c+)?(d+)e+");
        var match = re.CreateMatchBuffer().Match("xxxaaabbddeeezzz".AsSpan());

        Assert.That(match.Success, Is.True);
        Assert.That(match.CaptureCount, Is.EqualTo(3));
        Assert.That(match.Value.ToString(), Is.EqualTo("aaabbddeee"));
        Assert.That(match.Index, Is.EqualTo(3));
        Assert.That(match.Length, Is.EqualTo(10));

        Assert.That(match[1].Success, Is.True);
        Assert.That(match[1].IsDefined, Is.True);
        Assert.That(match[1].Value.ToString(), Is.EqualTo("bb"));
        Assert.That(match[1].Index, Is.EqualTo(6));
        Assert.That(match[1].Length, Is.EqualTo(2));

        Assert.That(match[2].Success, Is.False);
        Assert.That(match[2].IsDefined, Is.True);
        Assert.That(match[2].Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match[2].Index, Is.EqualTo(-1));
        Assert.That(match[2].Length, Is.EqualTo(0));

        Assert.That(match[3].Success, Is.True);
        Assert.That(match[3].IsDefined, Is.True);
        Assert.That(match[3].Value.ToString(), Is.EqualTo("dd"));
        Assert.That(match[3].Index, Is.EqualTo(8));
        Assert.That(match[3].Length, Is.EqualTo(2));

        Assert.That(match[4].Success, Is.False);
        Assert.That(match[4].IsDefined, Is.False);
        Assert.That(match[4].Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match[4].Index, Is.EqualTo(-1));
        Assert.That(match[4].Length, Is.EqualTo(0));

        Assert.That(match.TryGetGroup(1, out var group), Is.True);
        Assert.That(group.Success, Is.True);
        Assert.That(group.IsDefined, Is.True);
        Assert.That(group.Value.ToString(), Is.EqualTo("bb"));
        Assert.That(group.Index, Is.EqualTo(6));
        Assert.That(group.Length, Is.EqualTo(2));

        Assert.That(match.TryGetGroup(4, out group), Is.False);
        Assert.That(group.Success, Is.False);
        Assert.That(group.IsDefined, Is.False);
        Assert.That(group.Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(group.Index, Is.EqualTo(-1));
        Assert.That(group.Length, Is.EqualTo(0));
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
        Assert.That(match[1].ValueSpan.ToString(), Is.EqualTo("a"));

        Assert.That(match[2].Success, Is.False);
        Assert.That(match[2].Index, Is.EqualTo(-1));
        Assert.That(match[2].EndIndex, Is.EqualTo(-1));
        Assert.That(match[2].Value, Is.SameAs(string.Empty));
        Assert.That(match[2].ValueSpan.Length, Is.EqualTo(0));

        Assert.That(match[3].Success, Is.True);
        Assert.That(match[3].Index, Is.EqualTo(1));
        Assert.That(match[3].Length, Is.EqualTo(2));
        Assert.That(match[3].Value, Is.EqualTo("bc"));
        Assert.That(match[3].ValueSpan.ToString(), Is.EqualTo("bc"));
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
    public void should_support_unmatched_groups_before_matched_groups_buf()
    {
        var re = new PcreRegex(@"(a|(z))(bc)");
        var match = re.CreateMatchBuffer().Match("abc".AsSpan());

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
    public void should_match_starting_at_end_of_string_buf()
    {
        var re = new PcreRegex(@"(?<=a)");
        var match = re.CreateMatchBuffer().Match("xxa".AsSpan(), 3);

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
        Assert.That(match.ValueSpan.ToString(), Is.EqualTo("aaabbcccddeee"));
        Assert.That(match.Index, Is.EqualTo(3));
        Assert.That(match.Length, Is.EqualTo(13));

        Assert.That(match["bees"], Is.Not.Null);
        Assert.That(match["bees"].Success, Is.True);
        Assert.That(match["bees"].IsDefined, Is.True);
        Assert.That(match["bees"].Value, Is.EqualTo("bb"));
        Assert.That(match["bees"].ValueSpan.ToString(), Is.EqualTo("bb"));
        Assert.That(match["bees"].Index, Is.EqualTo(6));
        Assert.That(match["bees"].Length, Is.EqualTo(2));

        Assert.That(match.Groups["bees"], Is.SameAs(match["bees"]));

        Assert.That(match[2], Is.Not.Null);
        Assert.That(match[2].Value, Is.EqualTo("ccc"));
        Assert.That(match[2].ValueSpan.ToString(), Is.EqualTo("ccc"));
        Assert.That(match[2].Index, Is.EqualTo(8));
        Assert.That(match[2].Length, Is.EqualTo(3));

        Assert.That(match["dees"], Is.Not.Null);
        Assert.That(match["dees"].Success, Is.True);
        Assert.That(match["dees"].IsDefined, Is.True);
        Assert.That(match["dees"].Value, Is.EqualTo("dd"));
        Assert.That(match["dees"].ValueSpan.ToString(), Is.EqualTo("dd"));
        Assert.That(match["dees"].Index, Is.EqualTo(11));
        Assert.That(match["dees"].Length, Is.EqualTo(2));

        Assert.That(match["nope"], Is.Not.Null);
        Assert.That(match["nope"].Success, Is.False);
        Assert.That(match["nope"].IsDefined, Is.False);
        Assert.That(match["nope"].Value, Is.SameAs(string.Empty));
        Assert.That(match["nope"].ValueSpan.Length, Is.EqualTo(0));
        Assert.That(match["nope"].Index, Is.EqualTo(-1));
        Assert.That(match["nope"].Length, Is.EqualTo(0));

        Assert.That(match.TryGetGroup("bees", out var group), Is.True);
        Assert.That(group, Is.SameAs(match["bees"]));

        Assert.That(match.TryGetGroup("nope", out group), Is.False);
        Assert.That(group, Is.Null);
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

        Assert.That(match["bees"].Success, Is.True);
        Assert.That(match["bees"].IsDefined, Is.True);
        Assert.That(match["bees"].Value.ToString(), Is.EqualTo("bb"));
        Assert.That(match["bees"].Index, Is.EqualTo(6));
        Assert.That(match["bees"].Length, Is.EqualTo(2));

        Assert.That(match[2].Value.ToString(), Is.EqualTo("ccc"));
        Assert.That(match[2].Index, Is.EqualTo(8));
        Assert.That(match[2].Length, Is.EqualTo(3));

        Assert.That(match["dees"].Success, Is.True);
        Assert.That(match["dees"].IsDefined, Is.True);
        Assert.That(match["dees"].Value.ToString(), Is.EqualTo("dd"));
        Assert.That(match["dees"].Index, Is.EqualTo(11));
        Assert.That(match["dees"].Length, Is.EqualTo(2));

        Assert.That(match["nope"].Success, Is.False);
        Assert.That(match["nope"].IsDefined, Is.False);
        Assert.That(match["nope"].Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match["nope"].Index, Is.EqualTo(-1));
        Assert.That(match["nope"].Length, Is.EqualTo(0));

        Assert.That(match.TryGetGroup("bees", out var group), Is.True);
        Assert.That(group.Success, Is.True);
        Assert.That(group.IsDefined, Is.True);
        Assert.That(group.Value.ToString(), Is.EqualTo("bb"));
        Assert.That(group.Index, Is.EqualTo(6));
        Assert.That(group.Length, Is.EqualTo(2));

        Assert.That(match.TryGetGroup("nope", out group), Is.False);
        Assert.That(group.Success, Is.False);
        Assert.That(group.IsDefined, Is.False);
        Assert.That(group.Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(group.Index, Is.EqualTo(-1));
        Assert.That(group.Length, Is.EqualTo(0));
    }

    [Test]
    public void should_handle_named_groups_buf()
    {
        var re = new PcreRegex(@"a+(?<bees>b+)(c+)(?<dees>d+)e+");

        var match = re.CreateMatchBuffer().Match("xxxaaabbcccddeeezzz".AsSpan());

        Assert.That(match.Success, Is.True);
        Assert.That(match.CaptureCount, Is.EqualTo(3));
        Assert.That(match.Value.ToString(), Is.EqualTo("aaabbcccddeee"));
        Assert.That(match.Index, Is.EqualTo(3));
        Assert.That(match.Length, Is.EqualTo(13));

        Assert.That(match["bees"].Success, Is.True);
        Assert.That(match["bees"].IsDefined, Is.True);
        Assert.That(match["bees"].Value.ToString(), Is.EqualTo("bb"));
        Assert.That(match["bees"].Index, Is.EqualTo(6));
        Assert.That(match["bees"].Length, Is.EqualTo(2));

        Assert.That(match[2].Value.ToString(), Is.EqualTo("ccc"));
        Assert.That(match[2].Index, Is.EqualTo(8));
        Assert.That(match[2].Length, Is.EqualTo(3));

        Assert.That(match["dees"].Success, Is.True);
        Assert.That(match["dees"].IsDefined, Is.True);
        Assert.That(match["dees"].Value.ToString(), Is.EqualTo("dd"));
        Assert.That(match["dees"].Index, Is.EqualTo(11));
        Assert.That(match["dees"].Length, Is.EqualTo(2));

        Assert.That(match["nope"].Success, Is.False);
        Assert.That(match["nope"].IsDefined, Is.False);
        Assert.That(match["nope"].Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match["nope"].Index, Is.EqualTo(-1));
        Assert.That(match["nope"].Length, Is.EqualTo(0));

        Assert.That(match.TryGetGroup("bees", out var group), Is.True);
        Assert.That(group.Success, Is.True);
        Assert.That(group.IsDefined, Is.True);
        Assert.That(group.Value.ToString(), Is.EqualTo("bb"));
        Assert.That(group.Index, Is.EqualTo(6));
        Assert.That(group.Length, Is.EqualTo(2));

        Assert.That(match.TryGetGroup("nope", out group), Is.False);
        Assert.That(group.Success, Is.False);
        Assert.That(group.IsDefined, Is.False);
        Assert.That(group.Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(group.Index, Is.EqualTo(-1));
        Assert.That(group.Length, Is.EqualTo(0));
    }

    [Test]
    public void should_handle_case_sensitive_group_names()
    {
        var re = new PcreRegex(@"a+(?<grp>b+)(?<GRP>c+)(?<GrP>d+)e+");

        var match = re.Match("xxxaaabbcccddeeezzz");

        Assert.That(match["grp"], Is.Not.Null);
        Assert.That(match["grp"].Value, Is.EqualTo("bb"));
        Assert.That(match["grp"].ValueSpan.ToString(), Is.EqualTo("bb"));
        Assert.That(match["grp"].Index, Is.EqualTo(6));
        Assert.That(match["grp"].Length, Is.EqualTo(2));

        Assert.That(match["GRP"], Is.Not.Null);
        Assert.That(match["GRP"].Value, Is.EqualTo("ccc"));
        Assert.That(match["GRP"].ValueSpan.ToString(), Is.EqualTo("ccc"));
        Assert.That(match["GRP"].Index, Is.EqualTo(8));
        Assert.That(match["GRP"].Length, Is.EqualTo(3));

        Assert.That(match["GrP"], Is.Not.Null);
        Assert.That(match["GrP"].Value, Is.EqualTo("dd"));
        Assert.That(match["GrP"].ValueSpan.ToString(), Is.EqualTo("dd"));
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
    public void should_handle_case_sensitive_group_names_buf()
    {
        var re = new PcreRegex(@"a+(?<grp>b+)(?<GRP>c+)(?<GrP>d+)e+");

        var match = re.CreateMatchBuffer().Match("xxxaaabbcccddeeezzz".AsSpan());

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
        Assert.That(match["g"].ValueSpan.ToString(), Is.EqualTo("b"));

        Assert.That(match.GetDuplicateNamedGroups("g").Select(g => g.Success), Is.EqualTo([false, true, false]));

        match = re.Match("bc");
        Assert.That(match, Is.Not.Null);
        Assert.That(match.Success, Is.True);
        Assert.That(match["g"].Value, Is.EqualTo("b"));
        Assert.That(match["g"].ValueSpan.ToString(), Is.EqualTo("b"));

        Assert.That(match.GetDuplicateNamedGroups("g").Select(g => g.Success), Is.EqualTo([false, true, true]));
    }

    [Test]
    public void should_allow_duplicate_names_ref()
    {
        var re = new PcreRegex(@"(?<g>a)?(?<g>b)?(?<g>c)?", PcreOptions.DupNames);
        var match = re.Match("b".AsSpan());

        Assert.That(match.Success, Is.True);
        Assert.That(match["g"].Value.ToString(), Is.EqualTo("b"));

        Assert.That(GetDuplicateNamedGroupsSuccesses(match, "g"), Is.EqualTo([false, true, false]));

        match = re.Match("bc".AsSpan());
        Assert.That(match.Success, Is.True);
        Assert.That(match["g"].Value.ToString(), Is.EqualTo("b"));

        Assert.That(GetDuplicateNamedGroupsSuccesses(match, "g"), Is.EqualTo([false, true, true]));
    }

    [Test]
    public void should_allow_duplicate_names_buf()
    {
        var re = new PcreRegex(@"(?<g>a)?(?<g>b)?(?<g>c)?", PcreOptions.DupNames);
        var match = re.CreateMatchBuffer().Match("b".AsSpan());

        Assert.That(match.Success, Is.True);
        Assert.That(match["g"].Value.ToString(), Is.EqualTo("b"));

        Assert.That(GetDuplicateNamedGroupsSuccesses(match, "g"), Is.EqualTo([false, true, false]));

        match = re.Match("bc".AsSpan());
        Assert.That(match.Success, Is.True);
        Assert.That(match["g"].Value.ToString(), Is.EqualTo("b"));

        Assert.That(GetDuplicateNamedGroupsSuccesses(match, "g"), Is.EqualTo([false, true, true]));
    }

    [Test]
    public void should_detect_duplicate_names()
    {
        var re = new PcreRegex(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?");

        var match = re.Match("bc");
        Assert.That(match, Is.Not.Null);
        Assert.That(match.Success, Is.True);
        Assert.That(match["g"].Value, Is.EqualTo("b"));

        Assert.That(match.GetDuplicateNamedGroups("g").Select(g => g.Success), Is.EqualTo([false, true, true]));
    }

    [Test]
    public void should_detect_duplicate_names_ref()
    {
        var re = new PcreRegex(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?");

        var match = re.Match("bc".AsSpan());
        Assert.That(match.Success, Is.True);
        Assert.That(match["g"].Value.ToString(), Is.EqualTo("b"));

        Assert.That(GetDuplicateNamedGroupsSuccesses(match, "g"), Is.EqualTo([false, true, true]));
    }

    [Test]
    public void should_detect_duplicate_names_buf()
    {
        var re = new PcreRegex(@"(?J)(?<g>a)?(?<g>b)?(?<g>c)?");

        var match = re.CreateMatchBuffer().Match("bc".AsSpan());
        Assert.That(match.Success, Is.True);
        Assert.That(match["g"].Value.ToString(), Is.EqualTo("b"));

        Assert.That(GetDuplicateNamedGroupsSuccesses(match, "g"), Is.EqualTo([false, true, true]));
    }

    private static List<bool> GetDuplicateNamedGroupsSuccesses(PcreRefMatch match, string groupName)
        => match.GetDuplicateNamedGroups(groupName).ToList(i => i.Success);

    [Test]
    public void should_return_value_span_from_subject_string()
    {
        var subject = string.Concat("foo", "bar");
        var re = new PcreRegex(@"b(a)(r)");

        var match = re.Match(subject);

        ref var subjectRef = ref MemoryMarshal.GetReference(subject.AsSpan(3));
        ref var valueRef = ref MemoryMarshal.GetReference(match.ValueSpan);
        Assert.That(Unsafe.AreSame(ref valueRef, ref subjectRef), Is.True);

        _ = match.Value; // Reading the string value shouldn't change the span target

        valueRef = ref MemoryMarshal.GetReference(match.ValueSpan);
        Assert.That(Unsafe.AreSame(ref valueRef, ref subjectRef), Is.True);
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
    public void should_return_marks_buf()
    {
        var re = new PcreRegex(@"a(?:(*MARK:foo)b(*MARK:bar)|c)");
        var match = re.CreateMatchBuffer().Match("ab".AsSpan());

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
    public void should_use_callout_result_buf()
    {
        var regex = new PcreRegex(@"(\d+)(*SKIP)(?C1):\s*(\w+)");

        var match = regex.CreateMatchBuffer().Match(
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
    public void should_execute_passing_callout_buf()
    {
        const string pattern = @"(a)(*MARK:foo)(x)?(?C42)(bc)";
        var re = new PcreRegex(pattern);

        var calls = 0;

        var match = re.CreateMatchBuffer().Match("abc".AsSpan(), data =>
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
    public void should_execute_failing_callout_buf()
    {
        var re = new PcreRegex(@".(?C42)");

        var first = true;

        var match = re.CreateMatchBuffer().Match("ab".AsSpan(), data =>
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
    public void should_execute_aborting_callout_buf()
    {
        var re = new PcreRegex(@".(?C42)");

        var match = re.CreateMatchBuffer().Match("ab".AsSpan(), data =>
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

        var ex = Assert.Throws<PcreCalloutException>(() => re.Match("ab", _ => throw new DivideByZeroException("test")))!;

        Assert.That(ex.ErrorCode, Is.EqualTo(PcreErrorCode.Callout));
        Assert.That(ex.InnerException, Is.InstanceOf<DivideByZeroException>());
    }

    [Test]
    public void should_throw_when_callout_throws_ref()
    {
        var re = new PcreRegex(@".(?C42)");

        var ex = Assert.Throws<PcreCalloutException>(() => re.Match("ab".AsSpan(), _ => throw new DivideByZeroException("test")))!;

        Assert.That(ex.ErrorCode, Is.EqualTo(PcreErrorCode.Callout));
        Assert.That(ex.InnerException, Is.InstanceOf<DivideByZeroException>());
    }

    [Test]
    public void should_throw_when_callout_throws_buf()
    {
        var re = new PcreRegex(@".(?C42)");

        var buffer = re.CreateMatchBuffer();
        var ex = Assert.Throws<PcreCalloutException>(() => buffer.Match("ab".AsSpan(), _ => throw new DivideByZeroException("test")))!;

        Assert.That(ex.ErrorCode, Is.EqualTo(PcreErrorCode.Callout));
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
    public void should_auto_callout_buf()
    {
        var re = new PcreRegex(@"a.c", PcreOptions.AutoCallout);

        var count = 0;

        var match = re.CreateMatchBuffer().Match("abc".AsSpan(), data =>
        {
            Assert.That(data.Number, Is.EqualTo(255));
            ++count;
            return PcreCalloutResult.Pass;
        });

        Assert.That(match.Success, Is.True);
        Assert.That(count, Is.EqualTo(4));
    }

    [Test]
    public void should_get_info_for_auto_callouts()
    {
        var re = new PcreRegex(@"a.c", PcreOptions.AutoCallout);

        var count = 0;

        var match = re.Match("abc", data =>
        {
            ++count;
            Assert.That(data.Info, Is.Not.Null);
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

        Assert.That(startMatchList, Is.EqualTo([true, false, false]));
        Assert.That(backtrackList, Is.EqualTo([false, false, true]));
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

        Assert.That(startMatchList, Is.EqualTo([true, false, false]));
        Assert.That(backtrackList, Is.EqualTo([false, false, true]));
    }

    [Test]
    public void should_provide_callout_flags_buf()
    {
        var re = new PcreRegex(@"a(?C1)(?:(?C2)(*FAIL)|b)(?C3)");

        var startMatchList = new List<bool>();
        var backtrackList = new List<bool>();

        re.CreateMatchBuffer().Match("abc".AsSpan(), data =>
        {
            startMatchList.Add(data.StartMatch);
            backtrackList.Add(data.Backtrack);
            return PcreCalloutResult.Pass;
        });

        Assert.That(startMatchList, Is.EqualTo([true, false, false]));
        Assert.That(backtrackList, Is.EqualTo([false, false, true]));
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
    public void should_execute_string_callout_buf()
    {
        const string pattern = @"(a)(*MARK:foo)(x)?(?C{bar})(bc)";
        var re = new PcreRegex(pattern);

        var calls = 0;

        var match = re.CreateMatchBuffer().Match("abc".AsSpan(), data =>
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
    public void should_support_unmatched_groups_before_matched_groups_in_callout_buf()
    {
        var re = new PcreRegex(@"(a|(z))(bc)(?C42)");
        var calls = 0;

        re.CreateMatchBuffer().Match("abc".AsSpan(), data =>
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
    public void should_handle_callouts_with_many_captures_ref()
    {
        var sb = new StringBuilder();
        const int length = InternalRegex.MaxStackAllocCaptureCount * 2;

        for (var i = 0; i < length; ++i)
            sb.Append("(a)");

        sb.Append("(?C)");

        var re = new PcreRegex(sb.ToString());
        var calls = 0;
        var subject = new string('a', length);

        re.Match(subject.AsSpan(), data =>
        {
            ++calls;
            Assert.That(data.Match.Length, Is.EqualTo(length));
            Assert.That(data.Match.Groups[1].Value.ToString(), Is.EqualTo("a"));
            Assert.That(data.Match.Groups[length].Value.ToString(), Is.EqualTo("a"));
            return PcreCalloutResult.Pass;
        });

        Assert.That(calls, Is.EqualTo(1));
    }

    [Test]
    public void should_handle_callouts_with_many_captures_buf()
    {
        var sb = new StringBuilder();
        const int length = InternalRegex.MaxStackAllocCaptureCount * 2;

        for (var i = 0; i < length; ++i)
            sb.Append("(a)");

        sb.Append("(?C)");

        var re = new PcreRegex(sb.ToString());
        var calls = 0;
        var subject = new string('a', length);

        re.CreateMatchBuffer().Match(subject.AsSpan(), data =>
        {
            ++calls;
            Assert.That(data.Match.Length, Is.EqualTo(length));
            Assert.That(data.Match.Groups[1].Value.ToString(), Is.EqualTo("a"));
            Assert.That(data.Match.Groups[length].Value.ToString(), Is.EqualTo("a"));
            return PcreCalloutResult.Pass;
        });

        Assert.That(calls, Is.EqualTo(1));
    }

    [Test]
    public void should_handle_end_before_start()
    {
        var re = new PcreRegex(@"(?=a+\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });

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
        var re = new PcreRegex(@"(?=a+\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });

        var match = re.Match("aaa".AsSpan());

        Assert.That(match.Success, Is.True);
        Assert.That(match.Index, Is.EqualTo(3));
        Assert.That(match.EndIndex, Is.EqualTo(0));
        Assert.That(match.Length, Is.EqualTo(0));
        Assert.That(match.Value.ToString(), Is.EqualTo(string.Empty));
    }

    [Test]
    public void should_handle_end_before_start_buf()
    {
        var re = new PcreRegex(@"(?=a+\K)", new PcreRegexSettings { ExtraCompileOptions = PcreExtraCompileOptions.AllowLookaroundBsK });

        var match = re.CreateMatchBuffer().Match("aaa".AsSpan());

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
    public void should_handle_additional_options_buf()
    {
        var re = new PcreRegex(@"bar");
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("foobar".AsSpan(), PcreMatchOptions.None);

        Assert.That(match.Success, Is.True);

        match = buffer.Match("foobar".AsSpan(), PcreMatchOptions.Anchored);

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
    public void should_handle_extra_options_buf()
    {
        var re = new PcreRegex(@"bar", new PcreRegexSettings(PcreOptions.Literal)
        {
            ExtraCompileOptions = PcreExtraCompileOptions.MatchWord
        });

        var buffer = re.CreateMatchBuffer();

        Assert.That(buffer.IsMatch("foo bar baz".AsSpan()), Is.True);
        Assert.That(buffer.IsMatch("foobar baz".AsSpan()), Is.False);
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
    [TestCase(PcreMatchOptions.PartialSoft)]
    [TestCase(PcreMatchOptions.PartialHard)]
    public void should_match_partially_buf(PcreMatchOptions options)
    {
        var re = new PcreRegex(@"(?<=abc)123");

        var match = re.CreateMatchBuffer().Match("xyzabc12".AsSpan(), options);

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
    public void should_differentiate_soft_and_hard_partial_matching_buf()
    {
        var re = new PcreRegex(@"dog(sbody)?");
        var buffer = re.CreateMatchBuffer();

        var softMatch = buffer.Match("dog".AsSpan(), PcreMatchOptions.PartialSoft);

        Assert.That(softMatch.Success, Is.True);
        Assert.That(softMatch.IsPartialMatch, Is.False);

        var hardMatch = buffer.Match("dog".AsSpan(), PcreMatchOptions.PartialHard);

        Assert.That(hardMatch.Success, Is.False);
        Assert.That(hardMatch.IsPartialMatch, Is.True);
    }

    [Test]
    public void should_check_pattern_utf_validity()
    {
        var ex = Assert.Throws<PcrePatternException>(() => _ = new PcreRegex("A\uD800B"))!;
        Assert.That(ex.ErrorCode, Is.EqualTo(PcreErrorCode.Utf16Err2));
        Assert.That(ex.Message, Contains.Substring("invalid low surrogate"));
    }

    [Test]
    public void should_check_subject_utf_validity()
    {
        var re = new PcreRegex(@"A");
        var ex = Assert.Throws<PcreMatchException>(() => _ = re.Match("A\uD800B"))!;
        Assert.That(ex.ErrorCode, Is.EqualTo(PcreErrorCode.Utf16Err2));
        Assert.That(ex.Message, Contains.Substring("invalid low surrogate"));
    }

    [Test]
    public void should_check_subject_utf_validity_ref()
    {
        var re = new PcreRegex(@"A");
        var ex = Assert.Throws<PcreMatchException>(() => _ = re.Match("A\uD800B".AsSpan()))!;
        Assert.That(ex.ErrorCode, Is.EqualTo(PcreErrorCode.Utf16Err2));
        Assert.That(ex.Message, Contains.Substring("invalid low surrogate"));
    }

    [Test]
    public void should_check_subject_utf_validity_buf()
    {
        var re = new PcreRegex(@"A");
        var buffer = re.CreateMatchBuffer();

        var ex = Assert.Throws<PcreMatchException>(() => _ = buffer.Match("A\uD800B".AsSpan()))!;
        Assert.That(ex.ErrorCode, Is.EqualTo(PcreErrorCode.Utf16Err2));
        Assert.That(ex.Message, Contains.Substring("invalid low surrogate"));
    }

    [Test]
    public void should_handle_offset_limit()
    {
        var re = new PcreRegex(@"bar", PcreOptions.UseOffsetLimit);

        var match = re.Match("foobar");
        Assert.That(match.Success, Is.True);

        match = re.Match("foobar", 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        });
        Assert.That(match.Success, Is.True);

        match = re.Match("foobar", 0, PcreMatchOptions.None, null, new PcreMatchSettings
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

        match = re.Match("foobar".AsSpan(), 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        });
        Assert.That(match.Success, Is.True);

        match = re.Match("foobar".AsSpan(), 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 2
        });
        Assert.That(match.Success, Is.False);
    }

    [Test]
    public void should_handle_offset_limit_buf()
    {
        var re = new PcreRegex(@"bar", PcreOptions.UseOffsetLimit);

        var buffer = re.CreateMatchBuffer();
        var match = buffer.Match("foobar".AsSpan());
        Assert.That(match.Success, Is.True);

        buffer = re.CreateMatchBuffer(new PcreMatchSettings { OffsetLimit = 3 });
        match = buffer.Match("foobar".AsSpan());
        Assert.That(match.Success, Is.True);

        buffer = re.CreateMatchBuffer(new PcreMatchSettings { OffsetLimit = 2 });
        match = buffer.Match("foobar".AsSpan());
        Assert.That(match.Success, Is.False);
    }

    [Test]
    public void should_detect_invalid_offset_limit_usage()
    {
        var re = new PcreRegex(@"bar");

        var ex = Assert.Throws<PcreMatchException>(() => re.Match("foobar", 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        }))!;

        Assert.That(ex.ErrorCode, Is.EqualTo(PcreErrorCode.BadOffsetLimit));
    }

    [Test]
    public void should_detect_invalid_offset_limit_usage_ref()
    {
        var re = new PcreRegex(@"bar");

        var ex = Assert.Throws<PcreMatchException>(() => re.Match("foobar".AsSpan(), 0, PcreMatchOptions.None, null, new PcreMatchSettings
        {
            OffsetLimit = 3
        }))!;

        Assert.That(ex.ErrorCode, Is.EqualTo(PcreErrorCode.BadOffsetLimit));
    }

    [Test]
    public void should_detect_invalid_offset_limit_usage_buf()
    {
        var re = new PcreRegex(@"bar");
        var buffer = re.CreateMatchBuffer(new PcreMatchSettings
        {
            OffsetLimit = 3
        });

        var ex = Assert.Throws<PcreMatchException>(() => buffer.Match("foobar".AsSpan()))!;
        Assert.That(ex.ErrorCode, Is.EqualTo(PcreErrorCode.BadOffsetLimit));
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
    public void should_match_script_run_buf()
    {
        const string subject = "123\U0001D7CF\U0001D7D0\U0001D7D1";

        var normal = new PcreRegex(@"\d+", PcreOptions.Unicode).CreateMatchBuffer().Match(subject.AsSpan());
        Assert.That(normal.Success, Is.True);
        Assert.That(normal.Index, Is.EqualTo(0));
        Assert.That(normal.Length, Is.EqualTo(subject.Length));

        var scriptRun = new PcreRegex(@"(*script_run:\d+)", PcreOptions.Unicode).CreateMatchBuffer().Match(subject.AsSpan());
        Assert.That(scriptRun.Success, Is.True);
        Assert.That(scriptRun.Index, Is.EqualTo(0));
        Assert.That(scriptRun.Length, Is.EqualTo(3));
    }

    [Test]
    public void should_match_empty_string()
    {
        var re = new PcreRegex(string.Empty);
        var match = re.Match(string.Empty);

        Assert.That(match, Is.Not.Null);
        Assert.That(match.Success, Is.True);
        Assert.That(match.CaptureCount, Is.EqualTo(0));
        Assert.That(match.Value, Is.EqualTo(string.Empty));
        Assert.That(match.Index, Is.EqualTo(0));
        Assert.That(match.EndIndex, Is.EqualTo(0));
        Assert.That(match.Length, Is.EqualTo(0));

        Assert.That(match[0], Is.Not.Null);
        Assert.That(match[0].Success, Is.True);
        Assert.That(match[0].IsDefined, Is.True);
        Assert.That(match[0].Value, Is.EqualTo(string.Empty));
        Assert.That(match[0].Index, Is.EqualTo(0));
        Assert.That(match[0].EndIndex, Is.EqualTo(0));
        Assert.That(match[0].Length, Is.EqualTo(0));
    }

    [Test]
    public void should_match_empty_string_ref()
    {
        var re = new PcreRegex(string.Empty);
        var match = re.Match(default(ReadOnlySpan<char>));

        Assert.That(match.Success, Is.True);
        Assert.That(match.CaptureCount, Is.EqualTo(0));
        Assert.That(match.Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match.Index, Is.EqualTo(0));
        Assert.That(match.EndIndex, Is.EqualTo(0));
        Assert.That(match.Length, Is.EqualTo(0));

        Assert.That(match[0].Success, Is.True);
        Assert.That(match[0].IsDefined, Is.True);
        Assert.That(match[0].Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match[0].Index, Is.EqualTo(0));
        Assert.That(match[0].EndIndex, Is.EqualTo(0));
        Assert.That(match[0].Length, Is.EqualTo(0));
    }

    [Test]
    public void should_match_empty_string_buf()
    {
        var re = new PcreRegex(string.Empty);
        var match = re.CreateMatchBuffer().Match(default(ReadOnlySpan<char>));

        Assert.That(match.Success, Is.True);
        Assert.That(match.CaptureCount, Is.EqualTo(0));
        Assert.That(match.Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match.Index, Is.EqualTo(0));
        Assert.That(match.EndIndex, Is.EqualTo(0));
        Assert.That(match.Length, Is.EqualTo(0));

        Assert.That(match[0].Success, Is.True);
        Assert.That(match[0].IsDefined, Is.True);
        Assert.That(match[0].Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(match[0].Index, Is.EqualTo(0));
        Assert.That(match[0].EndIndex, Is.EqualTo(0));
        Assert.That(match[0].Length, Is.EqualTo(0));
    }

    [Test]
    public void should_have_undefined_value_in_default_ref_group()
    {
        var group = default(PcreRefGroup);

        Assert.That(group.Success, Is.False);
        Assert.That(group.IsDefined, Is.False);
        Assert.That(group.Value.ToString(), Is.SameAs(string.Empty));
        Assert.That(group.Index, Is.EqualTo(-1));
        Assert.That(group.EndIndex, Is.EqualTo(-1));
        Assert.That(group.Length, Is.EqualTo(0));
    }

    [Test]
    public void should_return_singleton_for_no_match()
    {
        var re = new PcreRegex("foo");
        var matchA = re.Match("bar");
        var matchB = re.Match("baz");

        Assert.That(matchB, Is.SameAs(matchA));
    }

    [Test]
    public void should_not_allocate_output_vector_for_no_match_ref()
    {
        var re = new PcreRegex("foo");
        var match = re.Match("bar".AsSpan());

        Assert.That(match.OutputVector.Length, Is.Zero);
    }

    [Test]
    public unsafe void should_use_buffer_output_vector_for_no_match()
    {
        var re = new PcreRegex("foo");
        var buffer = re.CreateMatchBuffer();

        var match = buffer.Match("bar".AsSpan());

        Assert.That(Unsafe.AreSame(ref MemoryMarshal.GetReference(match.OutputVector), ref buffer.OutputVector[0]), Is.True);
    }

    [Test]
    public void should_fix_issue_22()
    {
        var regex = new PcreRegex(@"[\w]*[CA]X*B", PcreOptions.Compiled);
        Assert.That(regex.IsMatch("ABC"), Is.True);
    }

    [Test]
    public void should_fix_pcre_issue_21()
    {
        var regex = new PcreRegex(@"(?P<size>\\d+)m|M", PcreOptions.Compiled);
        Assert.That(regex.Match("4M").Value, Is.EqualTo("M"));
    }

    [Test]
    public void should_throw_on_null_subject()
    {
        var re = new PcreRegex("a");
        Assert.Throws<ArgumentNullException>(() => re.Match(default(string)!));
    }

    [Test]
    public void should_throw_on_null_settings()
    {
        var re = new PcreRegex("a");
        Assert.Throws<ArgumentNullException>(() => re.Match("a", 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
    }

    [Test]
    public void should_throw_on_null_settings_ref()
    {
        var re = new PcreRegex("a");
        Assert.Throws<ArgumentNullException>(() => re.Match("a".AsSpan(), 0, PcreMatchOptions.None, null, default(PcreMatchSettings)!));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index(int startIndex)
    {
        var re = new PcreRegex(@"a");
        Assert.Throws<ArgumentOutOfRangeException>(() => re.Match("a", startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_ref(int startIndex)
    {
        var re = new PcreRegex(@"a");
        Assert.Throws<ArgumentOutOfRangeException>(() => re.Match("a".AsSpan(), startIndex));
    }

    [Test]
    [TestCase(-1)]
    [TestCase(2)]
    public void should_throw_on_invalid_start_index_buf(int startIndex)
    {
        var re = new PcreRegex(@"a");
        var buffer = re.CreateMatchBuffer();
        Assert.Throws<ArgumentOutOfRangeException>(() => buffer.Match("a".AsSpan(), startIndex));
    }

    [Test]
    public void should_return_matched_string()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab");

        Assert.That(match.ToString(), Is.EqualTo("a"));
    }

    [Test]
    public void should_return_matched_string_from_group()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab");

        Assert.That(match[0].ToString(), Is.EqualTo("a"));
    }

    [Test]
    public void should_cast_group_to_string()
    {
        var re = new PcreRegex(".");
        var match = re.Match("ab");

        Assert.That((string)match[0], Is.EqualTo("a"));
    }

    [Test]
    [TestCase(new PcreOptimizationDirective[0], true)]
    [TestCase(new[] { PcreOptimizationDirective.None }, false)]
    [TestCase(new[] { PcreOptimizationDirective.Full }, true)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess }, true)]
    [TestCase(new[] { PcreOptimizationDirective.None, PcreOptimizationDirective.AutoPossess }, true)]
    [TestCase(new[] { PcreOptimizationDirective.Full, PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess, PcreOptimizationDirective.None }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff, PcreOptimizationDirective.Full }, true)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossess, PcreOptimizationDirective.AutoPossessOff }, false)]
    [TestCase(new[] { PcreOptimizationDirective.AutoPossessOff, PcreOptimizationDirective.AutoPossess }, true)]
    public void should_use_optimization_settings(PcreOptimizationDirective[] directives, bool expectedAutoPossess)
    {
        var settings = new PcreRegexSettings(PcreOptions.AutoCallout | PcreOptions.NoStartOptimize);

        foreach (var directive in directives)
            settings.OptimizationDirectives.Add(directive);

        var re = new PcreRegex("^a+b", settings);

        var calloutCount = 0;
        re.Match("aac", _ =>
        {
            ++calloutCount;
            return PcreCalloutResult.Pass;
        });

        var autoPossess = calloutCount switch
        {
            3 => true,
            4 => false,
            _ => throw new InvalidOperationException($"Unexpected callout count: {calloutCount}.")
        };

        Assert.That(autoPossess, Is.EqualTo(expectedAutoPossess));
    }

    [Test]
    public void readme_json_example()
    {
        const string jsonPattern =
            """
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
                    " (?: [^"\\\p{Cc}]++ | \\u[0-9A-Fa-f]{4} | \\ ["\\/bfnrt] )* "
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
            """;

        var regex = new PcreRegex(jsonPattern, PcreOptions.IgnorePatternWhitespace | PcreOptions.Compiled);

        //language=json
        const string subject =
            """
            {
                "hello": "world",
                "numbers": [4, 8, 15, 16, 23, 42],
                "foo": null,
                "bar": -2.42e+17,
                "baz": true
            }
            """;

        Assert.That(regex.IsMatch(subject), Is.True);
        Assert.That(regex.IsMatch(subject.AsSpan()), Is.True);
    }
}
