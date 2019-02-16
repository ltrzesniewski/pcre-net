using System;
using PCRE.Internal;

namespace PCRE
{
    public static class PcreBuildInfo
    {
        public static PcreBackslashR BackslashR { get; } = (PcreBackslashR) GetConfigUInt32(PcreConstants.CONFIG_BSR);
        public static bool Jit { get; } = GetConfigBool(PcreConstants.CONFIG_JIT);
        public static string JitTarget { get; } = PcreBuild.GetConfigString(PcreConstants.CONFIG_JITTARGET);
        public static uint LinkSize { get; } = GetConfigUInt32(PcreConstants.CONFIG_LINKSIZE);
        public static uint MatchLimit { get; } = GetConfigUInt32(PcreConstants.CONFIG_MATCHLIMIT);
        public static PcreNewLine NewLine { get; } = (PcreNewLine) GetConfigUInt32(PcreConstants.CONFIG_NEWLINE);
        public static uint ParensLimit { get; } = GetConfigUInt32(PcreConstants.CONFIG_PARENSLIMIT);
        public static uint DepthLimit { get; } = GetConfigUInt32(PcreConstants.CONFIG_DEPTHLIMIT);
        public static bool Unicode { get; } = GetConfigBool(PcreConstants.CONFIG_UNICODE);
        public static string UnicodeVersion { get; } = GetConfigString(PcreConstants.CONFIG_UNICODE_VERSION);
        public static string Version { get; } = GetConfigString(PcreConstants.CONFIG_VERSION);
        public static uint HeapLimit { get; } = GetConfigUInt32(PcreConstants.CONFIG_HEAPLIMIT);
        public static bool NeverBackslashC { get; } = GetConfigBool(PcreConstants.CONFIG_NEVER_BACKSLASH_C);
        public static uint CompiledWidths { get; } = GetConfigUInt32(PcreConstants.CONFIG_COMPILED_WIDTHS);

        private static bool GetConfigBool(uint key) 
            => PcreBuild.GetConfigUInt32(key).GetValueOrDefault() != 0;

        private static uint GetConfigUInt32(uint key)
        {
            var value = PcreBuild.GetConfigUInt32(key);
            if (value == null)
                throw new InvalidOperationException("Could not retrieve the configuration property: " + key);

            return value.Value;
        }

        private static string GetConfigString(uint key)
        {
            var value = PcreBuild.GetConfigString(key);
            if (value == null)
                throw new InvalidOperationException("Could not retrieve the configuration property: " + key);

            return value;
        }
    }
}
