using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// An error code returned by PCRE.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "CommentTypo")]
public enum PcreErrorCode
{
    /// <summary>
    /// No error.
    /// </summary>
    None = 0,

    /// <summary>
    /// <c>PCRE2_ERROR_END_BACKSLASH</c> - <c>\</c> at end of pattern.
    /// </summary>
    EndBackslash = PcreConstants.ERROR_END_BACKSLASH,

    /// <summary>
    /// <c>PCRE2_ERROR_END_BACKSLASH_C</c> - <c>\c</c> at end of pattern.
    /// </summary>
    EndBackslashC = PcreConstants.ERROR_END_BACKSLASH_C,

    /// <summary>
    /// <c>PCRE2_ERROR_UNKNOWN_ESCAPE</c> - Unrecognized character follows <c>\</c>
    /// </summary>
    UnknownEscape = PcreConstants.ERROR_UNKNOWN_ESCAPE,

    /// <summary>
    /// <c>PCRE2_ERROR_QUANTIFIER_OUT_OF_ORDER</c> - Numbers out of order in <c>{}</c> quantifier.
    /// </summary>
    QuantifierOutOfOrder = PcreConstants.ERROR_QUANTIFIER_OUT_OF_ORDER,

    /// <summary>
    /// <c>PCRE2_ERROR_QUANTIFIER_TOO_BIG</c> - Number too big in <c>{}</c> quantifier.
    /// </summary>
    QuantifierTooBig = PcreConstants.ERROR_QUANTIFIER_TOO_BIG,

    /// <summary>
    /// <c>PCRE2_ERROR_MISSING_SQUARE_BRACKET</c> - Missing terminating <c>]</c> for character class.
    /// </summary>
    MissingSquareBracket = PcreConstants.ERROR_MISSING_SQUARE_BRACKET,

    /// <summary>
    /// <c>PCRE2_ERROR_ESCAPE_INVALID_IN_CLASS</c> - Escape sequence is invalid in character class.
    /// </summary>
    EscapeInvalidInClass = PcreConstants.ERROR_ESCAPE_INVALID_IN_CLASS,

    /// <summary>
    /// <c>PCRE2_ERROR_CLASS_RANGE_ORDER</c> - Range out of order in character class.
    /// </summary>
    ClassRangeOrder = PcreConstants.ERROR_CLASS_RANGE_ORDER,

    /// <summary>
    /// <c>PCRE2_ERROR_QUANTIFIER_INVALID</c> - Quantifier does not follow a repeatable item.
    /// </summary>
    QuantifierInvalid = PcreConstants.ERROR_QUANTIFIER_INVALID,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_UNEXPECTED_REPEAT</c> - Internal error: unexpected repeat.
    /// </summary>
    InternalUnexpectedRepeat = PcreConstants.ERROR_INTERNAL_UNEXPECTED_REPEAT,

    /// <summary>
    /// <c>PCRE2_ERROR_INVALID_AFTER_PARENS_QUERY</c> - Unrecognized character after <c>(?</c> or <c>(?-</c>
    /// </summary>
    InvalidAfterParensQuery = PcreConstants.ERROR_INVALID_AFTER_PARENS_QUERY,

    /// <summary>
    /// <c>PCRE2_ERROR_POSIX_CLASS_NOT_IN_CLASS</c> - POSIX named classes are supported only within a class.
    /// </summary>
    PosixClassNotInClass = PcreConstants.ERROR_POSIX_CLASS_NOT_IN_CLASS,

    /// <summary>
    /// <c>PCRE2_ERROR_POSIX_NO_SUPPORT_COLLATING</c> - POSIX collating elements are not supported.
    /// </summary>
    PosixNoSupportCollating = PcreConstants.ERROR_POSIX_NO_SUPPORT_COLLATING,

    /// <summary>
    /// <c>PCRE2_ERROR_MISSING_CLOSING_PARENTHESIS</c> - Missing closing parenthesis.
    /// </summary>
    MissingClosingParenthesis = PcreConstants.ERROR_MISSING_CLOSING_PARENTHESIS,

    /// <summary>
    /// <c>PCRE2_ERROR_BAD_SUBPATTERN_REFERENCE</c> - Reference to non-existent subpattern.
    /// </summary>
    BadSubpatternReference = PcreConstants.ERROR_BAD_SUBPATTERN_REFERENCE,

    /// <summary>
    /// <c>PCRE2_ERROR_NULL_PATTERN</c> - Pattern passed as NULL.
    /// </summary>
    NullPattern = PcreConstants.ERROR_NULL_PATTERN,

    /// <summary>
    /// <c>PCRE2_ERROR_BAD_OPTIONS</c> - Unrecognised compile-time option bit(s).
    /// </summary>
    BadOptions = PcreConstants.ERROR_BAD_OPTIONS,

    /// <summary>
    /// <c>PCRE2_ERROR_MISSING_COMMENT_CLOSING</c> - Missing <c>)</c> after <c>(?#</c> comment.
    /// </summary>
    MissingCommentClosing = PcreConstants.ERROR_MISSING_COMMENT_CLOSING,

    /// <summary>
    /// <c>PCRE2_ERROR_PARENTHESES_NEST_TOO_DEEP</c> - Parentheses are too deeply nested.
    /// </summary>
    ParenthesesNestTooDeep = PcreConstants.ERROR_PARENTHESES_NEST_TOO_DEEP,

    /// <summary>
    /// <c>PCRE2_ERROR_PATTERN_TOO_LARGE</c> - Regular expression is too large.
    /// </summary>
    PatternTooLarge = PcreConstants.ERROR_PATTERN_TOO_LARGE,

    /// <summary>
    /// <c>PCRE2_ERROR_HEAP_FAILED</c> - Failed to allocate heap memory.
    /// </summary>
    HeapFailed = PcreConstants.ERROR_HEAP_FAILED,

    /// <summary>
    /// <c>PCRE2_ERROR_UNMATCHED_CLOSING_PARENTHESIS</c> - Unmatched closing parenthesis.
    /// </summary>
    UnmatchedClosingParenthesis = PcreConstants.ERROR_UNMATCHED_CLOSING_PARENTHESIS,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_CODE_OVERFLOW</c> - Internal error: code overflow.
    /// </summary>
    InternalCodeOverflow = PcreConstants.ERROR_INTERNAL_CODE_OVERFLOW,

