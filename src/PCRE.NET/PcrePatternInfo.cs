using System.Collections.Generic;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcrePatternInfo
    {
        private readonly InternalRegex _re;

        internal PcrePatternInfo(InternalRegex re, RegexKey key)
        {
            _re = re;
            PatternString = key.Pattern;
            Options = key.Options;
        }

        public string PatternString { get; private set; }
        public PcreOptions Options { get; private set; }

        public int MaxBackReference
        {
            get { return _re.GetInfoInt32(InfoKey.BackRefMax); }
        }

        public int CaptureCount
        {
            get { return _re.CaptureCount; }
        }

        public bool IsCompiled
        {
            get { return _re.GetInfoInt32(InfoKey.Jit) != 0; }
        }

        public bool CanMatchEmptyString
        {
            get { return _re.GetInfoInt32(InfoKey.MatchEmpty) != 0; }
        }

        public int MaxLookBehind
        {
            get { return _re.GetInfoInt32(InfoKey.MaxLookBehind); }
        }

        public int MinSubjectLength
        {
            get { return _re.GetInfoInt32(InfoKey.MinLength); }
        }

        public int NamedGroupsCount
        {
            get { return _re.GetInfoInt32(InfoKey.NameCount); }
        }

        public int RecursionLimit
        {
            get { return _re.GetInfoInt32(InfoKey.RecursionLimit); }
        }

        public IEnumerable<int> GetGroupIndexesByName(string name)
        {
            var map = _re.CaptureNames;
            if (map == null)
                yield break;

            int[] indexes;
            if (!map.TryGetValue(name, out indexes))
                yield break;

            foreach (var index in indexes)
                yield return index;
        }
    }
}
