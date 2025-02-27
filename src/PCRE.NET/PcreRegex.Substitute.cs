﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using PCRE.Internal;

namespace PCRE;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
public partial class PcreRegex
{
    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(string subject, string replacement)
        => Substitute(subject, replacement, 0, PcreSubstituteOptions.None, null, null, null, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement)
        => Substitute(subject, replacement, 0, PcreSubstituteOptions.None, null, null, null, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(string subject, string replacement, PcreSubstituteOptions options)
        => Substitute(subject, replacement, 0, options, null, null, null, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement, PcreSubstituteOptions options)
        => Substitute(subject, replacement, 0, options, null, null, null, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="startIndex" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(string subject, string replacement, int startIndex, PcreSubstituteOptions options)
        => Substitute(subject, replacement, startIndex, options, null, null, null, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="startIndex" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement, int startIndex, PcreSubstituteOptions options)
        => Substitute(subject, replacement, startIndex, options, null, null, null, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="options" or @name="onSubstituteCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    public string Substitute(string subject, string replacement, PcreSubstituteOptions options, PcreSubstituteCalloutFunc? onSubstituteCallout)
        => Substitute(subject, replacement, 0, options, null, onSubstituteCallout, null, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="options" or @name="onSubstituteCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement, PcreSubstituteOptions options, PcreSubstituteCalloutFunc? onSubstituteCallout)
        => Substitute(subject, replacement, 0, options, null, onSubstituteCallout, null, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="startIndex" or @name="options" or @name="onSubstituteCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    public string Substitute(string subject, string replacement, int startIndex, PcreSubstituteOptions options, PcreSubstituteCalloutFunc? onSubstituteCallout)
        => Substitute(subject, replacement, startIndex, options, null, onSubstituteCallout, null, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="startIndex" or @name="options" or @name="onSubstituteCallout"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement, int startIndex, PcreSubstituteOptions options, PcreSubstituteCalloutFunc? onSubstituteCallout)
        => Substitute(subject, replacement, startIndex, options, null, onSubstituteCallout, null, null);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="options" or @name="startIndex" or @name="onSubstituteCallout" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    public string Substitute(string subject, string replacement, int startIndex, PcreSubstituteOptions options, PcreSubstituteCalloutFunc? onSubstituteCallout, PcreMatchSettings? settings)
        => Substitute(subject, replacement, startIndex, options, null, onSubstituteCallout, null, settings);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="startIndex" or @name="options" or @name="onSubstituteCallout" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement, int startIndex, PcreSubstituteOptions options, PcreSubstituteCalloutFunc? onSubstituteCallout, PcreMatchSettings? settings)
        => Substitute(subject, replacement, startIndex, options, null, onSubstituteCallout, null, settings);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="options" or @name="startIndex" or @name="onMatchCallout" or @name="onSubstituteCallout" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    public string Substitute(string subject, string replacement, int startIndex, PcreSubstituteOptions options, PcreRefCalloutFunc? onMatchCallout, PcreSubstituteCalloutFunc? onSubstituteCallout, PcreMatchSettings? settings)
        => Substitute(subject, replacement, startIndex, options, onMatchCallout, onSubstituteCallout, null, settings);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="options" or @name="startIndex" or @name="onMatchCallout" or @name="onSubstituteCallout" or @name="onSubstituteCaseCallout" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    public string Substitute(string subject, string replacement, int startIndex, PcreSubstituteOptions options, PcreRefCalloutFunc? onMatchCallout, PcreSubstituteCalloutFunc? onSubstituteCallout, PcreSubstituteCaseCalloutFunc? onSubstituteCaseCallout, PcreMatchSettings? settings)
    {
        if (subject == null)
            throw new ArgumentNullException(nameof(subject));

        if (replacement == null)
            throw new ArgumentNullException(nameof(replacement));

        if (unchecked((uint)startIndex > (uint)subject.Length))
            ThrowInvalidStartIndex();

        return InternalRegex.Substitute(
            subject.AsSpan(),
            subject,
            replacement.AsSpan(),
            settings,
            startIndex,
            options.ToSubstituteOptions(),
            onMatchCallout,
            onSubstituteCallout,
            onSubstituteCaseCallout,
            out _
        );
    }

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="startIndex" or @name="options" or @name="onMatchCallout" or @name="onSubstituteCallout" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement, int startIndex, PcreSubstituteOptions options, PcreRefCalloutFunc? onMatchCallout, PcreSubstituteCalloutFunc? onSubstituteCallout, PcreMatchSettings? settings)
        => Substitute(subject, replacement, startIndex, options, onMatchCallout, onSubstituteCallout, null, settings);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="startIndex" or @name="options" or @name="onMatchCallout" or @name="onSubstituteCallout" or @name="onSubstituteCaseCallout" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement, int startIndex, PcreSubstituteOptions options, PcreRefCalloutFunc? onMatchCallout, PcreSubstituteCalloutFunc? onSubstituteCallout, PcreSubstituteCaseCalloutFunc? onSubstituteCaseCallout, PcreMatchSettings? settings)
    {
        if (unchecked((uint)startIndex > (uint)subject.Length))
            ThrowInvalidStartIndex();

        return InternalRegex.Substitute(
            subject,
            null,
            replacement,
            settings,
            startIndex,
            options.ToSubstituteOptions(),
            onMatchCallout,
            onSubstituteCallout,
            onSubstituteCaseCallout,
            out _
        );
    }

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="replacement"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static" or @name="replacementStringPcre2"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public static string Substitute(string subject, string pattern, string replacement)
        => Substitute(subject, pattern, replacement, PcreOptions.None, PcreSubstituteOptions.None);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="replacement" or @name="patternOptions" or @name="substituteOptions"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static" or @name="replacementStringPcre2"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public static string Substitute(string subject, string pattern, string replacement, PcreOptions patternOptions, PcreSubstituteOptions substituteOptions)
        => new PcreRegex(pattern, patternOptions).Substitute(subject, replacement, substituteOptions);
}
