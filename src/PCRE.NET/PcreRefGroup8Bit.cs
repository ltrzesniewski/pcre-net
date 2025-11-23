using System;
using System.Diagnostics.CodeAnalysis;

namespace PCRE;

/// <summary>
/// The result of a capturing group.
/// </summary>
public readonly ref partial struct PcreRefGroup8Bit
{
    [SuppressMessage("ReSharper", "ReplaceWithFieldKeyword")]
    private readonly ReadOnlySpan<byte> _subject;

    // Indices are offset by 1. 0 means undefined group. -1 means empty group.
    private readonly int _indexWithOffset;
    private readonly int _endIndexWithOffset;

    /// <summary>
    /// A function which returns an output value out of a group.
    /// </summary>
    /// <typeparam name="T">The output value type.</typeparam>
    public delegate T Func<out T>(PcreRefGroup8Bit group);

    /// <inheritdoc cref="PcreGroup.ToString"/>
    public override string ToString()
        => string.Empty; // TODO: Improve this?
}
