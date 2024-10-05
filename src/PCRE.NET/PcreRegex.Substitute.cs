using System;
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
        => Substitute(subject, replacement, 0, PcreSubstituteOptions.None, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement)
        => Substitute(subject, replacement, 0, PcreSubstituteOptions.None, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(string subject, string replacement, PcreSubstituteOptions options)
        => Substitute(subject, replacement, 0, options, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement, PcreSubstituteOptions options)
        => Substitute(subject, replacement, 0, options, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="options" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(string subject, string replacement, int startIndex, PcreSubstituteOptions options)
        => Substitute(subject, replacement, startIndex, options, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="startIndex" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement, int startIndex, PcreSubstituteOptions options)
        => Substitute(subject, replacement, startIndex, options, PcreMatchSettings.Default);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="options" or @name="startIndex" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(string subject, string replacement, int startIndex, PcreSubstituteOptions options, PcreMatchSettings settings)
    {
        if (subject == null)
            throw new ArgumentNullException(nameof(subject));

        if (replacement == null)
            throw new ArgumentNullException(nameof(replacement));

        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        if (unchecked((uint)startIndex > (uint)subject.Length))
            ThrowInvalidStartIndex();

        return InternalRegex.Substitute(subject.AsSpan(), subject, replacement.AsSpan(), settings, startIndex, options.ToSubstituteOptions());
    }

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Substitute"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="startIndex" or @name="options" or @name="settings"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementStringPcre2" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Replace(string,string)"/>
    [Pure]
    public string Substitute(ReadOnlySpan<char> subject, ReadOnlySpan<char> replacement, int startIndex, PcreSubstituteOptions options, PcreMatchSettings settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        if (unchecked((uint)startIndex > (uint)subject.Length))
            ThrowInvalidStartIndex();

        return InternalRegex.Substitute(subject, null, replacement, settings, startIndex, options.ToSubstituteOptions());
    }
}
