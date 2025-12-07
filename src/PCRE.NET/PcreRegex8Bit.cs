using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// A PCRE regular expression for the 8-bit PCRE2 library.
/// </summary>
/// <seealso cref="PcreRegex8Bit"/>
public partial class PcreRegex8Bit
{
    private static PcreRegexSettings DefaultSettings { get; } = new PcreRegexSettings().ToReadOnlySnapshot(PcreOptions.None);

    internal InternalRegex8Bit InternalRegex { get; }

    /// <summary>
    /// The encoding used to retrieve information about the pattern, such as the capture group names.
    /// </summary>
    public Encoding Encoding => InternalRegex.Encoding;

    /// <summary>
    /// Creates a PCRE2 regex for the 8-bit PCRE2 library.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="encoding">The pattern encoding.</param>
    [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
    public PcreRegex8Bit(ReadOnlySpan<byte> pattern, Encoding encoding)
        : this(pattern, encoding, DefaultSettings)
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for the 8-bit PCRE2 library.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="encoding">The pattern encoding.</param>
    /// <param name="options">Pattern options.</param>
    public PcreRegex8Bit(ReadOnlySpan<byte> pattern, Encoding encoding, PcreOptions options)
        : this(pattern, encoding, OptionsToSettings(options))
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for the 8-bit PCRE2 library.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="encoding">The pattern encoding.</param>
    /// <param name="settings">Additional advanced settings.</param>
    public PcreRegex8Bit(ReadOnlySpan<byte> pattern, Encoding encoding, PcreRegexSettings settings)
    {
        if (settings is null)
            throw new ArgumentNullException(nameof(settings));

        if (encoding is null)
            throw new ArgumentNullException(nameof(encoding));

        InternalRegex = new InternalRegex8Bit(
            pattern,
            InternalRegex8Bit.GetString(pattern, encoding),
            settings.ToReadOnlySnapshot(PcreOptions.None),
            encoding
        );
    }

    private protected PcreRegex8Bit(InternalRegex8Bit internalRegex)
    {
        InternalRegex = internalRegex;
    }

    private static PcreRegexSettings OptionsToSettings(PcreOptions options)
        => options is PcreOptions.None ? DefaultSettings : new PcreRegexSettings(options);
}
