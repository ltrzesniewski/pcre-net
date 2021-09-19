using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    [DebuggerTypeProxy(typeof(DebugProxy))]
    public unsafe ref struct PcreRefMatch
    {
        private readonly InternalRegex? _regex;
        internal Span<uint> OutputVector; // Can be empty when there is no match
        private int _resultCode;
        private char* _markPtr;

        public delegate T Func<out T>(PcreRefMatch match);

        internal PcreRefMatch(InternalRegex regex, Span<uint> oVector)
        {
            // Empty match

            _regex = regex;
            OutputVector = oVector;

            Subject = default;
            _markPtr = null;
            _resultCode = 0;
        }

        internal PcreRefMatch(ReadOnlySpan<char> subject, InternalRegex regex, Span<uint> oVector, char* mark)
        {
            // Callout

            Subject = subject;
            _regex = regex;
            OutputVector = oVector;
            _markPtr = mark;

            _resultCode = oVector.Length / 2;
        }

        public readonly int CaptureCount => _regex?.CaptureCount ?? 0;

        public readonly PcreRefGroup this[int index] => GetGroup(index);
        public readonly PcreRefGroup this[string name] => GetGroup(name);

        internal ReadOnlySpan<char> Subject;
        internal readonly bool IsInitialized => _regex != null;

        public readonly int Index => this[0].Index;
        public readonly int EndIndex => this[0].EndIndex;
        public readonly int Length => this[0].Length;

        public readonly bool Success => _resultCode > 0;
        public readonly ReadOnlySpan<char> Value => this[0].Value;

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

        public readonly GroupList Groups => new(this);

        public readonly bool IsPartialMatch => _resultCode == PcreConstants.ERROR_PARTIAL;

        public readonly GroupEnumerator GetEnumerator() => new(this);

        public readonly bool TryGetGroup(int index, out PcreRefGroup result)
        {
            result = GetGroup(index);
            return result.IsDefined;
        }

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
                                 uint[]? calloutOutputVector)
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
                                uint[]? calloutOutputVector)
        {
            _regex!.Match(
                ref this,
                Subject,
                settings,
                GetStartOfNextMatchIndex(),
                options.ToPatternOptions() | PcreConstants.NO_UTF_CHECK | (Length == 0 ? PcreConstants.NOTEMPTY_ATSTART : 0),
                callout,
                calloutOutputVector
            );
        }

        internal void Update(ReadOnlySpan<char> subject, in Native.match_result result, uint[]? outputVector)
        {
            Subject = subject;
            _markPtr = result.mark;
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

        /// <summary>
        /// Creates a copy that won't get modified.
        /// </summary>
        public readonly PcreRefMatch Copy()
        {
            var copy = this;

            if (OutputVector.Length != 0)
                copy.OutputVector = OutputVector.ToArray();

            return copy;
        }

        public override readonly string ToString()
            => Value.ToString();

        public readonly ref struct GroupList
        {
            private readonly PcreRefMatch _match;

            internal GroupList(PcreRefMatch match)
                => _match = match;

            public int Count => _match._regex?.CaptureCount + 1 ?? 0;

            public GroupEnumerator GetEnumerator()
                => new(_match);

            public PcreRefGroup this[int index] => _match[index];
            public PcreRefGroup this[string name] => _match[name];

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

        public ref struct GroupEnumerator
        {
            private readonly PcreRefMatch _match;
            private int _index;

            internal GroupEnumerator(PcreRefMatch match)
            {
                _match = match;
                _index = -1;
            }

            public readonly PcreRefGroup Current => _match[_index];

            public bool MoveNext()
                => ++_index <= _match.CaptureCount;
        }

        public readonly ref struct DuplicateNamedGroupEnumerable
        {
            private readonly PcreRefMatch _match;
            private readonly int[]? _groupIndexes;

            internal DuplicateNamedGroupEnumerable(PcreRefMatch match, int[] groupIndexes)
            {
                _match = match;
                _groupIndexes = groupIndexes;
            }

            public DuplicateNamedGroupEnumerator GetEnumerator()
                => new(_match, _groupIndexes);

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

            public readonly PcreRefGroup Current => _groupIndexes != null ? _match.GetGroup(_groupIndexes[_index]) : PcreRefGroup.Undefined;

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

            public override string ToString() => Value ?? "<no match>";
        }
    }
}
