using System;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// A callout function for a case transformation.
/// </summary>
public delegate string PcreSubstituteCaseCalloutFunc(ReadOnlySpan<char> input, PcreSubstituteCase targetCase);

/// <summary>
/// Indicates which case substitution should be applied.
/// </summary>
public enum PcreSubstituteCase
{
    /// <summary>
    /// No substitution.
    /// </summary>
    None = 0,

    /// <summary>
    /// The input should be converted to lowercase.
    /// </summary>
    Lower = (int)PcreConstants.SUBSTITUTE_CASE_LOWER,

    /// <summary>
    /// The input should be converted to uppercase.
    /// </summary>
    Upper = (int)PcreConstants.SUBSTITUTE_CASE_UPPER,

    /// <summary>
    /// The first character or glyph should be transformed to Unicode titlecase and the rest to Unicode lowercase.
    /// </summary>
    /// <remarks>
    /// Note that titlecasing sometimes uses Unicode properties to titlecase each word in a string;
    /// but PCRE2 is requesting that only the single leading character is to be titlecased.
    /// </remarks>
    TitleFirst = (int)PcreConstants.SUBSTITUTE_CASE_TITLE_FIRST
}
