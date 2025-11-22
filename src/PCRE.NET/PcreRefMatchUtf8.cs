using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// The result of a match.
/// </summary>
[DebuggerTypeProxy(typeof(DebugProxy))]
public unsafe ref partial struct PcreRefMatchUtf8
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
    public delegate T Func<out T>(PcreRefMatchUtf8 match);

    internal PcreRefMatchUtf8(IRegexHolder8Bit owner, Span<nuint> oVector)
    {
        // Empty match

        Debug.Assert(oVector.Length == 0 || oVector.Length == owner.Regex.OutputVectorSize);

        _owner = owner;
        OutputVector = oVector;

        Subject = default;
        _markPtr = null;
        _resultCode = 0;
    }

    internal PcreRefMatchUtf8(ReadOnlySpan<byte> subject, InternalRegex8Bit regex, Span<nuint> oVector, byte* mark)
    {
        // Callout

        Subject = subject;
        _owner = regex;
        OutputVector = oVector;
        _markPtr = mark;

        _resultCode = oVector.Length / 2;
    }

    private readonly InternalRegex8Bit? Regex => _owner?.Regex;

    internal readonly bool IsInitialized => _owner != null;

    /// <inheritdoc cref="PcreMatch.CaptureCount"/>
    public readonly int CaptureCount => Regex?.CaptureCount ?? 0;

    /// <summary>
    /// Returns the capturing group at a given index.
    /// </summary>
    /// <param name="index">The index of the capturing group.</param>
    public readonly PcreRefGroupUtf8 this[int index]
        => GetGroup(index);

    /// <summary>
    /// Returns the capturing group of a given name.
    /// </summary>
    /// <param name="name">The name of the capturing group.</param>
    public readonly PcreRefGroupUtf8 this[string name]
        => GetGroup(name);

    /// <inheritdoc cref="PcreMatch.ToString"/>
    public readonly override string ToString()
    {
        GC.KeepAlive(_owner); // Good enough place to keep the object alive, I suppose.
        return PcreRegexUtf8.GetString(Value);
    }

    /// <summary>
    /// The list of groups in a match.
    /// </summary>
    public readonly ref partial struct GroupList
    {
        private readonly PcreRefMatchUtf8 _match;

        internal GroupList(PcreRefMatchUtf8 match)
            => _match = match;
    }

    /// <summary>
    /// A capturing group enumerator.
    /// </summary>
    public ref partial struct GroupEnumerator
    {
        private readonly PcreRefMatchUtf8 _match;
        private int _index;

        internal GroupEnumerator(PcreRefMatchUtf8 match)
        {
            _match = match;
            _index = -1;
        }
    }

    /// <summary>
    /// An enumerable of duplicated named groups.
    /// </summary>
    public readonly ref partial struct DuplicateNamedGroupEnumerable
    {
        private readonly PcreRefMatchUtf8 _match;
        private readonly int[]? _groupIndexes;

        internal DuplicateNamedGroupEnumerable(PcreRefMatchUtf8 match, int[] groupIndexes)
        {
            _match = match;
            _groupIndexes = groupIndexes;
        }
    }

    /// <summary>
    /// An enumerator of duplicated named groups.
    /// </summary>
    public ref partial struct DuplicateNamedGroupEnumerator
    {
        private readonly PcreRefMatchUtf8 _match;
        private readonly int[]? _groupIndexes;
        private int _index;

        internal DuplicateNamedGroupEnumerator(PcreRefMatchUtf8 match, int[]? groupIndexes)
        {
            _match = match;
            _groupIndexes = groupIndexes;
            _index = -1;
        }
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal class DebugProxy
    {
        public bool Success { get; }
        public string? Value { get; }

        public PcreRefGroupUtf8.DebugProxy[] Groups { get; }

        public DebugProxy(PcreRefMatchUtf8 match)
        {
            Success = match.Success;
            Value = Success ? PcreRegexUtf8.GetString(match.Value) : null;

            Groups = new PcreRefGroupUtf8.DebugProxy[match.CaptureCount + 1];
            for (var i = 0; i <= match.CaptureCount; ++i)
                Groups[i] = new PcreRefGroupUtf8.DebugProxy(match[i]);
        }

        public override string ToString()
            => Value ?? "<no match>";
    }
}
