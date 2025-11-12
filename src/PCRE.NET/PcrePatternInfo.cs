using System;
using System.Collections.Generic;
using System.Linq;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// Information about a regex pattern.
/// </summary>
public sealed class PcrePatternInfo
{
    private readonly InternalRegex _re;

    internal PcrePatternInfo(InternalRegex re)
        => _re = re;

    /// <summary>
    /// Returns the regex pattern string.
    /// </summary>
    public string PatternString => _re.Pattern;

    /// <summary>
    /// Returns the advanced settings that were used.
    /// </summary>
    public PcreRegexSettings Settings => _re.Settings;

    /// <summary>
    /// Returns the options used for pattern compilation.
    /// </summary>
    public PcreOptions Options => _re.Settings.Options;

    /// <summary>
    /// Returns the JIT options used for pattern compilation.
    /// </summary>
    public PcreJitCompileOptions JitOptions => _re.Settings.JitCompileOptions;

    /// <summary>
    /// Returns the capturing groups count.
    /// </summary>
    public int CaptureCount => _re.CaptureCount;

    /// <summary>
    /// <c>PCRE2_INFO_ARGOPTIONS</c> - Returns the options used for pattern compilation.
    /// </summary>
    public PcreOptions ArgOptions => (PcreOptions)_re.GetInfoUInt32(PcreConstants.PCRE2_INFO_ARGOPTIONS);

    /// <summary>
    /// <c>PCRE2_INFO_ALLOPTIONS</c> - Returns the options used for pattern compilation
    /// as modified by any top-level <c>(*XXX)</c> option settings such as <c>(*UTF)</c> at the start of the pattern itself.
    /// </summary>
    public PcreOptions AllOptions => (PcreOptions)_re.GetInfoUInt32(PcreConstants.PCRE2_INFO_ALLOPTIONS);

    /// <summary>
    /// <c>PCRE2_INFO_EXTRAOPTIONS</c> - Returns the extra options used for pattern compilation.
    /// </summary>
    public PcreExtraCompileOptions ExtraOptions => (PcreExtraCompileOptions)_re.GetInfoUInt32(PcreConstants.PCRE2_INFO_EXTRAOPTIONS);

