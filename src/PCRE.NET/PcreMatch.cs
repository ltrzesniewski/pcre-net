using System;
using System.Collections;
using System.Collections.Generic;
using PCRE.Internal;

namespace PCRE
{
    public sealed class PcreMatch : IPcreGroup, IPcreGroupList
    {
        private readonly PcreGroup[] _groups;
        private readonly InternalRegex _regex;
        private readonly int _resultCode;
        private readonly uint[] _oVector;

        internal PcreMatch(string subject, InternalRegex regex, ref Native.match_result result, uint[] oVector)
        {
            Subject = subject;
            _regex = regex;
            _oVector = oVector;
            _groups = new PcreGroup[regex.CaptureCount + 1];

            _resultCode = result.result_code;
        }

        public int CaptureCount => _regex.CaptureCount;

        public PcreGroup this[int index] => GetGroup(index);

        public PcreGroup this[string name] => GetGroup(name);

        internal string Subject { get; }

        public int Index => this[0].Index;

        public int EndIndex => this[0].EndIndex;

        public int Length => this[0].Length;

        public string Value => this[0].Value;

        public bool Success => _resultCode > 0;

        public string Mark => throw new NotImplementedException(); // InternalResult.Mark;

        public IPcreGroupList Groups => this;

        public bool IsPartialMatch => _resultCode == PcreConstants.ERROR_PARTIAL;

        public IEnumerator<PcreGroup> GetEnumerator() => GetAllGroups().GetEnumerator();

        private IEnumerable<PcreGroup> GetAllGroups()
        {
            for (var i = 0; i <= CaptureCount; ++i)
                yield return this[i];
        }

        private PcreGroup GetGroup(int index)
        {
            if (index < 0 || index > CaptureCount)
                return null;

            var group = _groups[index];
            if (group == null)
                _groups[index] = group = CreateGroup(index);

            return group;
        }

        private PcreGroup CreateGroup(int index)
        {
            var isAvailable = index < _resultCode || IsPartialMatch && index == 0;

            if (!isAvailable)
                return PcreGroup.Empty;

            var startOffset = GetStartOffset(index);
            if (startOffset >= 0)
                return new PcreGroup(Subject, startOffset, GetEndOffset(index));

            return PcreGroup.Empty;
        }

        private PcreGroup GetGroup(string name)
        {
            throw new NotImplementedException();
//            var map = InternalResult.Regex.CaptureNames;
//            if (map == null)
//                return null;
//
//            if (!map.TryGetValue(name, out var indexes))
//                return null;
//
//            if (indexes.Length == 1)
//                return GetGroup(indexes[0]);
//
//            foreach (var index in indexes)
//            {
//                var group = GetGroup(index);
//                if (group != null && group.Success)
//                    return group;
//            }
//
//            return PcreGroup.Empty;
        }

        public IEnumerable<PcreGroup> GetDuplicateNamedGroups(string name)
        {
            throw new NotImplementedException();
//            var map = InternalResult.Regex.CaptureNames;
//            if (map == null)
//                yield break;
//
//            if (!map.TryGetValue(name, out var indexes))
//                yield break;
//
//            foreach (var index in indexes)
//            {
//                var group = GetGroup(index);
//                if (group != null)
//                    yield return group;
//            }
        }

        internal int GetStartOfNextMatchIndex()
        {
            // It's possible to have EndIndex < Index
            // when the pattern contains \K in a lookahead
            return Math.Max(Index, EndIndex);
        }

        private int GetStartOffset(int index)
            => (uint)index < _oVector.Length ? (int)_oVector[2 * index] : -1;

        private int GetEndOffset(int index)
            => (uint)index < _oVector.Length ? (int)_oVector[2 * index + 1] : -1;

        public override string ToString() => Value;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        int IReadOnlyCollection<PcreGroup>.Count => CaptureCount + 1;
    }
}
