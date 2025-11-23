using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// The result of a match.
/// </summary>
[DebuggerTypeProxy(typeof(DebugProxy))]
public unsafe ref partial struct PcreRefMatch8Bit
{
    private readonly IRegexHolder8Bit? _owner; // Needs to be kept alive as long as this match is used
    internal Span<nuint> OutputVector; // Can be empty when there is no match
    private int _resultCode;
    private byte* _markPtr;

    internal ReadOnlySpan<byte> Subject;

    /// <summary>
    /// A function which returns an output value out of a match.
    /// </summary>
    /// <typeparam name="T">The output value type.</typeparam>
    public delegate T Func<out T>(PcreRefMatch8Bit match);

    /// <inheritdoc cref="PcreMatch.ToString"/>
    public readonly override string ToString()
    {
        GC.KeepAlive(_owner); // Good enough place to keep the object alive, I suppose.
        return _owner?.Regex?.GetString(Value) ?? string.Empty;
    }

    /// <summary>
    /// The list of groups in a match.
    /// </summary>
    public readonly ref partial struct GroupList
    {
        private readonly PcreRefMatch8Bit _match;
    }

    /// <summary>
    /// A capturing group enumerator.
    /// </summary>
    public ref partial struct GroupEnumerator
    {
        private readonly PcreRefMatch8Bit _match;
        private int _index;
    }

    /// <summary>
    /// An enumerable of duplicated named groups.
    /// </summary>
    public readonly ref partial struct DuplicateNamedGroupEnumerable
    {
        private readonly PcreRefMatch8Bit _match;
        private readonly int[]? _groupIndexes;
    }

    /// <summary>
    /// An enumerator of duplicated named groups.
    /// </summary>
    public ref partial struct DuplicateNamedGroupEnumerator
    {
        private readonly PcreRefMatch8Bit _match;
        private readonly int[]? _groupIndexes;
        private int _index;
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal class DebugProxy
    {
        public bool Success { get; }
        public string? Value { get; }

        public DebugProxy(PcreRefMatch8Bit match)
        {
            Success = match.Success;
            Value = Success ? match._owner?.Regex.GetString(match.Value) : null;
        }

        public override string ToString()
            => Value ?? "<no match>";
    }
}
