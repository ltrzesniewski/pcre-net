using PCRE.Internal;

namespace PCRE;

/// <summary>
/// Returns information about the PCRE build.
/// </summary>
public static unsafe class PcreBuildInfo
{
    /// <summary>
    /// <c>PCRE2_CONFIG_BSR</c> - Indicates what character sequences the <c>\R</c> escape sequence matches by default.
    /// </summary>
    /// <remarks>
    /// The default can be overridden when a pattern is compiled.
    /// </remarks>
    public static PcreBackslashR BackslashR { get; } = (PcreBackslashR)GetConfigUInt32(PcreConstants.CONFIG_BSR);

    /// <summary>
    /// <c>PCRE2_CONFIG_JIT</c> - Indicates if support for just-in-time compiling is available.
    /// </summary>
    public static bool Jit { get; } = GetConfigBool(PcreConstants.CONFIG_JIT);

    /// <summary>
    /// <c>PCRE2_CONFIG_JITTARGET</c> - Returns a string that contains the name of the architecture for which the JIT compiler is configured.
    /// </summary>
    public static string JitTarget { get; } = GetConfigString(PcreConstants.CONFIG_JITTARGET);

    /// <summary>
    /// <c>PCRE2_CONFIG_LINKSIZE</c> - Indicates the number of bytes used for internal linkage in compiled regular expressions.
    /// </summary>
    public static uint LinkSize { get; } = GetConfigUInt32(PcreConstants.CONFIG_LINKSIZE);

    /// <summary>
    /// <c>PCRE2_CONFIG_MATCHLIMIT</c> - Returns the default match limit.
    /// </summary>
    public static uint MatchLimit { get; } = GetConfigUInt32(PcreConstants.CONFIG_MATCHLIMIT);

    /// <summary>
    /// <c>PCRE2_CONFIG_NEWLINE</c> - Indicates the default character sequence that is recognized as meaning "newline".
    /// </summary>
    public static PcreNewLine NewLine { get; } = (PcreNewLine)GetConfigUInt32(PcreConstants.CONFIG_NEWLINE);

    /// <summary>
    /// <c>PCRE2_CONFIG_PARENSLIMIT</c> - Returns the maximum depth of nesting of parentheses (of any kind) in a pattern.
    /// </summary>
    /// <remarks>
    /// This limit is imposed to cap the amount of system stack used when a pattern is compiled.
    /// </remarks>
    public static uint ParensLimit { get; } = GetConfigUInt32(PcreConstants.CONFIG_PARENSLIMIT);

    /// <summary>
    /// <c>PCRE2_CONFIG_DEPTHLIMIT</c> - Returns the default limit for the depth of nested backtracking in NFA matching or the depth of nested recursions, lookarounds, and atomic groups in DFA matching.
    /// </summary>
    public static uint DepthLimit { get; } = GetConfigUInt32(PcreConstants.CONFIG_DEPTHLIMIT);

    /// <summary>
    /// <c>PCRE2_CONFIG_UNICODE</c> - Indicates if Unicode support is available.
    /// </summary>
    public static bool Unicode { get; } = GetConfigBool(PcreConstants.CONFIG_UNICODE);

    /// <summary>
    /// <c>PCRE2_CONFIG_UNICODE_VERSION</c> - Returns the supported Unicode version.
    /// </summary>
    public static string UnicodeVersion { get; } = GetConfigString(PcreConstants.CONFIG_UNICODE_VERSION);

    /// <summary>
    /// <c>PCRE2_CONFIG_VERSION</c> - Returns the PCRE2 version string.
    /// </summary>
    public static string Version { get; } = GetConfigString(PcreConstants.CONFIG_VERSION);

    /// <summary>
    /// <c>PCRE2_CONFIG_HEAPLIMIT</c> - Returns the default limit for the amount of heap memory used for matching, in kibibytes.
    /// </summary>
    public static uint HeapLimit { get; } = GetConfigUInt32(PcreConstants.CONFIG_HEAPLIMIT);

    /// <summary>
    /// <c>PCRE2_CONFIG_NEVER_BACKSLASH_C</c> - Indicates if the use of <c>\C</c> was permanently disabled when PCRE2 was built
    /// </summary>
    public static bool NeverBackslashC { get; } = GetConfigBool(PcreConstants.CONFIG_NEVER_BACKSLASH_C);

    /// <summary>
    /// <c>PCRE2_CONFIG_COMPILED_WIDTHS</c> - Indicates which code unit widths were selected when PCRE2 was built.
    /// </summary>
    /// <remarks>
    /// The 1-bit indicates 8-bit support, and the 2-bit and 4-bit indicate 16-bit and 32-bit support, respectively.
    /// </remarks>
    public static uint CompiledWidths { get; } = GetConfigUInt32(PcreConstants.CONFIG_COMPILED_WIDTHS);

    /// <summary>
    /// <c>PCRE2_CONFIG_TABLES_LENGTH</c> - Gives the length of PCRE2's character processing tables in bytes.
    /// </summary>
    public static uint TablesLength { get; } = GetConfigUInt32(PcreConstants.CONFIG_TABLES_LENGTH);

    /// <summary>
    /// <c>CONFIG_EFFECTIVE_LINKSIZE</c> - Indicates the number of bytes the library uses for internal linkage in compiled regular expressions.
    /// </summary>
    public static uint EffectiveLinkSize { get; } = GetConfigUInt32(PcreConstants.CONFIG_EFFECTIVE_LINKSIZE);

    private static bool GetConfigBool(uint key)
        => GetConfigUInt32(key) != 0;

    private static uint GetConfigUInt32(uint key)
    {
        uint result;
        var size = Native.config(key, &result);

        if (size < 0)
            throw new PcreException((PcreErrorCode)size, "Could not retrieve the configuration property: " + key);

        return result;
    }

    private static string GetConfigString(uint key)
    {
        var buffer = stackalloc char[256];
        var messageLength = Native.config(key, buffer);

        return messageLength >= 0
            ? new string(buffer, 0, messageLength - 1)
            : throw new PcreException((PcreErrorCode)messageLength, "Could not retrieve the configuration property: " + key);
    }
}
