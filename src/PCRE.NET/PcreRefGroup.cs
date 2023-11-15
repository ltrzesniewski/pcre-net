using System;
using System.Diagnostics;

namespace PCRE;

/// <summary>
/// The result of a capturing group.
/// </summary>
[DebuggerTypeProxy(typeof(DebugProxy))]
public readonly ref struct PcreRefGroup
{
    private readonly ReadOnlySpan<char> _subject;

    // Indices are offset by 1. 0 means undefined group. -1 means empty group.
    private readonly int _indexWithOffset;
    private readonly int _endIndexWithOffset;

    /// <summary>
    /// A function which returns an output value out of a group.
    /// </summary>
    /// <typeparam name="T">The output value type.</typeparam>
    public delegate T Func<out T>(PcreRefGroup group);

    internal static PcreRefGroup Empty => new(-1);
    internal static PcreRefGroup Undefined => default;

    internal PcreRefGroup(ReadOnlySpan<char> subject, int startOffset, int endOffset)
    {
        _subject = subject;
        _indexWithOffset = startOffset >= 0 ? startOffset + 1 : -1;
        _endIndexWithOffset = endOffset >= 0 ? endOffset + 1 : -1;
    }

    private PcreRefGroup(int flag)
    {
        _subject = default;
        _indexWithOffset = _endIndexWithOffset = flag;
    }

    /// <inheritdoc cref="PcreGroup.Index"/>
    public int Index => _indexWithOffset > 0 ? _indexWithOffset - 1 : -1;

    /// <inheritdoc cref="PcreGroup.EndIndex"/>
    public int EndIndex => _endIndexWithOffset > 0 ? _endIndexWithOffset - 1 : -1;

    /// <inheritdoc cref="PcreGroup.Length"/>
    public int Length => _endIndexWithOffset > _indexWithOffset ? _endIndexWithOffset - _indexWithOffset : 0;

    /// <inheritdoc cref="PcreGroup.Value"/>
    public ReadOnlySpan<char> Value => _indexWithOffset > 0 ? _subject.Slice(_indexWithOffset - 1, Length) : default;

    /// <inheritdoc cref="PcreGroup.Success"/>
    public bool Success => _indexWithOffset > 0;

    /// <inheritdoc cref="PcreGroup.IsDefined"/>
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
