using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// A PCRE regular expression for UTF-8.
/// </summary>
/// <remarks>
/// This inherits from <see cref="PcreRegex8Bit"/> just for convenience.
/// It forces the encoding to UTF-8 and adds the <see cref="PcreOptions.Utf"/> option.
/// </remarks>
/// <seealso cref="PcreRegex8Bit"/>
public sealed class PcreRegexUtf8 : PcreRegex8Bit
{
    private const PcreOptions _additionalOptions = PcreOptions.Utf;
    private static PcreRegexSettings DefaultSettings { get; } = new PcreRegexSettings().ToReadOnlySnapshot(_additionalOptions);

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
    public PcreRegexUtf8(ReadOnlySpan<byte> pattern)
        : this(pattern, GetString(pattern), DefaultSettings)
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
    public PcreRegexUtf8(string pattern)
        : this(GetBytes(pattern), pattern, DefaultSettings)
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="options">Pattern options.</param>
    public PcreRegexUtf8(ReadOnlySpan<byte> pattern, PcreOptions options)
        : this(pattern, GetString(pattern), OptionsToSettings(options))
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="options">Pattern options.</param>
    public PcreRegexUtf8(string pattern, PcreOptions options)
        : this(GetBytes(pattern), pattern, OptionsToSettings(options))
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
        : base(CreateRegex(pattern, patternString, settings))
    {
    }

    private static InternalRegex8Bit CreateRegex(ReadOnlySpan<byte> pattern, string patternString, PcreRegexSettings settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        return new InternalRegex8Bit(
            pattern,
            patternString,
            settings.ToReadOnlySnapshot(_additionalOptions),
            Encoding.UTF8
        );
    }

    /// <summary>
    /// Converts options to settings to avoid allocating settings for default options. Settings will be made read-only later.
    /// </summary>
    private static PcreRegexSettings OptionsToSettings(PcreOptions options)
        => options is PcreOptions.None or _additionalOptions ? DefaultSettings : new PcreRegexSettings(options | _additionalOptions);

    private static ReadOnlySpan<byte> GetBytes(string value)
        => Encoding.UTF8.GetBytes(value);

    private static string GetString(ReadOnlySpan<byte> value)
        => InternalRegex8Bit.GetString(value, Encoding.UTF8);
}
