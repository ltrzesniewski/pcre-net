using System;
using System.Collections;
using System.Collections.Generic;
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
    public readonly ref struct GroupList
    {
        private readonly PcreRefMatchUtf8 _match;

        internal GroupList(PcreRefMatchUtf8 match)
            => _match = match;

        /// <summary>
        /// Returns the capture count.
        /// </summary>
        public int Count => _match.Regex?.CaptureCount + 1 ?? 0;

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public GroupEnumerator GetEnumerator()
            => new(_match);

        /// <inheritdoc cref="PcreMatch.get_Item(int)"/>
        public PcreRefGroupUtf8 this[int index]
            => _match[index];

        /// <inheritdoc cref="PcreMatch.get_Item(string)"/>
        public PcreRefGroupUtf8 this[string name]
            => _match[name];

        /// <summary>
        /// Creates a <see cref="List{T}"/> out of the groups by applying a <paramref name="selector"/> method.
        /// </summary>
        /// <param name="selector">The selector that converts a group to a list item.</param>
        /// <typeparam name="T">The type of list items.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1002")]
        [SuppressMessage("Microsoft.Design", "CA1062")]
        public List<T> ToList<T>(PcreRefGroupUtf8.Func<T> selector)
        {
            var result = new List<T>(Count);

            foreach (var item in this)
                result.Add(selector(item));

            return result;
        }
    }

    /// <summary>
    /// A capturing group enumerator.
    /// </summary>
    public ref struct GroupEnumerator
    {
        private readonly PcreRefMatchUtf8 _match;
        private int _index;

        internal GroupEnumerator(PcreRefMatchUtf8 match)
        {
            _match = match;
            _index = -1;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public readonly PcreRefGroupUtf8 Current => _match[_index];

        /// <inheritdoc cref="IEnumerator.MoveNext"/>
        public bool MoveNext()
            => ++_index <= _match.CaptureCount;
    }

    /// <summary>
    /// An enumerable of duplicated named groups.
    /// </summary>
    public readonly ref struct DuplicateNamedGroupEnumerable
    {
        private readonly PcreRefMatchUtf8 _match;
        private readonly int[]? _groupIndexes;

        internal DuplicateNamedGroupEnumerable(PcreRefMatchUtf8 match, int[] groupIndexes)
        {
            _match = match;
            _groupIndexes = groupIndexes;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public DuplicateNamedGroupEnumerator GetEnumerator()
            => new(_match, _groupIndexes);

        /// <summary>
        /// Creates a <see cref="List{T}"/> out of the groups by applying a <paramref name="selector"/> method.
        /// </summary>
        /// <param name="selector">The selector that converts a group to a list item.</param>
        /// <typeparam name="T">The type of list items.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1002")]
        [SuppressMessage("Microsoft.Design", "CA1062")]
        public List<T> ToList<T>(PcreRefGroupUtf8.Func<T> selector)
        {
            var result = new List<T>(_groupIndexes?.Length ?? 0);

            foreach (var item in this)
                result.Add(selector(item));

            return result;
        }
    }

    /// <summary>
    /// An enumerator of duplicated named groups.
    /// </summary>
    public ref struct DuplicateNamedGroupEnumerator
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

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public readonly PcreRefGroupUtf8 Current => _groupIndexes != null ? _match.GetGroup(_groupIndexes[_index]) : PcreRefGroupUtf8.Undefined;

        /// <inheritdoc cref="IEnumerator.MoveNext"/>
        public bool MoveNext()
        {
            if (_groupIndexes is null)
                return false;

            if (_index + 1 >= _groupIndexes.Length)
                return false;

            ++_index;
            return true;
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
