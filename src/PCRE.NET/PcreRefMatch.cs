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
public unsafe ref struct PcreRefMatch
{
    private readonly InternalRegex16Bit? _regex;
    internal Span<nuint> OutputVector; // Can be empty when there is no match
    private int _resultCode;
    private char* _markPtr;

    internal ReadOnlySpan<char> Subject;

    /// <summary>
    /// A function which returns an output value out of a match.
    /// </summary>
    /// <typeparam name="T">The output value type.</typeparam>
    public delegate T Func<out T>(PcreRefMatch match);

    internal PcreRefMatch(InternalRegex16Bit regex, Span<nuint> oVector)
    {
        // Empty match

#if DEBUG
        if (oVector.Length != 0 && oVector.Length != regex.OutputVectorSize)
            throw new InvalidOperationException("Unexpected output vector size.");
#endif

        _regex = regex;
        OutputVector = oVector;

        Subject = default;
        _markPtr = null;
        _resultCode = 0;
    }

    internal PcreRefMatch(ReadOnlySpan<char> subject, InternalRegex16Bit regex, Span<nuint> oVector, char* mark)
    {
        // Callout

        Subject = subject;
        _regex = regex;
        OutputVector = oVector;
        _markPtr = mark;

        _resultCode = oVector.Length / 2;
    }

    internal readonly bool IsInitialized => _regex != null;

    /// <inheritdoc cref="PcreMatch.CaptureCount"/>
    public readonly int CaptureCount => _regex?.CaptureCount ?? 0;

    /// <summary>
    /// Returns the capturing group at a given index.
    /// </summary>
    /// <param name="index">The index of the capturing group.</param>
    public readonly PcreRefGroup this[int index]
        => GetGroup(index);

    /// <summary>
    /// Returns the capturing group of a given name.
    /// </summary>
    /// <param name="name">The name of the capturing group.</param>
    public readonly PcreRefGroup this[string name]
        => GetGroup(name);

    /// <inheritdoc cref="PcreMatch.Index"/>
    public readonly int Index => _regex is not null && _resultCode is > 0 or PcreConstants.PCRE2_ERROR_PARTIAL
        ? (int)OutputVector[0]
        : -1;

    /// <inheritdoc cref="PcreMatch.EndIndex"/>
    public readonly int EndIndex => _regex is not null && _resultCode is > 0 or PcreConstants.PCRE2_ERROR_PARTIAL
        ? (int)OutputVector[1]
        : -1;

    /// <inheritdoc cref="PcreMatch.Length"/>
    public readonly int Length => _regex is not null && _resultCode is > 0 or PcreConstants.PCRE2_ERROR_PARTIAL && OutputVector[1] > OutputVector[0]
        ? (int)(OutputVector[1] - OutputVector[0])
        : 0;

    /// <inheritdoc cref="PcreMatch.Success"/>
    public readonly bool Success => _resultCode > 0;

    /// <inheritdoc cref="PcreMatch.Value"/>
    public readonly ReadOnlySpan<char> Value => _regex is not null && _resultCode is > 0 or PcreConstants.PCRE2_ERROR_PARTIAL && OutputVector[1] > OutputVector[0]
        ? Subject.Slice((int)OutputVector[0], (int)(OutputVector[1] - OutputVector[0]))
        : ReadOnlySpan<char>.Empty;

    /// <inheritdoc cref="PcreMatch.Mark"/>
    public readonly ReadOnlySpan<char> Mark
    {
        get
        {
            if (_markPtr == null)
                return default;

            char* markEnd;
            for (markEnd = _markPtr; *markEnd != 0; ++markEnd)
            {
            }

            return new ReadOnlySpan<char>(_markPtr, (int)(markEnd - _markPtr));
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
    public readonly bool TryGetGroup(int index, out PcreRefGroup result)
    {
        result = GetGroup(index);
        return result.IsDefined;
    }

    /// <inheritdoc cref="PcreMatch.TryGetGroup(string,out PCRE.PcreGroup)"/>
    public readonly bool TryGetGroup(string name, out PcreRefGroup result)
    {
        result = GetGroup(name);
        return result.IsDefined;
    }

    private readonly PcreRefGroup GetGroup(int index)
    {
        if (_regex == null)
            return PcreRefGroup.Undefined;

        if (index < 0 || index > _regex.CaptureCount)
            return PcreRefGroup.Undefined;

        var isAvailable = index < _resultCode || IsPartialMatch && index == 0;
        if (!isAvailable)
            return PcreRefGroup.Empty;

        index *= 2;
        if (index >= OutputVector.Length)
            return PcreRefGroup.Empty;

        var startOffset = (int)OutputVector[index];
        if (startOffset < 0)
            return PcreRefGroup.Empty;

        var endOffset = (int)OutputVector[index + 1];

        return new PcreRefGroup(Subject, startOffset, endOffset);
    }

    private readonly PcreRefGroup GetGroup(string name)
    {
        if (_regex == null)
            return PcreRefGroup.Undefined;

        if (!_regex.CaptureNames.TryGetValue(name, out var indexes))
            return PcreRefGroup.Undefined;

        if (indexes.Length == 1)
            return GetGroup(indexes[0]);

        foreach (var index in indexes)
        {
            var group = GetGroup(index);
            if (group.Success)
                return group;
        }

        return PcreRefGroup.Empty;
    }

    /// <inheritdoc cref="PcreMatch.GetDuplicateNamedGroups"/>
    public readonly DuplicateNamedGroupEnumerable GetDuplicateNamedGroups(string name)
    {
        if (_regex == null)
            return default;

        if (!_regex.CaptureNames.TryGetValue(name, out var indexes))
            return default;

        return new DuplicateNamedGroupEnumerable(this, indexes);
    }

    internal void FirstMatch(ReadOnlySpan<char> subject,
                             PcreMatchSettings settings,
                             int startIndex,
                             PcreMatchOptions options,
                             PcreRefCalloutFunc? callout,
                             nuint[]? calloutOutputVector)
    {
        _regex!.Match(
            ref this,
            subject,
            settings,
            startIndex,
            options.ToPatternOptions(),
            callout,
            calloutOutputVector
        );
    }

    internal void NextMatch(PcreMatchSettings settings,
                            PcreMatchOptions options,
                            PcreRefCalloutFunc? callout,
                            nuint[]? calloutOutputVector)
    {
        var startOfNextMatchIndex = GetStartOfNextMatchIndex();
        var nextOptions = options.ToPatternOptions() | PcreConstants.PCRE2_NO_UTF_CHECK | (Length == 0 ? PcreConstants.PCRE2_NOTEMPTY_ATSTART : 0);

        OutputVector = Span<nuint>.Empty;

        _regex!.Match(
            ref this,
            Subject,
            settings,
            startOfNextMatchIndex,
            nextOptions,
            callout,
            calloutOutputVector
        );
    }

    internal void NextMatch(PcreMatchBuffer buffer,
                            PcreMatchOptions options,
                            PcreRefCalloutFunc? callout)
    {
        var startOfNextMatchIndex = GetStartOfNextMatchIndex();
        var nextOptions = options.ToPatternOptions() | PcreConstants.PCRE2_NO_UTF_CHECK | (Length == 0 ? PcreConstants.PCRE2_NOTEMPTY_ATSTART : 0);

        _regex!.BufferMatch(
            ref this,
            Subject,
            buffer,
            startOfNextMatchIndex,
            nextOptions,
            callout
        );
    }

    internal void Update(ReadOnlySpan<char> subject, scoped in Native.match_result result, nuint[]? outputVector)
    {
        Subject = subject;
        _markPtr = (char*)result.mark;
        _resultCode = result.result_code;

        if (outputVector != null)
            OutputVector = outputVector;
    }

    private readonly int GetStartOfNextMatchIndex()
    {
        // It's possible to have EndIndex < Index
        // when the pattern contains \K in a lookahead
        return Math.Max(Index, EndIndex);
    }

    /// <inheritdoc cref="PcreMatch.ToString"/>
    public readonly override string ToString()
        => Value.ToString();

    /// <summary>
    /// The list of groups in a match.
    /// </summary>
    public readonly ref struct GroupList
    {
        private readonly PcreRefMatch _match;

        internal GroupList(PcreRefMatch match)
            => _match = match;

        /// <summary>
        /// Returns the capture count.
        /// </summary>
        public int Count => _match._regex?.CaptureCount + 1 ?? 0;

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public GroupEnumerator GetEnumerator()
            => new(_match);

        /// <inheritdoc cref="PcreMatch.get_Item(int)"/>
        public PcreRefGroup this[int index]
            => _match[index];

        /// <inheritdoc cref="PcreMatch.get_Item(string)"/>
        public PcreRefGroup this[string name]
            => _match[name];

        /// <summary>
        /// Creates a <see cref="List{T}"/> out of the groups by applying a <paramref name="selector"/> method.
        /// </summary>
        /// <param name="selector">The selector that converts a group to a list item.</param>
        /// <typeparam name="T">The type of list items.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1002")]
        [SuppressMessage("Microsoft.Design", "CA1062")]
        public List<T> ToList<T>(PcreRefGroup.Func<T> selector)
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
        private readonly PcreRefMatch _match;
        private int _index;

        internal GroupEnumerator(PcreRefMatch match)
        {
            _match = match;
            _index = -1;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public readonly PcreRefGroup Current => _match[_index];

        /// <inheritdoc cref="IEnumerator.MoveNext"/>
        public bool MoveNext()
            => ++_index <= _match.CaptureCount;
    }

    /// <summary>
    /// An enumerable of duplicated named groups.
    /// </summary>
    public readonly ref struct DuplicateNamedGroupEnumerable
    {
        private readonly PcreRefMatch _match;
        private readonly int[]? _groupIndexes;

        internal DuplicateNamedGroupEnumerable(PcreRefMatch match, int[] groupIndexes)
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
        public List<T> ToList<T>(PcreRefGroup.Func<T> selector)
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
        private readonly PcreRefMatch _match;
        private readonly int[]? _groupIndexes;
        private int _index;

        internal DuplicateNamedGroupEnumerator(PcreRefMatch match, int[]? groupIndexes)
        {
            _match = match;
            _groupIndexes = groupIndexes;
            _index = -1;
        }

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public readonly PcreRefGroup Current => _groupIndexes != null ? _match.GetGroup(_groupIndexes[_index]) : PcreRefGroup.Undefined;

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

        public PcreRefGroup.DebugProxy[] Groups { get; }

        public DebugProxy(PcreRefMatch match)
        {
            Success = match.Success;
            Value = Success ? match.Value.ToString() : null;

            Groups = new PcreRefGroup.DebugProxy[match.CaptureCount + 1];
            for (var i = 0; i <= match.CaptureCount; ++i)
                Groups[i] = new PcreRefGroup.DebugProxy(match[i]);
        }

        public override string ToString()
            => Value ?? "<no match>";
    }
}
