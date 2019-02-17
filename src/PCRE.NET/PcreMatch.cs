using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using PCRE.Internal;

namespace PCRE
{
    public sealed class PcreMatch : IPcreGroup, IPcreGroupList
    {
        private readonly InternalRegex _regex;
        private readonly int _resultCode;
        private readonly uint[] _oVector;

        [CanBeNull]
        private PcreGroup[] _groups;

        internal PcreMatch(string subject, InternalRegex regex, ref Native.match_result result, uint[] oVector)
        {
            // Real match

            Subject = subject;
            _regex = regex;
            _oVector = oVector;

            _resultCode = result.result_code;
        }

        internal PcreMatch(string subject, InternalRegex regex, uint[] oVector)
        {
            // Callout

            Subject = subject;
            _regex = regex;
            _oVector = oVector;

            _resultCode = oVector.Length / 2;
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

            var groups = _groups ?? (_groups = new PcreGroup[_regex.CaptureCount + 1]);

            var group = groups[index];
            if (group == null)
                groups[index] = group = CreateGroup(index);

            return group;
        }

        private PcreGroup CreateGroup(int index)
        {
            var isAvailable = index < _resultCode || IsPartialMatch && index == 0;

            if (!isAvailable)
                return PcreGroup.Empty;

            index *= 2;
            if (index >= _oVector.Length)
                return PcreGroup.Empty;

            var startOffset = (int)_oVector[index];
            var endOffset = (int)_oVector[index + 1];

            return new PcreGroup(Subject, startOffset, endOffset);
        }

        private PcreGroup GetGroup(string name)
        {
            var map = _regex.CaptureNames;
            if (map == null)
                return null;

            if (!map.TryGetValue(name, out var indexes))
                return null;

            if (indexes.Length == 1)
                return GetGroup(indexes[0]);

            foreach (var index in indexes)
            {
                var group = GetGroup(index);
                if (group != null && group.Success)
                    return group;
            }

            return PcreGroup.Empty;
        }

        public IEnumerable<PcreGroup> GetDuplicateNamedGroups(string name)
        {
            var map = _regex.CaptureNames;
            if (map == null)
                yield break;

            if (!map.TryGetValue(name, out var indexes))
                yield break;

            foreach (var index in indexes)
            {
                var group = GetGroup(index);
                if (group != null)
                    yield return group;
            }
        }

        internal int GetStartOfNextMatchIndex()
        {
            // It's possible to have EndIndex < Index
            // when the pattern contains \K in a lookahead
            return Math.Max(Index, EndIndex);
        }

        public override string ToString() => Value;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        int IReadOnlyCollection<PcreGroup>.Count => CaptureCount + 1;
    }
}
