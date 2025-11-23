using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// The result of a capturing group.
/// </summary>
public readonly ref partial struct PcreRefGroup8Bit
{
    [SuppressMessage("ReSharper", "ReplaceWithFieldKeyword")]
    private readonly ReadOnlySpan<byte> _subject;

    private readonly InternalRegex8Bit? _regex;

    // Indices are offset by 1. 0 means undefined group. -1 means empty group.
    private readonly int _indexWithOffset;
    private readonly int _endIndexWithOffset;

    /// <summary>
    /// A function which returns an output value out of a group.
    /// </summary>
    /// <typeparam name="T">The output value type.</typeparam>
    public delegate T Func<out T>(PcreRefGroup8Bit group);

    internal PcreRefGroup8Bit(ReadOnlySpan<byte> subject, InternalRegex8Bit? regex, int startOffset, int endOffset)
    {
        _subject = subject;
        _regex = regex;
        _indexWithOffset = startOffset >= 0 ? startOffset + 1 : -1;
        _endIndexWithOffset = endOffset >= 0 ? endOffset + 1 : -1;
    }

    /// <inheritdoc cref="PcreGroup.op_Implicit"/>
    public static explicit operator string(PcreRefGroup8Bit group)
        => group.ToString();

    /// <inheritdoc cref="PcreGroup.ToString"/>
    public override string ToString()
        => _regex?.GetString(Value) ?? string.Empty;
}
