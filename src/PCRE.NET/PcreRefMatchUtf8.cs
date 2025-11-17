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
public unsafe ref struct PcreRefMatchUtf8
{
    private readonly object? _owner;
    internal Span<nuint> OutputVector; // Can be empty when there is no match
    private int _resultCode;
    private byte* _markPtr;

    internal ReadOnlySpan<byte> Subject;

    /// <summary>
    /// A function which returns an output value out of a match.
    /// </summary>
    /// <typeparam name="T">The output value type.</typeparam>
    public delegate T Func<out T>(PcreRefMatchUtf8 match);

    internal PcreRefMatchUtf8(InternalRegex8Bit regex, Span<nuint> oVector)
        : this(oVector)
    {
        Debug.Assert(oVector.Length == 0 || oVector.Length == regex.OutputVectorSize);

        _owner = regex;
    }

    internal PcreRefMatchUtf8(PcreMatchBufferUtf8 buffer, Span<nuint> oVector)
        : this(oVector)
    {
        Debug.Assert(oVector.Length == 0 || oVector.Length == buffer.Regex.OutputVectorSize);

        _owner = buffer; // Needs to be kept alive, as the output vector is in native memory.
    }

    private PcreRefMatchUtf8(Span<nuint> oVector)
    {
        // Empty match

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

    private readonly InternalRegex8Bit? Regex => _owner as InternalRegex8Bit
                                                 ?? (_owner as PcreMatchBufferUtf8)?.Regex;

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

    /// <inheritdoc cref="PcreMatch.Index"/>
    public readonly int Index => _owner is not null && _resultCode is > 0 or PcreConstants.PCRE2_ERROR_PARTIAL
        ? (int)OutputVector[0]
        : -1;

    /// <inheritdoc cref="PcreMatch.EndIndex"/>
    public readonly int EndIndex => _owner is not null && _resultCode is > 0 or PcreConstants.PCRE2_ERROR_PARTIAL
        ? (int)OutputVector[1]
        : -1;

    /// <inheritdoc cref="PcreMatch.Length"/>
    public readonly int Length => _owner is not null && _resultCode is > 0 or PcreConstants.PCRE2_ERROR_PARTIAL && OutputVector[1] > OutputVector[0]
        ? (int)(OutputVector[1] - OutputVector[0])
        : 0;

    /// <inheritdoc cref="PcreMatch.Success"/>
    public readonly bool Success => _resultCode > 0;

    /// <inheritdoc cref="PcreMatch.Value"/>
    public readonly ReadOnlySpan<byte> Value => _owner is not null && _resultCode is > 0 or PcreConstants.PCRE2_ERROR_PARTIAL && OutputVector[1] > OutputVector[0]
        ? Subject.Slice((int)OutputVector[0], (int)(OutputVector[1] - OutputVector[0]))
        : ReadOnlySpan<byte>.Empty;

    /// <inheritdoc cref="PcreMatch.Mark"/>
    public readonly ReadOnlySpan<byte> Mark
    {
        get
        {
            if (_markPtr == null)
                return default;

            byte* markEnd;
            for (markEnd = _markPtr; *markEnd != 0; ++markEnd)
            {
            }

            return new ReadOnlySpan<byte>(_markPtr, (int)(markEnd - _markPtr));
        }
    }

    /// <inheritdoc cref="PcreMatch.Groups"/>
    public readonly GroupList Groups => new(this);

    /// <inheritdoc cref="PcreMatch.IsPartialMatch"/>
    public readonly bool IsPartialMatch => _resultCode == PcreConstants.PCRE2_ERROR_PARTIAL;

    /// <inheritdoc cref="PcreMatch.GetEnumerator"/>
    public readonly GroupEnumerator GetEnumerator()
        => new(this);

    /// <inheritdoc cref="PcreMatch.TryGetGroup(int,out PCRE.PcreGroup)"/>
    public readonly bool TryGetGroup(int index, out PcreRefGroupUtf8 result)
    {
        result = GetGroup(index);
        return result.IsDefined;
    }

    /// <inheritdoc cref="PcreMatch.TryGetGroup(string,out PCRE.PcreGroup)"/>
    public readonly bool TryGetGroup(string name, out PcreRefGroupUtf8 result)
    {
        result = GetGroup(name);
        return result.IsDefined;
    }

    private readonly PcreRefGroupUtf8 GetGroup(int index)
    {
        if (Regex is not { } regex)
            return PcreRefGroupUtf8.Undefined;

        if (index < 0 || index > regex.CaptureCount)
            return PcreRefGroupUtf8.Undefined;

        var isAvailable = index < _resultCode || IsPartialMatch && index == 0;
        if (!isAvailable)
            return PcreRefGroupUtf8.Empty;

        index *= 2;
        if (index >= OutputVector.Length)
            return PcreRefGroupUtf8.Empty;

        var startOffset = (int)OutputVector[index];
        if (startOffset < 0)
            return PcreRefGroupUtf8.Empty;

        var endOffset = (int)OutputVector[index + 1];

        return new PcreRefGroupUtf8(Subject, startOffset, endOffset);
    }

    private readonly PcreRefGroupUtf8 GetGroup(string name)
    {
        if (Regex is not { } regex)
            return PcreRefGroupUtf8.Undefined;

        if (!regex.CaptureNames.TryGetValue(name, out var indexes))
            return PcreRefGroupUtf8.Undefined;

        if (indexes.Length == 1)
            return GetGroup(indexes[0]);

        foreach (var index in indexes)
        {
            var group = GetGroup(index);
            if (group.Success)
                return group;
        }

        return PcreRefGroupUtf8.Empty;
    }

    /// <inheritdoc cref="PcreMatch.GetDuplicateNamedGroups"/>
    public readonly DuplicateNamedGroupEnumerable GetDuplicateNamedGroups(string name)
    {
        if (Regex is not { } regex)
            return default;

        if (!regex.CaptureNames.TryGetValue(name, out var indexes))
            return default;

        return new DuplicateNamedGroupEnumerable(this, indexes);
    }

    internal void FirstMatch(ReadOnlySpan<byte> subject,
                             PcreMatchSettings settings,
                             int startIndex,
                             PcreMatchOptions options,
                             PcreRefCalloutFuncUtf8? callout,
                             nuint[]? calloutOutputVector)
    {
        Subject = subject;

        Regex!.Match(
            ref OutputVector,
            subject,
            settings,
            startIndex,
            options.ToPatternOptions(),
            callout,
            calloutOutputVector,
            out _markPtr,
            out _resultCode
        );
    }

    internal void NextMatch(PcreMatchSettings settings,
                            PcreMatchOptions options,
                            PcreRefCalloutFuncUtf8? callout,
                            nuint[]? calloutOutputVector)
    {
        var startOfNextMatchIndex = GetStartOfNextMatchIndex();
        var nextOptions = options.ToPatternOptions() | PcreConstants.PCRE2_NO_UTF_CHECK | (Length == 0 ? PcreConstants.PCRE2_NOTEMPTY_ATSTART : 0);

        OutputVector = Span<nuint>.Empty;

        Regex!.Match(
            ref OutputVector,
            Subject,
            settings,
            startOfNextMatchIndex,
            nextOptions,
            callout,
            calloutOutputVector,
            out _markPtr,
            out _resultCode
        );
    }

    internal void FirstMatch(PcreMatchBufferUtf8 buffer,
                             ReadOnlySpan<byte> subject,
                             int startIndex,
                             PcreMatchOptions options,
                             PcreRefCalloutFuncUtf8? callout)
    {
        Subject = subject;

        Regex!.BufferMatch(
            Subject,
            buffer,
            startIndex,
            options.ToPatternOptions(),
            callout,
            out _markPtr,
            out _resultCode
        );
    }

    internal void NextMatch(PcreMatchBufferUtf8 buffer,
                            PcreMatchOptions options,
                            PcreRefCalloutFuncUtf8? callout)
    {
        var startOfNextMatchIndex = GetStartOfNextMatchIndex();
        var nextOptions = options.ToPatternOptions() | PcreConstants.PCRE2_NO_UTF_CHECK | (Length == 0 ? PcreConstants.PCRE2_NOTEMPTY_ATSTART : 0);

        Regex!.BufferMatch(
            Subject,
            buffer,
            startOfNextMatchIndex,
            nextOptions,
            callout,
            out _markPtr,
            out _resultCode
        );
    }

    private readonly int GetStartOfNextMatchIndex()
    {
        // It's possible to have EndIndex < Index
        // when the pattern contains \K in a lookahead
        return Math.Max(Index, EndIndex);
    }

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
