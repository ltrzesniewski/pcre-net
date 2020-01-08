using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    [DebuggerTypeProxy(typeof(DebugProxy))]
    public readonly unsafe ref struct PcreRefMatch
    {
        private readonly InternalRegex _regex;
        private readonly int _resultCode;
        private readonly uint[] _oVector;
        private readonly char* _markPtr;

        internal PcreRefMatch(ReadOnlySpan<char> subject, InternalRegex regex, ref Native.match_result result, uint[] oVector)
        {
            // Real match

            Subject = subject;
            _regex = regex;
            _oVector = oVector;
            _markPtr = result.mark;

            _resultCode = result.result_code;
        }

        internal PcreRefMatch(ReadOnlySpan<char> subject, InternalRegex regex, uint[] oVector, char* mark)
        {
            // Callout

            Subject = subject;
            _regex = regex;
            _oVector = oVector;
            _markPtr = mark;

            _resultCode = oVector.Length / 2;
        }

        public int CaptureCount => _regex.CaptureCount;

        public PcreRefGroup this[int index] => GetGroup(index);

        public PcreRefGroup this[string name] => GetGroup(name);

        internal ReadOnlySpan<char> Subject { get; }

        public int Index => this[0].Index;

        public int EndIndex => this[0].EndIndex;

        public int Length => this[0].Length;

        public ReadOnlySpan<char> Value => this[0].Value;

        public bool Success => _resultCode > 0;

        public ReadOnlySpan<char> Mark
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

        public bool IsPartialMatch => _resultCode == PcreConstants.ERROR_PARTIAL;

        public Enumerator GetEnumerator() => new Enumerator(this);

        private PcreRefGroup GetGroup(int index)
        {
            if (index < 0 || index > CaptureCount)
                return default;

            var isAvailable = index < _resultCode || IsPartialMatch && index == 0;
            if (!isAvailable)
                return default;

            index *= 2;
            if (index >= _oVector.Length)
                return default;

            var startOffset = (int)_oVector[index];
            var endOffset = (int)_oVector[index + 1];

            return new PcreRefGroup(Subject, startOffset, endOffset);
        }

        private PcreRefGroup GetGroup(string name)
        {
            var map = _regex.CaptureNames;
            if (map == null)
                return default;

            if (!map.TryGetValue(name, out var indexes))
                return default;

            if (indexes.Length == 1)
                return GetGroup(indexes[0]);

            foreach (var index in indexes)
            {
                var group = GetGroup(index);
                if (group.Success)
                    return group;
            }

            return default;
        }

        public DuplicateNamedGroupEnumerable GetDuplicateNamedGroups(string name)
        {
            var map = _regex.CaptureNames;
            if (map == null)
                return default;

            if (!map.TryGetValue(name, out var indexes))
                return default;

            return new DuplicateNamedGroupEnumerable(this, indexes);
        }

        internal int GetStartOfNextMatchIndex()
        {
            // It's possible to have EndIndex < Index
            // when the pattern contains \K in a lookahead
            return Math.Max(Index, EndIndex);
        }

        public override string ToString() => Value.ToString();

        public ref struct Enumerator
        {
            private readonly PcreRefMatch _match;
            private int _index;

            internal Enumerator(PcreRefMatch match)
            {
                _match = match;
                _index = -1;
            }

            public PcreRefGroup Current => _match[_index];

            public bool MoveNext()
                => ++_index <= _match.CaptureCount;
        }

        public readonly ref struct DuplicateNamedGroupEnumerable
        {
            private readonly PcreRefMatch _match;
            private readonly int[] _groupIndexes;

            internal DuplicateNamedGroupEnumerable(PcreRefMatch match, int[] groupIndexes)
            {
                _match = match;
                _groupIndexes = groupIndexes;
            }

            public DuplicateNamedGroupEnumerator GetEnumerator()
                => new DuplicateNamedGroupEnumerator(_match, _groupIndexes);
        }

        public ref struct DuplicateNamedGroupEnumerator
        {
            private readonly PcreRefMatch _match;
            private readonly int[] _groupIndexes;
            private int _index;

            internal DuplicateNamedGroupEnumerator(PcreRefMatch match, int[] groupIndexes)
            {
                _match = match;
                _groupIndexes = groupIndexes;
                _index = -1;
            }

            public PcreRefGroup Current => _match.GetGroup(_groupIndexes[_index]);

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
            public string Value { get; }

            public PcreRefGroup.DebugProxy[] Groups { get; }

            public DebugProxy(PcreRefMatch match)
            {
                Success = match.Success;
                Value = Success ? match.Value.ToString() : null;

                Groups = new PcreRefGroup.DebugProxy[match.CaptureCount + 1];
                for (var i = 0; i <= match.CaptureCount; ++i)
                    Groups[i] = new PcreRefGroup.DebugProxy(match[i]);
            }
        }
    }
}