    /// <summary>
    /// <c>PCRE2_ERROR_MISSING_CONDITION_CLOSING</c> - Missing closing parenthesis for condition.
    /// </summary>
    MissingConditionClosing = PcreConstants.ERROR_MISSING_CONDITION_CLOSING,

    /// <summary>
    /// <c>PCRE2_ERROR_LOOKBEHIND_NOT_FIXED_LENGTH</c> - Lookbehind assertion is not fixed length.
    /// </summary>
    LookbehindNotFixedLength = PcreConstants.ERROR_LOOKBEHIND_NOT_FIXED_LENGTH,

    /// <summary>
    /// <c>PCRE2_ERROR_ZERO_RELATIVE_REFERENCE</c> - A relative value of zero is not allowed.
    /// </summary>
    ZeroRelativeReference = PcreConstants.ERROR_ZERO_RELATIVE_REFERENCE,

    /// <summary>
    /// <c>PCRE2_ERROR_TOO_MANY_CONDITION_BRANCHES</c> - Conditional subpattern contains more than two branches.
    /// </summary>
    TooManyConditionBranches = PcreConstants.ERROR_TOO_MANY_CONDITION_BRANCHES,

    /// <summary>
    /// <c>PCRE2_ERROR_CONDITION_ASSERTION_EXPECTED</c> - Assertion expected after <c>(?(</c> or <c>(?(?C)</c>
    /// </summary>
    ConditionAssertionExpected = PcreConstants.ERROR_CONDITION_ASSERTION_EXPECTED,

    /// <summary>
    /// <c>PCRE2_ERROR_BAD_RELATIVE_REFERENCE</c> - Digit expected after <c>(?+</c> or <c>(?-</c>
    /// </summary>
    BadRelativeReference = PcreConstants.ERROR_BAD_RELATIVE_REFERENCE,

    /// <summary>
    /// <c>PCRE2_ERROR_UNKNOWN_POSIX_CLASS</c> - Unknown POSIX class name.
    /// </summary>
    UnknownPosixClass = PcreConstants.ERROR_UNKNOWN_POSIX_CLASS,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_STUDY_ERROR</c> - Internal error in <c>pcre2_study()</c>: should not occur.
    /// </summary>
    InternalStudyError = PcreConstants.ERROR_INTERNAL_STUDY_ERROR,

    /// <summary>
    /// <c>PCRE2_ERROR_UNICODE_NOT_SUPPORTED</c> - This version of PCRE2 does not have Unicode support.
    /// </summary>
    UnicodeNotSupported = PcreConstants.ERROR_UNICODE_NOT_SUPPORTED,

    /// <summary>
    /// <c>PCRE2_ERROR_PARENTHESES_STACK_CHECK</c> - Parentheses are too deeply nested (stack check).
    /// </summary>
    ParenthesesStackCheck = PcreConstants.ERROR_PARENTHESES_STACK_CHECK,

    /// <summary>
    /// <c>PCRE2_ERROR_CODE_POINT_TOO_BIG</c> - Character code point value in <c>\x{}</c> or <c>\o{}</c> is too large.
    /// </summary>
    CodePointTooBig = PcreConstants.ERROR_CODE_POINT_TOO_BIG,

    /// <summary>
    /// <c>PCRE2_ERROR_LOOKBEHIND_TOO_COMPLICATED</c> - Lookbehind is too complicated.
    /// </summary>
    LookbehindTooComplicated = PcreConstants.ERROR_LOOKBEHIND_TOO_COMPLICATED,

    /// <summary>
    /// <c>PCRE2_ERROR_LOOKBEHIND_INVALID_BACKSLASH_C</c> - <c>\C</c> is not allowed in a lookbehind assertion in UTF-16 mode.
    /// </summary>
    LookbehindInvalidBackslashC = PcreConstants.ERROR_LOOKBEHIND_INVALID_BACKSLASH_C,

    /// <summary>
    /// <c>PCRE2_ERROR_UNSUPPORTED_ESCAPE_SEQUENCE</c> - PCRE2 does not support <c>\F</c>, <c>\L</c>, <c>\l</c>, <c>\N{name}</c>, <c>\U</c>, or <c>\u</c>
    /// </summary>
    UnsupportedEscapeSequence = PcreConstants.ERROR_UNSUPPORTED_ESCAPE_SEQUENCE,

    /// <summary>
    /// <c>PCRE2_ERROR_CALLOUT_NUMBER_TOO_BIG</c> - Number after <c>(?C</c> is greater than 255.
    /// </summary>
    CalloutNumberTooBig = PcreConstants.ERROR_CALLOUT_NUMBER_TOO_BIG,

    /// <summary>
    /// <c>PCRE2_ERROR_MISSING_CALLOUT_CLOSING</c> - Closing parenthesis for <c>(?C</c> expected.
    /// </summary>
    MissingCalloutClosing = PcreConstants.ERROR_MISSING_CALLOUT_CLOSING,

    /// <summary>
    /// <c>PCRE2_ERROR_ESCAPE_INVALID_IN_VERB</c> - Invalid escape sequence in <c>(*VERB)</c> name.
    /// </summary>
    EscapeInvalidInVerb = PcreConstants.ERROR_ESCAPE_INVALID_IN_VERB,

    /// <summary>
    /// <c>PCRE2_ERROR_UNRECOGNIZED_AFTER_QUERY_P</c> - Unrecognized character after <c>(?P</c>
    /// </summary>
    UnrecognizedAfterQueryP = PcreConstants.ERROR_UNRECOGNIZED_AFTER_QUERY_P,

    /// <summary>
    /// <c>PCRE2_ERROR_MISSING_NAME_TERMINATOR</c> - Syntax error in subpattern name (missing terminator?).
    /// </summary>
    MissingNameTerminator = PcreConstants.ERROR_MISSING_NAME_TERMINATOR,

    /// <summary>
    /// <c>PCRE2_ERROR_DUPLICATE_SUBPATTERN_NAME</c> - Two named subpatterns have the same name (<see cref="PcreOptions.DupNames"/> not set).
    /// </summary>
    DuplicateSubpatternName = PcreConstants.ERROR_DUPLICATE_SUBPATTERN_NAME,

