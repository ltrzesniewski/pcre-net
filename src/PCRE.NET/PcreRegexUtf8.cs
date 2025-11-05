using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// A PCRE regular expression for UTF-8.
/// </summary>
public sealed partial class PcreRegexUtf8
{
    internal InternalRegex8Bit InternalRegex { get; }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    [SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
    public PcreRegexUtf8(ReadOnlySpan<byte> pattern)
        : this(pattern, PcreOptions.None)
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="options">Pattern options.</param>
    public PcreRegexUtf8(ReadOnlySpan<byte> pattern, PcreOptions options)
        : this(pattern, new PcreRegexSettings(options))
    {
    }

    /// <summary>
    /// Creates a PCRE2 regex for UTF-8.
    /// </summary>
    /// <param name="pattern">The regular expression pattern.</param>
    /// <param name="settings">Additional advanced settings.</param>
    public PcreRegexUtf8(ReadOnlySpan<byte> pattern, PcreRegexSettings settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        // TODO: InternalRegex = Caches.RegexCache.GetOrAdd(new RegexKey(pattern, settings));
        InternalRegex = new InternalRegex8Bit(pattern, settings);
    }

    /// <summary>
    /// Returns the regex pattern.
    /// </summary>
    public override string ToString()
        => InternalRegex.PatternString;
}
