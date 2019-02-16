using System.Collections.Generic;
using PCRE.Internal;

namespace PCRE
{
    public sealed class PcrePatternInfo
    {
        private readonly IInternalRegexWrapper _re;
        private IReadOnlyList<PcreCalloutInfo> _callouts;

        internal PcrePatternInfo(IInternalRegexWrapper re)
        {
            _re = re;
        }

        public string PatternString => _re.Key.Pattern;
        public PcreRegexSettings Settings => _re.Key.Settings;
        public PcreOptions Options => _re.Key.Settings.Options;
        public PcreOptions ArgOptions => (PcreOptions)_re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_ARGOPTIONS);
        public PcreOptions AllOptions => (PcreOptions)_re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_ALLOPTIONS);
        public PcreExtraCompileOptions ExtraOptions => (PcreExtraCompileOptions)_re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_EXTRAOPTIONS);
        public uint MaxBackReference => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_BACKREFMAX);
        public int CaptureCount => (int)_re.InternalRegex.CaptureCount;
        public bool IsCompiled => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_JITSIZE) != 0;
        public bool CanMatchEmptyString => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_MATCHEMPTY) != 0;
        public uint MaxLookBehind => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_MAXLOOKBEHIND);
        public uint MinSubjectLength => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_MINLENGTH);
        public uint NamedGroupsCount => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_NAMECOUNT);
        public uint DepthLimit => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_DEPTHLIMIT);
        public PcreBackslashR BackslashR => (PcreBackslashR)_re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_BSR);
        public bool HasBackslashC => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_HASBACKSLASHC) != 0;
        public bool HasCrOrLf => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_HASCRORLF) != 0;
        public bool JChanged => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_JCHANGED) != 0;
        public ulong FrameSize => _re.InternalRegex.GetInfoNativeInt(PcreConstants.INFO_FRAMESIZE).ToUInt64();
        public ulong JitSize => _re.InternalRegex.GetInfoNativeInt(PcreConstants.INFO_JITSIZE).ToUInt64();
        public ulong PatternSize => _re.InternalRegex.GetInfoNativeInt(PcreConstants.INFO_SIZE).ToUInt64();
        public uint MatchLimit => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_MATCHLIMIT);
        public uint HeapLimit => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_HEAPLIMIT);
        public uint FirstCodeType => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_FIRSTCODETYPE);
        public uint FirstCodeUnit => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_FIRSTCODEUNIT);
        public uint LastCodeType => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_LASTCODETYPE);
        public uint LastCodeUnit => _re.InternalRegex.GetInfoUInt32(PcreConstants.INFO_LASTCODEUNIT);

        public IReadOnlyList<PcreCalloutInfo> Callouts => _callouts ?? (_callouts = _re.InternalRegex.Callouts.Select(i => new PcreCalloutInfo(i)).ToList().AsReadOnly());

        public IEnumerable<int> GetGroupIndexesByName(string name)
        {
            var map = _re.InternalRegex.CaptureNames;
            if (map == null)
                yield break;

            if (!map.TryGetValue(name, out var indexes))
                yield break;

            foreach (var index in indexes)
                yield return index;
        }
    }
}
