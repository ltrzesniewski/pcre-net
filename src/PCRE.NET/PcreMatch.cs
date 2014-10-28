using System;
using System.Collections;
using System.Collections.Generic;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreMatch : IEnumerable<PcreGroup>
    {
        private readonly PcreRegex _regex;
        private readonly string _subject;
        private readonly MatchOffsets _offsets;
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
            get
            {
                var group = TryGetGroup(index);

                if (group == null)
                    throw new ArgumentOutOfRangeException("index", "Group index out of range");

                return group;
            }
        }

        public PcreGroup this[string name]
        {
            get
            {
                var group = TryGetGroup(name);

                if (group == null)
                    throw new ArgumentException(String.Format("The named group '{0}' does not exist", name));

                return group;
            }
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
            return EnumerateAllGroups().GetEnumerator();
        }

        private IEnumerable<PcreGroup> EnumerateAllGroups()
        {
            for (var i = 0; i <= CaptureCount; ++i)
                yield return this[i];
        }

        public PcreGroup TryGetGroup(int index)
        {
            if (index < 0 || index > CaptureCount)
                return null;

            var group = _groups[index];
            if (group == null)
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                var startOffset = _offsets.GetStartOffset(index);

                if (startOffset >= 0)
                {
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                    var endOffset = _offsets.GetEndOffset(index);
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

        public PcreGroup TryGetGroup(string name)
        {
            var map = _regex.CaptureNameMap;
            if (map == null)
                return null;

            int[] indexes;
            if (!map.TryGetValue(name, out indexes))
                return null;

            if (indexes.Length == 1)
                return TryGetGroup(indexes[0]);

            foreach (var index in indexes)
            {
                var group = TryGetGroup(index);
                if (group != null && group.IsMatch)
                    return group;
            }

            return PcreGroup.Empty;
        }

        public override string ToString()
        {
            return Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