    /// <summary>
    /// <c>PCRE2_ERROR_INVALID_SUBPATTERN_NAME</c> - Subpattern name must start with a non-digit.
    /// </summary>
    InvalidSubpatternName = PcreConstants.ERROR_INVALID_SUBPATTERN_NAME,

    /// <summary>
    /// <c>PCRE2_ERROR_UNICODE_PROPERTIES_UNAVAILABLE</c> - This version of PCRE2 does not have support for <c>\P</c>, \p, or <c>\X</c>
    /// </summary>
    UnicodePropertiesUnavailable = PcreConstants.ERROR_UNICODE_PROPERTIES_UNAVAILABLE,

    /// <summary>
    /// <c>PCRE2_ERROR_MALFORMED_UNICODE_PROPERTY</c> - Malformed <c>\P</c> or <c>\p</c> sequence.
    /// </summary>
    MalformedUnicodeProperty = PcreConstants.ERROR_MALFORMED_UNICODE_PROPERTY,

    /// <summary>
    /// <c>PCRE2_ERROR_UNKNOWN_UNICODE_PROPERTY</c> - Unknown property name after <c>\P</c> or <c>\p</c>
    /// </summary>
    UnknownUnicodeProperty = PcreConstants.ERROR_UNKNOWN_UNICODE_PROPERTY,

    /// <summary>
    /// <c>PCRE2_ERROR_SUBPATTERN_NAME_TOO_LONG</c> - Subpattern name is too long (maximum 32 code units).
    /// </summary>
    SubpatternNameTooLong = PcreConstants.ERROR_SUBPATTERN_NAME_TOO_LONG,

    /// <summary>
    /// <c>PCRE2_ERROR_TOO_MANY_NAMED_SUBPATTERNS</c> - Too many named subpatterns (maximum 10000).
    /// </summary>
    TooManyNamedSubpatterns = PcreConstants.ERROR_TOO_MANY_NAMED_SUBPATTERNS,

    /// <summary>
    /// <c>PCRE2_ERROR_CLASS_INVALID_RANGE</c> - Invalid range in character class.
    /// </summary>
    ClassInvalidRange = PcreConstants.ERROR_CLASS_INVALID_RANGE,

    /// <summary>
    /// <c>PCRE2_ERROR_OCTAL_BYTE_TOO_BIG</c> - Octal value is greater than <c>\377</c> in 8-bit non-UTF-8 mode.
    /// </summary>
    OctalByteTooBig = PcreConstants.ERROR_OCTAL_BYTE_TOO_BIG,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_OVERRAN_WORKSPACE</c> - Internal error: overran compiling workspace.
    /// </summary>
    InternalOverranWorkspace = PcreConstants.ERROR_INTERNAL_OVERRAN_WORKSPACE,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_MISSING_SUBPATTERN</c> - Internal error: previously-checked referenced subpattern not found.
    /// </summary>
    InternalMissingSubpattern = PcreConstants.ERROR_INTERNAL_MISSING_SUBPATTERN,

    /// <summary>
    /// <c>PCRE2_ERROR_DEFINE_TOO_MANY_BRANCHES</c> - <c>DEFINE</c> subpattern contains more than one branch.
    /// </summary>
    DefineTooManyBranches = PcreConstants.ERROR_DEFINE_TOO_MANY_BRANCHES,

    /// <summary>
    /// <c>PCRE2_ERROR_BACKSLASH_O_MISSING_BRACE</c> - Missing opening brace after <c>\o</c>
    /// </summary>
    BackslashOMissingBrace = PcreConstants.ERROR_BACKSLASH_O_MISSING_BRACE,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_UNKNOWN_NEWLINE</c> - Internal error: unknown newline setting.
    /// </summary>
    InternalUnknownNewline = PcreConstants.ERROR_INTERNAL_UNKNOWN_NEWLINE,

    /// <summary>
    /// <c>PCRE2_ERROR_BACKSLASH_G_SYNTAX</c> - <c>\g</c> is not followed by a braced, angle-bracketed, or quoted name/number or by a plain number.
    /// </summary>
    BackslashGSyntax = PcreConstants.ERROR_BACKSLASH_G_SYNTAX,

    /// <summary>
    /// <c>PCRE2_ERROR_PARENS_QUERY_R_MISSING_CLOSING</c> - <c>(?R</c> (recursive pattern call) must be followed by a closing parenthesis.
    /// </summary>
    ParensQueryRMissingClosing = PcreConstants.ERROR_PARENS_QUERY_R_MISSING_CLOSING,

    /// <summary>
    /// <c>PCRE2_ERROR_VERB_ARGUMENT_NOT_ALLOWED</c> - Obsolete error (should not occur).
    /// </summary>
    VerbArgumentNotAllowed = PcreConstants.ERROR_VERB_ARGUMENT_NOT_ALLOWED,

    /// <summary>
    /// <c>PCRE2_ERROR_VERB_UNKNOWN</c> - <c>(*VERB)</c> not recognized or malformed.
    /// </summary>
    VerbUnknown = PcreConstants.ERROR_VERB_UNKNOWN,

    /// <summary>
    /// <c>PCRE2_ERROR_SUBPATTERN_NUMBER_TOO_BIG</c> - Subpattern number is too big.
    /// </summary>
    SubpatternNumberTooBig = PcreConstants.ERROR_SUBPATTERN_NUMBER_TOO_BIG,

    /// <summary>
    /// <c>PCRE2_ERROR_SUBPATTERN_NAME_EXPECTED</c> - Subpattern name expected.
    /// </summary>
    SubpatternNameExpected = PcreConstants.ERROR_SUBPATTERN_NAME_EXPECTED,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_PARSED_OVERFLOW</c> - Internal error: parsed pattern overflow.
    /// </summary>
    InternalParsedOverflow = PcreConstants.ERROR_INTERNAL_PARSED_OVERFLOW,

    /// <summary>
    /// <c>PCRE2_ERROR_INVALID_OCTAL</c> - Non-octal character in <c>\o{}</c> (closing brace missing?).
    /// </summary>
    InvalidOctal = PcreConstants.ERROR_INVALID_OCTAL,

    /// <summary>
    /// <c>PCRE2_ERROR_SUBPATTERN_NAMES_MISMATCH</c> - Different names for subpatterns of the same number are not allowed.
    /// </summary>
    SubpatternNamesMismatch = PcreConstants.ERROR_SUBPATTERN_NAMES_MISMATCH,

