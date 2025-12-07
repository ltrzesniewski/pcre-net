using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// The result of a capturing group.
/// </summary>
[ForwardTo8Bit]
[DebuggerTypeProxy(typeof(DebugProxy))]
public readonly ref struct PcreRefGroup
{
    [SuppressMessage("ReSharper", "ReplaceWithFieldKeyword")]
    private readonly ReadOnlySpan<char> _subject;

    // Indices are offset by 1. 0 means undefined group. -1 means empty group.
    private readonly int _indexWithOffset;
    private readonly int _endIndexWithOffset;

    /// <summary>
    /// A function which returns an output value out of a group.
    /// </summary>
    /// <typeparam name="T">The output value type.</typeparam>
    public delegate T Func<out T>(PcreRefGroup group);

    [ForwardTo8Bit]
    internal static PcreRefGroup Empty => new(-1);

    [ForwardTo8Bit]
    internal static PcreRefGroup Undefined => default;

    internal PcreRefGroup(ReadOnlySpan<char> subject, int startOffset, int endOffset)
    {
        _subject = subject;
        _indexWithOffset = startOffset >= 0 ? startOffset + 1 : -1;
        _endIndexWithOffset = endOffset >= 0 ? endOffset + 1 : -1;
    }

    [ForwardTo8Bit]
    private PcreRefGroup(int flag)
    {
        _subject = default;
        _indexWithOffset = _endIndexWithOffset = flag;
    }

    /// <inheritdoc cref="PcreGroup.Index"/>
    [ForwardTo8Bit]
    public int Index => _indexWithOffset > 0 ? _indexWithOffset - 1 : -1;

    /// <inheritdoc cref="PcreGroup.EndIndex"/>
    [ForwardTo8Bit]
    public int EndIndex => _endIndexWithOffset > 0 ? _endIndexWithOffset - 1 : -1;

    /// <inheritdoc cref="PcreGroup.Length"/>
    [ForwardTo8Bit]
    public int Length => _endIndexWithOffset > _indexWithOffset ? _endIndexWithOffset - _indexWithOffset : 0;

    /// <inheritdoc cref="PcreGroup.Value"/>
    [ForwardTo8Bit]
    public ReadOnlySpan<char> Value => _indexWithOffset > 0 ? _subject.Slice(_indexWithOffset - 1, Length) : default;

    /// <inheritdoc cref="PcreGroup.Success"/>
    [ForwardTo8Bit]
    public bool Success => _indexWithOffset > 0;

    /// <inheritdoc cref="PcreGroup.IsDefined"/>
    [ForwardTo8Bit]
    public bool IsDefined => _indexWithOffset != 0;

    /// <inheritdoc cref="PcreGroup.op_Implicit"/>
    public static implicit operator string(PcreRefGroup group)
        => group.Value.ToString();

    /// <inheritdoc cref="PcreGroup.ToString"/>
    public override string ToString()
        => Value.ToString();

    internal class DebugProxy
    {
        public bool Success { get; }
        public string? Value { get; }

        public DebugProxy(PcreRefGroup group)
        {
            Success = group.Success;
            Value = Success ? group.Value.ToString() : null;
        }

        public override string ToString()
            => Value ?? "<no match>";
    }
}
