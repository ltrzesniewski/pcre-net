using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PCRE;

/// <summary>
/// The result of a capturing group.
/// </summary>
[DebuggerTypeProxy(typeof(DebugProxy))]
public readonly ref partial struct PcreRefGroupUtf8
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
    public delegate T Func<out T>(PcreRefGroupUtf8 group);

    internal PcreRefGroupUtf8(ReadOnlySpan<byte> subject, int startOffset, int endOffset)
    {
        _subject = subject;
        _indexWithOffset = startOffset >= 0 ? startOffset + 1 : -1;
        _endIndexWithOffset = endOffset >= 0 ? endOffset + 1 : -1;
    }

    /// <inheritdoc cref="PcreGroup.op_Implicit"/>
    public static explicit operator string(PcreRefGroupUtf8 group)
        => PcreRegexUtf8.GetString(group.Value);

    /// <inheritdoc cref="PcreGroup.ToString"/>
    public override string ToString()
        => PcreRegexUtf8.GetString(Value);

    internal class DebugProxy
    {
        public bool Success { get; }
        public string? Value { get; }

        public DebugProxy(PcreRefGroupUtf8 group)
        {
            Success = group.Success;
            Value = Success ? PcreRegexUtf8.GetString(group.Value) : null;
        }

        public override string ToString()
            => Value ?? "<no match>";
    }
}