    /// <summary>
    /// <c>PCRE2_ERROR_MARK_MISSING_ARGUMENT</c> - <c>(*MARK)</c> must have an argument.
    /// </summary>
    MarkMissingArgument = PcreConstants.ERROR_MARK_MISSING_ARGUMENT,

    /// <summary>
    /// <c>PCRE2_ERROR_INVALID_HEXADECIMAL</c> - Non-hex character in <c>\x{}</c> (closing brace missing?).
    /// </summary>
    InvalidHexadecimal = PcreConstants.ERROR_INVALID_HEXADECIMAL,

    /// <summary>
    /// <c>PCRE2_ERROR_BACKSLASH_C_SYNTAX</c> - <c>\c</c> must be followed by a printable ASCII character.
    /// </summary>
    BackslashCSyntax = PcreConstants.ERROR_BACKSLASH_C_SYNTAX,

    /// <summary>
    /// <c>PCRE2_ERROR_BACKSLASH_K_SYNTAX</c> - <c>\k</c> is not followed by a braced, angle-bracketed, or quoted name.
    /// </summary>
    BackslashKSyntax = PcreConstants.ERROR_BACKSLASH_K_SYNTAX,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_BAD_CODE_LOOKBEHINDS</c> - Internal error: unknown meta code in <c>check_lookbehinds()</c>.
    /// </summary>
    InternalBadCodeLookbehinds = PcreConstants.ERROR_INTERNAL_BAD_CODE_LOOKBEHINDS,

    /// <summary>
    /// <c>PCRE2_ERROR_BACKSLASH_N_IN_CLASS</c> - <c>\N</c> is not supported in a class.
    /// </summary>
    BackslashNInClass = PcreConstants.ERROR_BACKSLASH_N_IN_CLASS,

    /// <summary>
    /// <c>PCRE2_ERROR_CALLOUT_STRING_TOO_LONG</c> - Callout string is too long.
    /// </summary>
    CalloutStringTooLong = PcreConstants.ERROR_CALLOUT_STRING_TOO_LONG,

    /// <summary>
    /// <c>PCRE2_ERROR_UNICODE_DISALLOWED_CODE_POINT</c> - Disallowed Unicode code point (&gt;= 0xd800 &amp;&amp; &lt;= 0xdfff).
    /// </summary>
    UnicodeDisallowedCodePoint = PcreConstants.ERROR_UNICODE_DISALLOWED_CODE_POINT,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF_IS_DISABLED</c> - Using UTF is disabled by the application.
    /// </summary>
    UtfIsDisabled = PcreConstants.ERROR_UTF_IS_DISABLED,

    /// <summary>
    /// <c>PCRE2_ERROR_UCP_IS_DISABLED</c> - Using UCP is disabled by the application.
    /// </summary>
    UcpIsDisabled = PcreConstants.ERROR_UCP_IS_DISABLED,

    /// <summary>
    /// <c>PCRE2_ERROR_VERB_NAME_TOO_LONG</c> - Name is too long in <c>(*MARK)</c>, <c>(*PRUNE)</c>, <c>(*SKIP)</c>, or <c>(*THEN)</c>
    /// </summary>
    VerbNameTooLong = PcreConstants.ERROR_VERB_NAME_TOO_LONG,

    /// <summary>
    /// <c>PCRE2_ERROR_BACKSLASH_U_CODE_POINT_TOO_BIG</c> - Character code point value in <c>\u....</c> sequence is too large.
    /// </summary>
    BackslashUCodePointTooBig = PcreConstants.ERROR_BACKSLASH_U_CODE_POINT_TOO_BIG,

    /// <summary>
    /// <c>PCRE2_ERROR_MISSING_OCTAL_OR_HEX_DIGITS</c> - Digits missing in <c>\x{}</c> or <c>\o{}</c> or <c>\N{U+}</c>
    /// </summary>
    MissingOctalOrHexDigits = PcreConstants.ERROR_MISSING_OCTAL_OR_HEX_DIGITS,

    /// <summary>
    /// <c>PCRE2_ERROR_VERSION_CONDITION_SYNTAX</c> - Syntax error or number too big in <c>(?(VERSION</c> condition.
    /// </summary>
    VersionConditionSyntax = PcreConstants.ERROR_VERSION_CONDITION_SYNTAX,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_BAD_CODE_AUTO_POSSESS</c> - Internal error: unknown opcode in <c>auto_possessify()</c>.
    /// </summary>
    InternalBadCodeAutoPossess = PcreConstants.ERROR_INTERNAL_BAD_CODE_AUTO_POSSESS,

    /// <summary>
    /// <c>PCRE2_ERROR_CALLOUT_NO_STRING_DELIMITER</c> - Missing terminating delimiter for callout with string argument.
    /// </summary>
    CalloutNoStringDelimiter = PcreConstants.ERROR_CALLOUT_NO_STRING_DELIMITER,

    /// <summary>
    /// <c>PCRE2_ERROR_CALLOUT_BAD_STRING_DELIMITER</c> - Unrecognized string delimiter follows <c>(?C</c>
    /// </summary>
    CalloutBadStringDelimiter = PcreConstants.ERROR_CALLOUT_BAD_STRING_DELIMITER,

    /// <summary>
    /// <c>PCRE2_ERROR_BACKSLASH_C_CALLER_DISABLED</c> - Using <c>\C</c> is disabled by the application.
    /// </summary>
    BackslashCCallerDisabled = PcreConstants.ERROR_BACKSLASH_C_CALLER_DISABLED,

    /// <summary>
    /// <c>PCRE2_ERROR_QUERY_BARJX_NEST_TOO_DEEP</c> - <c>(?|</c> and/or <c>(?J:</c> or <c>(?x:</c> parentheses are too deeply nested.
    /// </summary>
    QueryBarJxNestTooDeep = PcreConstants.ERROR_QUERY_BARJX_NEST_TOO_DEEP,

    /// <summary>
    /// <c>PCRE2_ERROR_BACKSLASH_C_LIBRARY_DISABLED</c> - Using <c>\C</c> is disabled in this PCRE2 library.
    /// </summary>
    BackslashCLibraryDisabled = PcreConstants.ERROR_BACKSLASH_C_LIBRARY_DISABLED,

