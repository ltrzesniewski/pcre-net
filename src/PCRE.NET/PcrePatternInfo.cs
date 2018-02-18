using System.Collections.Generic;
using System.Linq;
using PCRE.Wrapper;

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
        public PcreOptions AllOptions => (PcreOptions)_re.InternalRegex.GetInfoUInt32(InfoKey.AllOptions);
        public uint MaxBackReference => _re.InternalRegex.GetInfoUInt32(InfoKey.BackRefMax);
        public int CaptureCount => (int)_re.InternalRegex.CaptureCount;
        public bool IsCompiled => _re.InternalRegex.GetInfoUInt32(InfoKey.JitSize) != 0;
        public bool CanMatchEmptyString => _re.InternalRegex.GetInfoUInt32(InfoKey.MatchEmpty) != 0;
        public uint MaxLookBehind => _re.InternalRegex.GetInfoUInt32(InfoKey.MaxLookBehind);
        public uint MinSubjectLength => _re.InternalRegex.GetInfoUInt32(InfoKey.MinLength);
        public uint NamedGroupsCount => _re.InternalRegex.GetInfoUInt32(InfoKey.NameCount);
        public uint DepthLimit => _re.InternalRegex.GetInfoUInt32(InfoKey.DepthLimit);
        public PcreBackslashR BackslashR => (PcreBackslashR)_re.InternalRegex.GetInfoUInt32(InfoKey.Bsr);
        public bool HasBackslashC => _re.InternalRegex.GetInfoUInt32(InfoKey.HasBackslashC) != 0;
        public bool HasCrOrLf => _re.InternalRegex.GetInfoUInt32(InfoKey.HasCrOrLf) != 0;
        public bool JChanged => _re.InternalRegex.GetInfoUInt32(InfoKey.JChanged) != 0;
        public ulong FrameSize => _re.InternalRegex.GetInfoNativeInt(InfoKey.FrameSize).ToUInt64();
        public ulong JitSize => _re.InternalRegex.GetInfoNativeInt(InfoKey.JitSize).ToUInt64();
        public ulong PatternSize => _re.InternalRegex.GetInfoNativeInt(InfoKey.Size).ToUInt64();
        public uint MatchLimit => _re.InternalRegex.GetInfoUInt32(InfoKey.MatchLimit);
        public uint HeapLimit => _re.InternalRegex.GetInfoUInt32(InfoKey.HeapLimit);
        public uint FirstCodeType => _re.InternalRegex.GetInfoUInt32(InfoKey.FirstCodeType);
        public uint FirstCodeUnit => _re.InternalRegex.GetInfoUInt32(InfoKey.FirstCodeUnit);
        public uint LastCodeType => _re.InternalRegex.GetInfoUInt32(InfoKey.LastCodeType);
        public uint LastCodeUnit => _re.InternalRegex.GetInfoUInt32(InfoKey.LastCodeUnit);

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
