﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;
using PCRE.Dfa;

namespace PCRE.Tests.PcreNet.Dfa;

[TestFixture]
public class DfaMatchTests
{
    [Test]
    public void should_match_with_dfa()
    {
        var re = new PcreRegex(@"<.*>");
        var result = re.Dfa.Match("This is <something> <something else> <something further> no more");

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Index, Is.EqualTo(8));

        Assert.That(result.LongestMatch, Is.Not.Null);
        Assert.That(result.LongestMatch.Value, Is.EqualTo("<something> <something else> <something further>"));
        Assert.That(result.LongestMatch.ValueSpan.ToString(), Is.EqualTo("<something> <something else> <something further>"));

        Assert.That(result.LongestMatch, Is.SameAs(result[0]));
        Assert.That(result.ShortestMatch, Is.SameAs(result[2]));

        Assert.That(result.ShortestMatch, Is.Not.Null);
        Assert.That(result.ShortestMatch.Value, Is.EqualTo("<something>"));
        Assert.That(result.ShortestMatch.ValueSpan.ToString(), Is.EqualTo("<something>"));

        Assert.That(result[1], Is.Not.Null);
        Assert.That(result[1].Value, Is.EqualTo("<something> <something else>"));
        Assert.That(result[1].ValueSpan.ToString(), Is.EqualTo("<something> <something else>"));

        Assert.That(result[3], Is.Not.Null);
        Assert.That(result[3].Value, Is.SameAs(string.Empty));
        Assert.That(result[3].ValueSpan.Length, Is.EqualTo(0));
        Assert.That(result[3].Index, Is.EqualTo(-1));
        Assert.That(result[3].Length, Is.EqualTo(0));
    }

    [Test]
    public void should_get_shortest_match()
    {
        var re = new PcreRegex(@"<.*>");
        var result = re.Dfa.Match("This is <something> <something else> <something further> no more", PcreDfaMatchOptions.DfaShortest);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Index, Is.EqualTo(8));

        Assert.That(result.ShortestMatch, Is.Not.Null);
        Assert.That(result.ShortestMatch.Value, Is.EqualTo("<something>"));
        Assert.That(result.ShortestMatch.ValueSpan.ToString(), Is.EqualTo("<something>"));
    }

    [Test]
    public void should_get_max_matches()
    {
        var re = new PcreRegex(@"<.*>");
        var result = re.Dfa.Match("This is <something> <something else> <something further> no more", new PcreDfaMatchSettings
        {
            MaxResults = 2
        });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Index, Is.EqualTo(8));

        Assert.That(result.LongestMatch, Is.Not.Null);
        Assert.That(result.LongestMatch.Value, Is.EqualTo("<something> <something else> <something further>"));
        Assert.That(result.LongestMatch.ValueSpan.ToString(), Is.EqualTo("<something> <something else> <something further>"));

        Assert.That(result.ShortestMatch, Is.Not.Null);
        Assert.That(result.ShortestMatch.Value, Is.EqualTo("<something> <something else>"));
        Assert.That(result.ShortestMatch.ValueSpan.ToString(), Is.EqualTo("<something> <something else>"));
    }

    [Test]
    public void should_start_at_given_index()
    {
        var re = new PcreRegex(@"<.*>");
        var result = re.Dfa.Match("This is <something> <something else> <something further> no more", 10);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Index, Is.EqualTo(20));

        Assert.That(result.LongestMatch, Is.Not.Null);
        Assert.That(result.LongestMatch.Value, Is.EqualTo("<something else> <something further>"));
        Assert.That(result.LongestMatch.ValueSpan.ToString(), Is.EqualTo("<something else> <something further>"));

        Assert.That(result.ShortestMatch, Is.Not.Null);
        Assert.That(result.ShortestMatch.Value, Is.EqualTo("<something else>"));
        Assert.That(result.ShortestMatch.ValueSpan.ToString(), Is.EqualTo("<something else>"));
    }

    [Test]
    public void should_execute_callouts()
    {
        var re = new PcreRegex(@"<.*(?C1)>");
        var settings = new PcreDfaMatchSettings();
        settings.OnCallout += callout => callout.Match.Subject[callout.CurrentOffset - 1] == 'e' ? PcreCalloutResult.Fail : PcreCalloutResult.Pass;

        var result = re.Dfa.Match("This is <something> <something else> <something further> no more", settings);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.LongestMatch.Value, Is.EqualTo("<something> <something else> <something further>"));
        Assert.That(result.ShortestMatch.Value, Is.EqualTo("<something>"));
    }

    [Test]
    public void should_return_value_span_from_subject_string()
    {
        var subject = string.Concat("foo", "bar");
        var re = new PcreRegex(@"b(a)(r)");

        var result = re.Dfa.Match(subject);

        ref var subjectRef = ref MemoryMarshal.GetReference(subject.AsSpan(3));
        ref var valueRef = ref MemoryMarshal.GetReference(result.LongestMatch.ValueSpan);
        Assert.That(Unsafe.AreSame(ref valueRef, ref subjectRef), Is.True);

        _ = result.LongestMatch.Value; // Reading the string value shouldn't change the span target

        valueRef = ref MemoryMarshal.GetReference(result.LongestMatch.ValueSpan);
        Assert.That(Unsafe.AreSame(ref valueRef, ref subjectRef), Is.True);
    }
}
