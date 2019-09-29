using System;
using System.Collections.Generic;
using PCRE.Internal;

namespace PCRE
{
    public sealed class PcrePatternInfo
    {
        private readonly InternalRegex _re;
        private IReadOnlyList<PcreCalloutInfo> _callouts;

        internal PcrePatternInfo(InternalRegex re)
        {
            _re = re;
        }

        public string PatternString => _re.Pattern;
        public PcreRegexSettings Settings => _re.Settings;
        public PcreOptions Options => _re.Settings.Options;
        public PcreJitCompileOptions JitOptions => _re.Settings.JitCompileOptions;
        public PcreOptions ArgOptions => (PcreOptions)_re.GetInfoUInt32(PcreConstants.INFO_ARGOPTIONS);
        public PcreOptions AllOptions => (PcreOptions)_re.GetInfoUInt32(PcreConstants.INFO_ALLOPTIONS);
        public PcreExtraCompileOptions ExtraOptions => (PcreExtraCompileOptions)_re.GetInfoUInt32(PcreConstants.INFO_EXTRAOPTIONS);
        public uint MaxBackReference => _re.GetInfoUInt32(PcreConstants.INFO_BACKREFMAX);
        public int CaptureCount => _re.CaptureCount;
        public bool IsCompiled => _re.GetInfoNativeInt(PcreConstants.INFO_JITSIZE) != UIntPtr.Zero;
        public bool CanMatchEmptyString => _re.GetInfoUInt32(PcreConstants.INFO_MATCHEMPTY) != 0;
        public uint MaxLookBehind => _re.GetInfoUInt32(PcreConstants.INFO_MAXLOOKBEHIND);
        public uint MinSubjectLength => _re.GetInfoUInt32(PcreConstants.INFO_MINLENGTH);
        public uint NamedGroupsCount => _re.GetInfoUInt32(PcreConstants.INFO_NAMECOUNT);
        public uint DepthLimit => _re.GetInfoUInt32(PcreConstants.INFO_DEPTHLIMIT);
        public PcreBackslashR BackslashR => (PcreBackslashR)_re.GetInfoUInt32(PcreConstants.INFO_BSR);
        public bool HasBackslashC => _re.GetInfoUInt32(PcreConstants.INFO_HASBACKSLASHC) != 0;
        public bool HasCrOrLf => _re.GetInfoUInt32(PcreConstants.INFO_HASCRORLF) != 0;
        public bool JChanged => _re.GetInfoUInt32(PcreConstants.INFO_JCHANGED) != 0;
        public ulong FrameSize => _re.GetInfoNativeInt(PcreConstants.INFO_FRAMESIZE).ToUInt64();
        public ulong JitSize => _re.GetInfoNativeInt(PcreConstants.INFO_JITSIZE).ToUInt64();
        public ulong PatternSize => _re.GetInfoNativeInt(PcreConstants.INFO_SIZE).ToUInt64();
        public uint MatchLimit => _re.GetInfoUInt32(PcreConstants.INFO_MATCHLIMIT);
        public uint HeapLimit => _re.GetInfoUInt32(PcreConstants.INFO_HEAPLIMIT);
        public uint FirstCodeType => _re.GetInfoUInt32(PcreConstants.INFO_FIRSTCODETYPE);
        public uint FirstCodeUnit => _re.GetInfoUInt32(PcreConstants.INFO_FIRSTCODEUNIT);
        public uint LastCodeType => _re.GetInfoUInt32(PcreConstants.INFO_LASTCODETYPE);
        public uint LastCodeUnit => _re.GetInfoUInt32(PcreConstants.INFO_LASTCODEUNIT);

        public IReadOnlyList<PcreCalloutInfo> Callouts => _callouts ??= _re.GetCallouts();

        public IEnumerable<int> GetGroupIndexesByName(string name)
        {
            var map = _re.CaptureNames;
            if (map == null)
                yield break;

            if (!map.TryGetValue(name, out var indexes))
                yield break;

            foreach (var index in indexes)
                yield return index;
        }
    }
}