    /// <summary>
    /// <c>PCRE2_INFO_BACKREFMAX</c> - Returns the number of the highest backreference in the pattern.
    /// </summary>
    /// <remarks>
    /// Named capture groups acquire numbers as well as names, and these count towards the highest backreference.
    /// Backreferences such as <c>\4</c> or <c>\g{12}</c> match the captured characters of the given group, but in addition,
    /// the check that a capture group is set in a conditional group such as <c>(?(3)a|b)</c> is also a backreference.
    /// Zero is returned if there are no backreferences.
    /// </remarks>
    public uint MaxBackReference => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_BACKREFMAX);

    /// <summary>
    /// <c>PCRE2_INFO_JITSIZE</c> - Returns the size of the JIT compiled code, or zero if the pattern wasn't JIT-compiled.
    /// </summary>
    public bool IsCompiled => _re.GetInfoNativeInt(PcreConstants.PCRE2_INFO_JITSIZE) != 0;

    /// <summary>
    /// <c>PCRE2_INFO_MATCHEMPTY</c> - Indicates of the pattern might match an empty string.
    /// </summary>
    public bool CanMatchEmptyString => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_MATCHEMPTY) != 0;

    /// <summary>
    /// <c>PCRE2_INFO_MAXLOOKBEHIND</c> - Maximum length of a lookbehind.
    /// </summary>
    /// <remarks>
    /// A lookbehind assertion moves back a certain number of characters (not code units) when it starts to process each of its branches.
    /// This request returns the largest of these backward moves.
    /// The simple assertions <c>\b</c> and <c>\B</c> require a one-character lookbehind and cause <see cref="MaxLookBehind"/> to return 1 in the absence of anything longer.
    /// <c>\A</c> also registers a one-character lookbehind, though it does not actually inspect the previous character.
    /// Note that this information is useful for multi-segment matching only if the pattern contains no nested lookbehinds.
    /// For example, the pattern <c>(?&lt;=a(?&lt;=ba)c)</c> returns a maximum lookbehind of 2, but when it is processed,
    /// the first lookbehind moves back by two characters, matches one character, then the nested lookbehind also moves back by two characters.
    /// This puts the matching point three characters earlier than it was at the start.
    /// <see cref="MaxLookBehind"/> is really only useful as a debugging tool.
    /// </remarks>
    public uint MaxLookBehind => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_MAXLOOKBEHIND);

    /// <summary>
    /// <c>PCRE2_INFO_MINLENGTH</c> - The minimum length for matching subject strings if computed, or zero otherwise.
    /// </summary>
    /// <remarks>
    /// This value is not computed when <see cref="PcreOptions.NoStartOptimize"/> is set.
    /// The value is a number of characters, which in UTF mode may be different from the number of code units.
    /// The value is a lower bound to the length of any matching string.
    /// There may not be any strings of that length that do actually match, but every string that does match is at least that long.
    /// </remarks>
    public uint MinSubjectLength => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_MINLENGTH);

    /// <summary>
    /// <c>PCRE2_INFO_NAMECOUNT</c> - The count of named capturing groups.
    /// </summary>
    public uint NamedGroupsCount => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_NAMECOUNT);

    /// <summary>
    /// <c>PCRE2_INFO_DEPTHLIMIT</c> - Returns the backtracking depth limit if the pattern specified it
    /// by including an item of the form <c>(*LIMIT_DEPTH=nnnn)</c> at the start.
    /// </summary>
    /// <remarks>
    /// Note that this limit will only be used during matching if it is less than the limit set or defaulted by the caller of the match function.
    /// </remarks>
    public uint DepthLimit => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_DEPTHLIMIT);

    /// <summary>
    /// <c>PCRE2_INFO_BSR</c> - Indicates what character sequences the <c>\R</c> escape sequence matches.
    /// </summary>
    public PcreBackslashR BackslashR => (PcreBackslashR)_re.GetInfoUInt32(PcreConstants.PCRE2_INFO_BSR);

    /// <summary>
    /// <c>PCRE2_INFO_HASBACKSLASHC</c> - Indicates if the pattern contains any instances of <c>\C</c>.
    /// </summary>
    public bool HasBackslashC => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_HASBACKSLASHC) != 0;

    /// <summary>
    /// <c>PCRE2_INFO_HASCRORLF</c> - Indicates if the pattern contains any explicit matches for CR or LF characters.
    /// </summary>
    /// <remarks>
    /// An explicit match is either a literal CR or LF character, or <c>\r</c> or <c>\n</c> or one of the equivalent hexadecimal or octal escape sequences.
    /// </remarks>
    public bool HasCrOrLf => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_HASCRORLF) != 0;

    /// <summary>
    /// <c>PCRE2_INFO_JCHANGED</c> - Indicates if the <c>(?J)</c> or <c>(?-J)</c> option setting is used in the pattern.
    /// </summary>
    /// <remarks>
    /// <c>(?J)</c> and <c>(?-J)</c> set and unset the local <see cref="PcreOptions.DupNames"/> option, respectively.
    /// </remarks>
    public bool JChanged => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_JCHANGED) != 0;

    /// <summary>
    /// <c>PCRE2_INFO_FRAMESIZE</c> - Returns the size (in bytes) of the data frames that are used to remember backtracking positions
    /// when the pattern is processed without the use of JIT.
    /// </summary>
    /// <remarks>
    /// The frame size depends on the number of capturing parentheses in the pattern. Each additional capture group adds two <see cref="IntPtr"/> variables.
    /// </remarks>
    public ulong FrameSize => _re.GetInfoNativeInt(PcreConstants.PCRE2_INFO_FRAMESIZE);

    /// <summary>
    /// <c>PCRE2_INFO_JITSIZE</c> - Returns the size of the JIT compiled code.
    /// </summary>
    public ulong JitSize => _re.GetInfoNativeInt(PcreConstants.PCRE2_INFO_JITSIZE);

    /// <summary>
    /// <c>PCRE2_INFO_SIZE</c> - Returns the size of the compiled pattern in bytes.
    /// </summary>
    /// <remarks>
    /// This value includes the size of the general data block that precedes the code units of the compiled pattern itself.
    /// Processing a pattern with the JIT compiler does not alter the value returned by this option.
    /// </remarks>
    public ulong PatternSize => _re.GetInfoNativeInt(PcreConstants.PCRE2_INFO_SIZE);

    /// <summary>
    /// <c>PCRE2_INFO_MATCHLIMIT</c> - Returns the value of the match limit set by <c>(*LIMIT_MATCH=nnnn)</c> at the start of the pattern.
    /// </summary>
    public uint MatchLimit => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_MATCHLIMIT);

    /// <summary>
    /// <c>PCRE2_INFO_HEAPLIMIT</c> - Returns the value of the heap limit set by <c>(*LIMIT_HEAP=nnnn)</c> at the start of the pattern.
    /// </summary>
    public uint HeapLimit => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_HEAPLIMIT);

    /// <summary>
    /// <c>PCRE2_INFO_FIRSTCODETYPE</c> - Returns information about the first code unit of any matched string, for a non-anchored pattern.
    /// </summary>
    /// <remarks>
    /// If there is a fixed first value, for example, the letter "c" from a pattern such as <c>(cat|cow|coyote)</c>, true is returned,
    /// and the value can be retrieved using <see cref="FirstCodeUnit"/>. If there is no fixed first value, but it is known that a match
    /// can occur only at the start of the subject or following a newline in the subject, 2 is returned. Otherwise, and for anchored patterns, 0 is returned.
    /// </remarks>
    public uint FirstCodeType => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_FIRSTCODETYPE);

    /// <summary>
    /// <c>PCRE2_INFO_FIRSTCODEUNIT</c> - Returns the value of the first code unit of any matched string for a pattern where <see cref="FirstCodeType"/> is true.
    /// </summary>
    public uint FirstCodeUnit => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_FIRSTCODEUNIT);

    /// <summary>
    /// <c>PCRE2_INFO_LASTCODETYPE</c> - Returns true if there is a rightmost literal code unit that must exist in any matched string, other than at its start.
    /// </summary>
    /// <remarks>
    /// When true, the code unit value itself can be retrieved using <see cref="LastCodeUnit"/>. For anchored patterns, a last literal value is recorded only
    /// if it follows something of variable length. For example, for the pattern <c>/^a\d+z\d+/</c> the returned value is 1 (with "z" returned from <see cref="LastCodeUnit"/>),
    /// but for <c>/^a\dz\d/</c> the returned value is 0.
    /// </remarks>
    public uint LastCodeType => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_LASTCODETYPE);

    /// <summary>
    /// <c>PCRE2_INFO_LASTCODEUNIT</c> - Return the value of the rightmost literal code unit that must exist in any matched string, other than at its start,
    /// for a pattern where <see cref="LastCodeType"/> returns true.
    /// </summary>
    public uint LastCodeUnit => _re.GetInfoUInt32(PcreConstants.PCRE2_INFO_LASTCODEUNIT);

    /// <summary>
    /// Returns the list of callouts in the pattern.
    /// </summary>
    public IReadOnlyList<PcreCalloutInfo> Callouts => field ??= _re.GetCallouts();

    /// <summary>
    /// Returns the list of group names in the pattern.
    /// </summary>
    public IReadOnlyList<string> GroupNames => field ??= _re.CaptureNames.OrderBy(i => i.Value.Min()).Select(i => i.Key).ToList().AsReadOnly();

    /// <summary>
    /// Returns the list of indexes in the pattern for a given capturing group name.
    /// </summary>
    /// <param name="name">The capturing group name.</param>
    public IReadOnlyList<int> GetGroupIndexesByName(string name)
        => _re.CaptureNames.TryGetValue(name, out var indexes) ? indexes : [];
}
