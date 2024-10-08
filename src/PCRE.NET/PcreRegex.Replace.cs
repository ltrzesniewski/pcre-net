﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;
using PCRE.Internal;

namespace PCRE;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
public partial class PcreRegex
{
    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementString"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    [Pure]
    public string Replace(string subject, string replacement)
        => Replace(subject, replacement, -1, 0);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="count"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementString"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    [Pure]
    public string Replace(string subject, string replacement, int count)
        => Replace(subject, replacement, count, 0);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacement" or @name="count" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="replacementString" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    [Pure]
    public string Replace(string subject, string replacement, int count, int startIndex)
    {
        if (replacement == null)
            throw new ArgumentNullException(nameof(replacement));

        return Replace(subject, Caches.ReplacementCache.GetOrAdd(replacement), count, startIndex);
    }

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacementFunc"]'/>
    /// <seealso cref="Substitute(string,string)"/>
    public string Replace(string subject, Func<PcreMatch, string> replacementFunc)
        => Replace(subject, replacementFunc, -1, 0);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacementFunc" or @name="count"]'/>
    /// <seealso cref="Substitute(string,string)"/>
    public string Replace(string subject, Func<PcreMatch, string> replacementFunc, int count)
        => Replace(subject, replacementFunc, count, 0);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="replacementFunc" or @name="count" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    public string Replace(string subject, Func<PcreMatch, string> replacementFunc, int count, int startIndex)
    {
        if (subject == null)
            throw new ArgumentNullException(nameof(subject));
        if (replacementFunc == null)
            throw new ArgumentNullException(nameof(replacementFunc));

        if (count == 0)
            return subject;

        StringBuilder? sb = null;
        var position = 0;

        foreach (var match in Matches(subject, startIndex))
        {
            sb ??= new StringBuilder((int)(subject.Length * 1.2));
            sb.Append(subject, position, match.Index - position);
            sb.Append(replacementFunc(match));
            position = match.GetStartOfNextMatchIndex();

            if (--count == 0)
                break;
        }

        if (sb == null)
            return subject;

        sb.Append(subject, position, subject.Length - position);
        return sb.ToString();
    }

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="replacement"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static" or @name="replacementString"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    [Pure]
    public static string Replace(string subject, string pattern, string replacement)
        => Replace(subject, pattern, replacement, PcreOptions.None, -1, 0);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="replacement" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static" or @name="replacementString"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    [Pure]
    public static string Replace(string subject, string pattern, string replacement, PcreOptions options)
        => Replace(subject, pattern, replacement, options, -1, 0);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="replacement" or @name="options" or @name="count"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static" or @name="replacementString"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    [Pure]
    public static string Replace(string subject, string pattern, string replacement, PcreOptions options, int count)
        => Replace(subject, pattern, replacement, options, count, 0);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="replacement" or @name="options" or @name="count" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static" or @name="replacementString" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    [Pure]
    public static string Replace(string subject, string pattern, string replacement, PcreOptions options, int count, int startIndex)
        => new PcreRegex(pattern, options).Replace(subject, replacement, count, startIndex);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="replacementFunc"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    public static string Replace(string subject, string pattern, Func<PcreMatch, string> replacementFunc)
        => Replace(subject, pattern, replacementFunc, PcreOptions.None, -1, 0);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="replacementFunc" or @name="options"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    public static string Replace(string subject, string pattern, Func<PcreMatch, string> replacementFunc, PcreOptions options)
        => Replace(subject, pattern, replacementFunc, options, -1, 0);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="replacementFunc" or @name="options" or @name="count"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    public static string Replace(string subject, string pattern, Func<PcreMatch, string> replacementFunc, PcreOptions options, int count)
        => Replace(subject, pattern, replacementFunc, options, count, 0);

    /// <include file='PcreRegex.xml' path='/doc/method[@name="Replace"]/*'/>
    /// <include file='PcreRegex.xml' path='/doc/param[@name="subject" or @name="pattern" or @name="replacementFunc" or @name="options" or @name="count" or @name="startIndex"]'/>
    /// <remarks>
    /// <include file='PcreRegex.xml' path='/doc/remarks[@name="static" or @name="startIndex"]/*'/>
    /// </remarks>
    /// <seealso cref="Substitute(string,string)"/>
    public static string Replace(string subject, string pattern, Func<PcreMatch, string> replacementFunc, PcreOptions options, int count, int startIndex)
        => new PcreRegex(pattern, options).Replace(subject, replacementFunc, count, startIndex);
}