    /// <summary>
    /// <c>PCRE2_ERROR_PATTERN_TOO_COMPLICATED</c> - Regular expression is too complicated.
    /// </summary>
    PatternTooComplicated = PcreConstants.ERROR_PATTERN_TOO_COMPLICATED,

    /// <summary>
    /// <c>PCRE2_ERROR_LOOKBEHIND_TOO_LONG</c> - Lookbehind assertion is too long.
    /// </summary>
    LookbehindTooLong = PcreConstants.ERROR_LOOKBEHIND_TOO_LONG,

    /// <summary>
    /// <c>PCRE2_ERROR_PATTERN_STRING_TOO_LONG</c> - Pattern string is longer than the limit set by the application.
    /// </summary>
    PatternStringTooLong = PcreConstants.ERROR_PATTERN_STRING_TOO_LONG,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_BAD_CODE</c> - Internal error: unknown code in parsed pattern.
    /// </summary>
    InternalBadCode = PcreConstants.ERROR_INTERNAL_BAD_CODE,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_BAD_CODE_IN_SKIP</c> - Internal error: bad code value in <c>parsed_skip()</c>.
    /// </summary>
    InternalBadCodeInSkip = PcreConstants.ERROR_INTERNAL_BAD_CODE_IN_SKIP,

    /// <summary>
    /// <c>PCRE2_ERROR_NO_SURROGATES_IN_UTF16</c> - <see cref="PcreExtraCompileOptions.AllowSurrogateEscapes"/> is not allowed in UTF-16 mode.
    /// </summary>
    NoSurrogatesInUtf16 = PcreConstants.ERROR_NO_SURROGATES_IN_UTF16,

    /// <summary>
    /// <c>PCRE2_ERROR_BAD_LITERAL_OPTIONS</c> - Invalid option bits with <see cref="PcreOptions.Literal"/>.
    /// </summary>
    BadLiteralOptions = PcreConstants.ERROR_BAD_LITERAL_OPTIONS,

    /// <summary>
    /// <c>PCRE2_ERROR_SUPPORTED_ONLY_IN_UNICODE</c> - <c>\N{U+dddd}</c> is supported only in Unicode (UTF) mode.
    /// </summary>
    SupportedOnlyInUnicode = PcreConstants.ERROR_SUPPORTED_ONLY_IN_UNICODE,

    /// <summary>
    /// <c>PCRE2_ERROR_INVALID_HYPHEN_IN_OPTIONS</c> - Invalid hyphen in option setting.
    /// </summary>
    InvalidHyphenInOptions = PcreConstants.ERROR_INVALID_HYPHEN_IN_OPTIONS,

    /// <summary>
    /// <c>PCRE2_ERROR_ALPHA_ASSERTION_UNKNOWN</c> - <c>(*alpha_assertion)</c> not recognized.
    /// </summary>
    AlphaAssertionUnknown = PcreConstants.ERROR_ALPHA_ASSERTION_UNKNOWN,

    /// <summary>
    /// <c>PCRE2_ERROR_SCRIPT_RUN_NOT_AVAILABLE</c> - Script runs require Unicode support, which this version of PCRE2 does not have.
    /// </summary>
    ScriptRunNotAvailable = PcreConstants.ERROR_SCRIPT_RUN_NOT_AVAILABLE,

    /// <summary>
    /// <c>PCRE2_ERROR_TOO_MANY_CAPTURES</c> - Too many capturing groups (maximum 65535).
    /// </summary>
    TooManyCaptures = PcreConstants.ERROR_TOO_MANY_CAPTURES,

    /// <summary>
    /// <c>PCRE2_ERROR_MISSING_OCTAL_DIGIT</c> - Octal digit missing after <c>\0</c> (<see cref="PcreExtraCompileOptions.NoBs0"/> is set).
    /// </summary>
    MissingOctalDigit = PcreConstants.ERROR_MISSING_OCTAL_DIGIT,

    /// <summary>
    /// <c>PCRE2_ERROR_CONDITION_ATOMIC_ASSERTION_EXPECTED</c> - Atomic assertion expected after <c>(?(</c> or <c>(?(?C)</c>
    /// </summary>
    [Obsolete($"Not used anymore, shares the code with {nameof(MissingOctalDigit)}")]
    ConditionAtomicAssertionExpected = MissingOctalDigit,

    /// <summary>
    /// <c>PCRE2_ERROR_BACKSLASH_K_IN_LOOKAROUND</c> - <c>\K</c> is not allowed in lookarounds (but see <see cref="PcreExtraCompileOptions.AllowLookaroundBsK"/>).
    /// </summary>
    BackslashKInLookaround = PcreConstants.ERROR_BACKSLASH_K_IN_LOOKAROUND,

    /// <summary>
    /// <c>PCRE2_ERROR_MAX_VAR_LOOKBEHIND_EXCEEDED</c> - Branch too long in variable-length lookbehind assertion.
    /// </summary>
    MaxVarLookbehindExceeded = PcreConstants.ERROR_MAX_VAR_LOOKBEHIND_EXCEEDED,

    /// <summary>
    /// <c>PCRE2_ERROR_PATTERN_COMPILED_SIZE_TOO_BIG</c> - Compiled pattern would be longer than the limit set by the application.
    /// </summary>
    PatternCompiledSizeTooBig = PcreConstants.ERROR_PATTERN_COMPILED_SIZE_TOO_BIG,

    /// <summary>
    /// <c>PCRE2_ERROR_OVERSIZE_PYTHON_OCTAL</c> - Octal value given by <c>\ddd</c> is greater than <c>\377</c> (forbidden by <see cref="PcreExtraCompileOptions.PythonOctal"/>).
    /// </summary>
    OversizePythonOctal = PcreConstants.ERROR_OVERSIZE_PYTHON_OCTAL,

    /// <summary>
    /// <c>PCRE2_ERROR_CALLOUT_CALLER_DISABLED</c> - Using callouts is disabled by the application.
    /// </summary>
    CalloutCallerDisabled = PcreConstants.ERROR_CALLOUT_CALLER_DISABLED,

