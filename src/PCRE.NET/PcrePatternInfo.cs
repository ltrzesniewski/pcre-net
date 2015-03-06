using System.Collections.Generic;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcrePatternInfo
    {
        private readonly IInternalRegexWrapper _re;

        internal PcrePatternInfo(IInternalRegexWrapper re)
        {
            _re = re;
        }

        public string PatternString
        {
            get { return _re.Key.Pattern; }
        }

        public PcreRegexSettings Settings
        {
            get { return _re.Key.Settings; }
        }

        public PcreOptions Options
        {
            get { return _re.Key.Settings.Options; }
        }

        public int MaxBackReference
        {
            get { return _re.InternalRegex.GetInfoInt32(InfoKey.BackRefMax); }
        }

        public int CaptureCount
        {
            get { return _re.InternalRegex.CaptureCount; }
        }

        public bool IsCompiled
        {
            get { return _re.InternalRegex.GetInfoInt32(InfoKey.JitSize) != 0; }
        }

        public bool CanMatchEmptyString
        {
            get { return _re.InternalRegex.GetInfoInt32(InfoKey.MatchEmpty) != 0; }
        }

        public int MaxLookBehind
        {
            get { return _re.InternalRegex.GetInfoInt32(InfoKey.MaxLookBehind); }
        }

        public int MinSubjectLength
        {
            get { return _re.InternalRegex.GetInfoInt32(InfoKey.MinLength); }
        }

        public int NamedGroupsCount
        {
            get { return _re.InternalRegex.GetInfoInt32(InfoKey.NameCount); }
        }

        public int RecursionLimit
        {
            get { return _re.InternalRegex.GetInfoInt32(InfoKey.RecursionLimit); }
        }

        public IEnumerable<int> GetGroupIndexesByName(string name)
        {
            var map = _re.InternalRegex.CaptureNames;
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
