using System.Runtime.InteropServices;
using PCRE.Internal;

namespace PCRE.Conversion;

/// <summary>
/// Conversion options for glob patterns.
/// </summary>
public sealed class PcreGlobConversionOptions
{
    /// <summary>
    /// <c>PCRE2_CONVERT_GLOB_NO_WILD_SEPARATOR</c> - Allow wildcards to match separator characters.
    /// </summary>
    public bool NoWildcardSeparator { get; set; }

    /// <summary>
    /// <c>PCRE2_CONVERT_GLOB_NO_STARSTAR</c> - Disable the double-star (<c>**</c>) wildcard.
    /// </summary>
    public bool NoStarStar { get; set; }

    /// <summary>
    /// The escape character.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>`</c> under Windows, and <c>\</c> otherwise.
    /// </remarks>
    public char EscapeCharacter { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? '`' : '\\';

    /// <summary>
    /// The path separator character.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>\</c> under Windows, and <c>/</c> otherwise.
    /// </remarks>
    public char SeparatorCharacter { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? '\\' : '/';

    /// <summary>
    /// Returns default options for Windows.
    /// </summary>
    public static PcreGlobConversionOptions DefaultWindows() => new()
    {
        EscapeCharacter = '`',
        SeparatorCharacter = '\\'
    };

    /// <summary>
    /// Returns default options for Unix.
    /// </summary>
    public static PcreGlobConversionOptions DefaultUnix() => new()
    {
        EscapeCharacter = '\\',
        SeparatorCharacter = '/'
    };

    internal void FillConvertInput(ref Native.convert_input input)
    {
        input.options = GetConvertOptions();
        input.glob_escape = EscapeCharacter;
        input.glob_separator = SeparatorCharacter;
    }

    private uint GetConvertOptions()
    {
        var options = PcreConstants.PCRE2_CONVERT_GLOB;

        if (NoWildcardSeparator)
            options |= PcreConstants.PCRE2_CONVERT_GLOB_NO_WILD_SEPARATOR;

        if (NoStarStar)
            options |= PcreConstants.PCRE2_CONVERT_GLOB_NO_STARSTAR;

        return options;
    }
}