    /// <summary>
    /// <c>PCRE2_ERROR_EXTRA_CASING_REQUIRES_UNICODE</c> - <see cref="PcreExtraCompileOptions.TurkishCasing"/> require Unicode (UTF or UCP) mode.
    /// </summary>
    ExtraCasingRequiresUnicode = PcreConstants.ERROR_EXTRA_CASING_REQUIRES_UNICODE,

    /// <summary>
    /// <c>PCRE2_ERROR_TURKISH_CASING_REQUIRES_UTF</c> - <see cref="PcreExtraCompileOptions.TurkishCasing"/> requires UTF in 8-bit mode.
    /// </summary>
    TurkishCasingRequiresUtf = PcreConstants.ERROR_TURKISH_CASING_REQUIRES_UTF,

    /// <summary>
    /// <c>PCRE2_ERROR_EXTRA_CASING_INCOMPATIBLE</c> - <see cref="PcreExtraCompileOptions.TurkishCasing"/> and <see cref="PcreExtraCompileOptions.CaselessRestrict"/> are not compatible.
    /// </summary>
    ExtraCasingIncompatible = PcreConstants.ERROR_EXTRA_CASING_INCOMPATIBLE,

    /// <summary>
    /// <c>PCRE2_ERROR_ECLASS_NEST_TOO_DEEP</c> - Extended character class nesting is too deep.
    /// </summary>
    EClassNestTooDeep = PcreConstants.ERROR_ECLASS_NEST_TOO_DEEP,

    /// <summary>
    /// <c>PCRE2_ERROR_ECLASS_INVALID_OPERATOR</c> - Invalid operator in extended character class.
    /// </summary>
    EClassInvalidOperator = PcreConstants.ERROR_ECLASS_INVALID_OPERATOR,

    /// <summary>
    /// <c>PCRE2_ERROR_ECLASS_UNEXPECTED_OPERATOR</c> - Unexpected operator in extended character class (no preceding operand).
    /// </summary>
    EClassUnexpectedOperator = PcreConstants.ERROR_ECLASS_UNEXPECTED_OPERATOR,

    /// <summary>
    /// <c>PCRE2_ERROR_ECLASS_EXPECTED_OPERAND</c> - Expected operand after operator in extended character class.
    /// </summary>
    EClassExpectedOperand = PcreConstants.ERROR_ECLASS_EXPECTED_OPERAND,

    /// <summary>
    /// <c>PCRE2_ERROR_ECLASS_MIXED_OPERATORS</c> - Square brackets needed to clarify operator precedence in extended character class.
    /// </summary>
    EClassMixedOperators = PcreConstants.ERROR_ECLASS_MIXED_OPERATORS,

    /// <summary>
    /// <c>PCRE2_ERROR_ECLASS_HINT_SQUARE_BRACKET</c> - Missing terminating <c>]</c> for extended character class (note <c>[</c> must be escaped under <see cref="PcreOptions.AltExtendedClass"/>).
    /// </summary>
    EClassHintSquareBracket = PcreConstants.ERROR_ECLASS_HINT_SQUARE_BRACKET,

    /// <summary>
    /// <c>PCRE2_ERROR_PERL_ECLASS_UNEXPECTED_EXPR</c> - Unexpected expression in extended character class (no preceding operator).
    /// </summary>
    PerlEClassUnexpectedExpr = PcreConstants.ERROR_PERL_ECLASS_UNEXPECTED_EXPR,

    /// <summary>
    /// <c>PCRE2_ERROR_PERL_ECLASS_EMPTY_EXPR</c> - Empty expression in extended character class.
    /// </summary>
    PerlEClassEmptyExpr = PcreConstants.ERROR_PERL_ECLASS_EMPTY_EXPR,

    /// <summary>
    /// <c>PCRE2_ERROR_PERL_ECLASS_MISSING_CLOSE</c> - Terminating <c>]</c> with no following closing parenthesis in <c>(?[...]</c>.
    /// </summary>
    PerlEClassMissingClose = PcreConstants.ERROR_PERL_ECLASS_MISSING_CLOSE,

    /// <summary>
    /// <c>PCRE2_ERROR_PERL_ECLASS_UNEXPECTED_CHAR</c> - Unexpected character in <c>(?[...])</c> extended character class.
    /// </summary>
    PerlEClassUnexpectedChar = PcreConstants.ERROR_PERL_ECLASS_UNEXPECTED_CHAR,

    /// <summary>
    /// <c>PCRE2_ERROR_NOMATCH</c> - No match.
    /// </summary>
    NoMatch = PcreConstants.ERROR_NOMATCH,

