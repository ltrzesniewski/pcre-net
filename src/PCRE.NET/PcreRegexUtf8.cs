using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// A PCRE regular expression for UTF-8.
/// </summary>
/// <seealso cref="PcreRegex8Bit"/>
public sealed partial class PcreRegexUtf8
{
    internal InternalRegex8Bit InternalRegex { get; }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
    public PcreRegexUtf8(ReadOnlySpan<byte> pattern)
        : this(pattern, GetString(pattern), new PcreRegexSettings(PcreOptions.None))
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
    public PcreRegexUtf8(string pattern)
        : this(GetBytes(pattern), pattern, new PcreRegexSettings(PcreOptions.None))
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="options">Pattern options.</param>
    public PcreRegexUtf8(ReadOnlySpan<byte> pattern, PcreOptions options)
        : this(pattern, GetString(pattern), new PcreRegexSettings(options))
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="options">Pattern options.</param>
    public PcreRegexUtf8(string pattern, PcreOptions options)
        : this(GetBytes(pattern), pattern, new PcreRegexSettings(options))
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="settings">Additional advanced settings.</param>
    public PcreRegexUtf8(ReadOnlySpan<byte> pattern, PcreRegexSettings settings)
        : this(pattern, GetString(pattern), settings)
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="settings">Additional advanced settings.</param>
    public PcreRegexUtf8(string pattern, PcreRegexSettings settings)
        : this(GetBytes(pattern), pattern, settings)
    {
    }

    private PcreRegexUtf8(ReadOnlySpan<byte> pattern, string patternString, PcreRegexSettings settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        InternalRegex = new InternalRegex8Bit(pattern, patternString, settings, Encoding.UTF8);
    }

    private static ReadOnlySpan<byte> GetBytes(string value)
        => Encoding.UTF8.GetBytes(value);

    internal static unsafe string GetString(ReadOnlySpan<byte> value)
        => InternalRegex8Bit.GetString(value, Encoding.UTF8);
}
