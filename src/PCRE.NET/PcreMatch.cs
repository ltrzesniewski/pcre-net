using System.Collections;
using System.Collections.Generic;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreMatch : IReadOnlyCollection<PcreGroup>
    {
        private readonly PcreRegex _regex;
        private readonly string _subject;
        private readonly object _offsets; // See remark about JIT in PcreRegex
        private readonly PcreGroup[] _groups;

        internal PcreMatch(PcreRegex regex, string subject, MatchOffsets offsets)
        {
            _regex = regex;
            _subject = subject;
            _offsets = offsets;
            _groups = new PcreGroup[_regex.CaptureCount + 1];
        }

        public int CaptureCount
        {
            get { return _regex.CaptureCount; }
        }

        public PcreGroup this[int index]
        {
            get { return GetGroup(index); }
        }

        public PcreGroup this[string name]
        {
            get { return GetGroup(name); }
        }

        internal string Subject
        {
            get { return _subject; }
        }

        public int Index
        {
            get { return this[0].Index; }
        }

        public int Length
        {
            get { return this[0].Length; }
        }

        public string Value
        {
            get { return this[0].Value; }
        }

        public IEnumerator<PcreGroup> GetEnumerator()
        {
            return GetAllGroups().GetEnumerator();
        }

        private IEnumerable<PcreGroup> GetAllGroups()
        {
            for (var i = 0; i <= CaptureCount; ++i)
                yield return this[i];
        }

        public PcreGroup GetGroup(int index)
        {
            if (index < 0 || index > CaptureCount)
                return null;

            var group = _groups[index];
            if (group == null)
            {
                var offsets = (MatchOffsets)_offsets;
                var startOffset = offsets.GetStartOffset(index);

                if (startOffset >= 0)
                {
                    var endOffset = offsets.GetEndOffset(index);
                    group = new PcreGroup(_subject, startOffset, endOffset);
                }
                else
                {
                    group = PcreGroup.Empty;
                }

                _groups[index] = group;
            }

            return group;
        }

        public PcreGroup GetGroup(string name)
        {
            var map = _regex.CaptureNameMap;
            if (map == null)
                return null;

            int[] indexes;
            if (!map.TryGetValue(name, out indexes))
                return null;

            if (indexes.Length == 1)
                return GetGroup(indexes[0]);

            foreach (var index in indexes)
            {
                var group = GetGroup(index);
                if (group != null && group.IsMatch)
                    return group;
            }

            return PcreGroup.Empty;
        }

        public IEnumerable<PcreGroup> GetGroups(string name)
        {
            var map = _regex.CaptureNameMap;
            if (map == null)
                yield break;

            int[] indexes;
            if (!map.TryGetValue(name, out indexes))
                yield break;

            foreach (var index in indexes)
            {
                var group = GetGroup(index);
                if (group != null)
                    yield return group;
            }
        }

        public override string ToString()
        {
            return Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        int IReadOnlyCollection<PcreGroup>.Count
        {
            get { return CaptureCount + 1; }
        }
    }
}