    /// <summary>
    /// <c>PCRE2_ERROR_PARTIAL</c> - Partial match.
    /// </summary>
    Partial = PcreConstants.ERROR_PARTIAL,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR1</c> - UTF-8 error: 1 byte missing at end.
    /// </summary>
    Utf8Err1 = PcreConstants.ERROR_UTF8_ERR1,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR2</c> - UTF-8 error: 2 bytes missing at end.
    /// </summary>
    Utf8Err2 = PcreConstants.ERROR_UTF8_ERR2,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR3</c> - UTF-8 error: 3 bytes missing at end.
    /// </summary>
    Utf8Err3 = PcreConstants.ERROR_UTF8_ERR3,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR4</c> - UTF-8 error: 4 bytes missing at end.
    /// </summary>
    Utf8Err4 = PcreConstants.ERROR_UTF8_ERR4,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR5</c> - UTF-8 error: 5 bytes missing at end.
    /// </summary>
    Utf8Err5 = PcreConstants.ERROR_UTF8_ERR5,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR6</c> - UTF-8 error: byte 2 top bits not 0x80.
    /// </summary>
    Utf8Err6 = PcreConstants.ERROR_UTF8_ERR6,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR7</c> - UTF-8 error: byte 3 top bits not 0x80.
    /// </summary>
    Utf8Err7 = PcreConstants.ERROR_UTF8_ERR7,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR8</c> - UTF-8 error: byte 4 top bits not 0x80.
    /// </summary>
    Utf8Err8 = PcreConstants.ERROR_UTF8_ERR8,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR9</c> - UTF-8 error: byte 5 top bits not 0x80.
    /// </summary>
    Utf8Err9 = PcreConstants.ERROR_UTF8_ERR9,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR10</c> - UTF-8 error: byte 6 top bits not 0x80.
    /// </summary>
    Utf8Err10 = PcreConstants.ERROR_UTF8_ERR10,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR11</c> - UTF-8 error: 5-byte character is not allowed (RFC 3629).
    /// </summary>
    Utf8Err11 = PcreConstants.ERROR_UTF8_ERR11,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR12</c> - UTF-8 error: 6-byte character is not allowed (RFC 3629).
    /// </summary>
    Utf8Err12 = PcreConstants.ERROR_UTF8_ERR12,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR13</c> - UTF-8 error: code points greater than 0x10ffff are not defined.
    /// </summary>
    Utf8Err13 = PcreConstants.ERROR_UTF8_ERR13,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR14</c> - UTF-8 error: code points 0xd800-0xdfff are not defined.
    /// </summary>
    Utf8Err14 = PcreConstants.ERROR_UTF8_ERR14,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR15</c> - UTF-8 error: overlong 2-byte sequence.
    /// </summary>
    Utf8Err15 = PcreConstants.ERROR_UTF8_ERR15,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR16</c> - UTF-8 error: overlong 3-byte sequence.
    /// </summary>
    Utf8Err16 = PcreConstants.ERROR_UTF8_ERR16,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR17</c> - UTF-8 error: overlong 4-byte sequence.
    /// </summary>
    Utf8Err17 = PcreConstants.ERROR_UTF8_ERR17,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR18</c> - UTF-8 error: overlong 5-byte sequence.
    /// </summary>
    Utf8Err18 = PcreConstants.ERROR_UTF8_ERR18,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR19</c> - UTF-8 error: overlong 6-byte sequence.
    /// </summary>
    Utf8Err19 = PcreConstants.ERROR_UTF8_ERR19,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR20</c> - UTF-8 error: isolated byte with 0x80 bit set.
    /// </summary>
    Utf8Err20 = PcreConstants.ERROR_UTF8_ERR20,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF8_ERR21</c> - UTF-8 error: illegal byte (0xfe or 0xff).
    /// </summary>
    Utf8Err21 = PcreConstants.ERROR_UTF8_ERR21,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF16_ERR1</c> - UTF-16 error: missing low surrogate at end.
    /// </summary>
    Utf16Err1 = PcreConstants.ERROR_UTF16_ERR1,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF16_ERR2</c> - UTF-16 error: invalid low surrogate.
    /// </summary>
    Utf16Err2 = PcreConstants.ERROR_UTF16_ERR2,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF16_ERR3</c> - UTF-16 error: isolated low surrogate.
    /// </summary>
    Utf16Err3 = PcreConstants.ERROR_UTF16_ERR3,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF32_ERR1</c> - UTF-32 error: code points 0xd800-0xdfff are not defined.
    /// </summary>
    Utf32Err1 = PcreConstants.ERROR_UTF32_ERR1,

    /// <summary>
    /// <c>PCRE2_ERROR_UTF32_ERR2</c> - UTF-32 error: code points greater than 0x10ffff are not defined.
    /// </summary>
    Utf32Err2 = PcreConstants.ERROR_UTF32_ERR2,

    /// <summary>
    /// <c>PCRE2_ERROR_BADDATA</c> - Bad data value.
    /// </summary>
    BadData = PcreConstants.ERROR_BADDATA,

    /// <summary>
    /// <c>PCRE2_ERROR_MIXEDTABLES</c> - Patterns do not all use the same character tables.
    /// </summary>
    MixedTables = PcreConstants.ERROR_MIXEDTABLES,

    /// <summary>
    /// <c>PCRE2_ERROR_BADMAGIC</c> - Magic number missing.
    /// </summary>
    BadMagic = PcreConstants.ERROR_BADMAGIC,

    /// <summary>
    /// <c>PCRE2_ERROR_BADMODE</c> - Pattern compiled in wrong mode: 8/16/32-bit error.
    /// </summary>
    BadMode = PcreConstants.ERROR_BADMODE,

    /// <summary>
    /// <c>PCRE2_ERROR_BADOFFSET</c> - Bad offset value.
    /// </summary>
    BadOffset = PcreConstants.ERROR_BADOFFSET,

    /// <summary>
    /// <c>PCRE2_ERROR_BADOPTION</c> - Bad option value.
    /// </summary>
    BadOption = PcreConstants.ERROR_BADOPTION,

    /// <summary>
    /// <c>PCRE2_ERROR_BADREPLACEMENT</c> - Invalid replacement string.
    /// </summary>
    BadReplacement = PcreConstants.ERROR_BADREPLACEMENT,

    /// <summary>
    /// <c>PCRE2_ERROR_BADUTFOFFSET</c> - Bad offset into UTF string.
    /// </summary>
    BadUtfOffset = PcreConstants.ERROR_BADUTFOFFSET,

    /// <summary>
    /// <c>PCRE2_ERROR_CALLOUT</c> - Callout error code.
    /// </summary>
    Callout = PcreConstants.ERROR_CALLOUT,

    /// <summary>
    /// <c>PCRE2_ERROR_DFA_BADRESTART</c> - Invalid data in workspace for DFA restart.
    /// </summary>
    DfaBadRestart = PcreConstants.ERROR_DFA_BADRESTART,

    /// <summary>
    /// <c>PCRE2_ERROR_DFA_RECURSE</c> - Too much recursion for DFA matching.
    /// </summary>
    DfaRecurse = PcreConstants.ERROR_DFA_RECURSE,

    /// <summary>
    /// <c>PCRE2_ERROR_DFA_UCOND</c> - Backreference condition or recursion test is not supported for DFA matching.
    /// </summary>
    DfaUCond = PcreConstants.ERROR_DFA_UCOND,

    /// <summary>
    /// <c>PCRE2_ERROR_DFA_UFUNC</c> - Function is not supported for DFA matching.
    /// </summary>
    DfaUFunc = PcreConstants.ERROR_DFA_UFUNC,

    /// <summary>
    /// <c>PCRE2_ERROR_DFA_UITEM</c> - Pattern contains an item that is not supported for DFA matching.
    /// </summary>
    DfaUItem = PcreConstants.ERROR_DFA_UITEM,

    /// <summary>
    /// <c>PCRE2_ERROR_DFA_WSSIZE</c> - Workspace size exceeded in DFA matching.
    /// </summary>
    DfaWsSize = PcreConstants.ERROR_DFA_WSSIZE,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL</c> - Internal error - pattern overwritten?
    /// </summary>
    Internal = PcreConstants.ERROR_INTERNAL,

    /// <summary>
    /// <c>PCRE2_ERROR_JIT_BADOPTION</c> - Bad JIT option.
    /// </summary>
    JitBadOption = PcreConstants.ERROR_JIT_BADOPTION,

    /// <summary>
    /// <c>PCRE2_ERROR_JIT_STACKLIMIT</c> - JIT stack limit reached.
    /// </summary>
    JitStackLimit = PcreConstants.ERROR_JIT_STACKLIMIT,

    /// <summary>
    /// <c>PCRE2_ERROR_MATCHLIMIT</c> - Match limit exceeded.
    /// </summary>
    MatchLimit = PcreConstants.ERROR_MATCHLIMIT,

    /// <summary>
    /// <c>PCRE2_ERROR_NOMEMORY</c> - No more memory.
    /// </summary>
    NoMemory = PcreConstants.ERROR_NOMEMORY,

    /// <summary>
    /// <c>PCRE2_ERROR_NOSUBSTRING</c> - Unknown substring.
    /// </summary>
    NoSubstring = PcreConstants.ERROR_NOSUBSTRING,

    /// <summary>
    /// <c>PCRE2_ERROR_NOUNIQUESUBSTRING</c> - Non-unique substring name.
    /// </summary>
    NoUniqueSubstring = PcreConstants.ERROR_NOUNIQUESUBSTRING,

    /// <summary>
    /// <c>PCRE2_ERROR_NULL</c> - NULL argument passed.
    /// </summary>
    Null = PcreConstants.ERROR_NULL,

    /// <summary>
    /// <c>PCRE2_ERROR_RECURSELOOP</c> - Nested recursion at the same subject position.
    /// </summary>
    RecurseLoop = PcreConstants.ERROR_RECURSELOOP,

    /// <summary>
    /// <c>PCRE2_ERROR_DEPTHLIMIT</c> - Matching depth limit exceeded.
    /// </summary>
    DepthLimit = PcreConstants.ERROR_DEPTHLIMIT,

    /// <summary>
    /// <c>PCRE2_ERROR_UNAVAILABLE</c> - Requested value is not available.
    /// </summary>
    Unavailable = PcreConstants.ERROR_UNAVAILABLE,

    /// <summary>
    /// <c>PCRE2_ERROR_UNSET</c> - Requested value is not set.
    /// </summary>
    Unset = PcreConstants.ERROR_UNSET,

    /// <summary>
    /// <c>PCRE2_ERROR_BADOFFSETLIMIT</c> - Offset limit set without <see cref="PcreOptions.UseOffsetLimit"/>
    /// </summary>
    BadOffsetLimit = PcreConstants.ERROR_BADOFFSETLIMIT,

    /// <summary>
    /// <c>PCRE2_ERROR_BADREPESCAPE</c> - Bad escape sequence in replacement string.
    /// </summary>
    BadRepEscape = PcreConstants.ERROR_BADREPESCAPE,

    /// <summary>
    /// <c>PCRE2_ERROR_REPMISSINGBRACE</c> - Expected closing curly bracket in replacement string.
    /// </summary>
    RepMissingBrace = PcreConstants.ERROR_REPMISSINGBRACE,

    /// <summary>
    /// <c>PCRE2_ERROR_BADSUBSTITUTION</c> - Bad substitution in replacement string.
    /// </summary>
    BadSubstitution = PcreConstants.ERROR_BADSUBSTITUTION,

    /// <summary>
    /// <c>PCRE2_ERROR_BADSUBSPATTERN</c> - Match with end before start or start moved backwards is not supported.
    /// </summary>
    BadSubsPattern = PcreConstants.ERROR_BADSUBSPATTERN,

    /// <summary>
    /// <c>PCRE2_ERROR_TOOMANYREPLACE</c> - Too many replacements (more than <see cref="int.MaxValue"/>)
    /// </summary>
    TooManyReplace = PcreConstants.ERROR_TOOMANYREPLACE,

    /// <summary>
    /// <c>PCRE2_ERROR_BADSERIALIZEDDATA</c> - Bad serialized data.
    /// </summary>
    BadSerializedData = PcreConstants.ERROR_BADSERIALIZEDDATA,

    /// <summary>
    /// <c>PCRE2_ERROR_HEAPLIMIT</c> - Heap limit exceeded.
    /// </summary>
    HeapLimit = PcreConstants.ERROR_HEAPLIMIT,

    /// <summary>
    /// <c>PCRE2_ERROR_CONVERT_SYNTAX</c> - Invalid syntax.
    /// </summary>
    ConvertSyntax = PcreConstants.ERROR_CONVERT_SYNTAX,

    /// <summary>
    /// <c>PCRE2_ERROR_INTERNAL_DUPMATCH</c> - Internal error - duplicate substitution match.
    /// </summary>
    InternalDupMatch = PcreConstants.ERROR_INTERNAL_DUPMATCH,

    /// <summary>
    /// <c>PCRE2_ERROR_DFA_UINVALID_UTF</c> - <see cref="PcreOptions.MatchInvalidUtf"/> is not supported for DFA matching.
    /// </summary>
    DfaUInvalidUtf = PcreConstants.ERROR_DFA_UINVALID_UTF,

    /// <summary>
    /// <c>PCRE2_ERROR_INVALIDOFFSET</c> - Invalid offset value.
    /// </summary>
    InvalidOffset = PcreConstants.ERROR_INVALIDOFFSET,

    ///<summary>
    ///<c>PCRE2_ERROR_JIT_UNSUPPORTED</c> - Feature is not supported by the JIT compiler.
    ///</summary>
    JitUnsupported = PcreConstants.ERROR_JIT_UNSUPPORTED,

    ///<summary>
    ///<c>PCRE2_ERROR_REPLACECASE</c> - Error performing replacement case transformation.
    ///</summary>
    ReplaceCase = PcreConstants.ERROR_REPLACECASE,

    ///<summary>
    ///<c>PCRE2_ERROR_TOOLARGEREPLACE</c> - Replacement too large (longer than <c>PCRE2_SIZE</c>).
    ///</summary>
    TooLargeReplace = PcreConstants.ERROR_TOOLARGEREPLACE,
}
