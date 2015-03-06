using System;
using PCRE.Wrapper;

namespace PCRE
{
    public static class PcreBuildInfo
    {
        static PcreBuildInfo()
        {
            BackslashR = (PcreBackslashR)GetConfigUInt32(ConfigKey.Bsr);
            Jit = GetConfigBool(ConfigKey.Jit);
            JitTarget = PcreBuild.GetConfigString(ConfigKey.JitTarget);
            LinkSize = GetConfigUInt32(ConfigKey.LinkSize);
            MatchLimit = GetConfigUInt32(ConfigKey.MatchLimit);
            NewLine = (PcreNewLine)GetConfigUInt32(ConfigKey.NewLine);
            ParensLimit = GetConfigUInt32(ConfigKey.ParensLimit);
            RecursionLimit = GetConfigUInt32(ConfigKey.RecursionLimit);
            StackRecurse = GetConfigBool(ConfigKey.StackRecurse);
            UnicodeVersion = GetConfigString(ConfigKey.UnicodeVersion);
            Unicode = GetConfigBool(ConfigKey.Unicode);
            Version = GetConfigString(ConfigKey.Version);
        }

        public static PcreBackslashR BackslashR { get; private set; }
        public static bool Jit { get; private set; }
        public static string JitTarget { get; private set; }
        public static uint LinkSize { get; private set; }
        public static uint MatchLimit { get; private set; }
        public static PcreNewLine NewLine { get; private set; }
        public static uint ParensLimit { get; private set; }
        public static uint RecursionLimit { get; private set; }
        public static bool StackRecurse { get; private set; }
        public static string UnicodeVersion { get; private set; }
        public static bool Unicode { get; private set; }
        public static string Version { get; private set; }

        private static bool GetConfigBool(ConfigKey key)
        {
            return PcreBuild.GetConfigUInt32(key).GetValueOrDefault() != 0;
        }

        private static uint GetConfigUInt32(ConfigKey key)
        {
            var value = PcreBuild.GetConfigUInt32(key);
            if (value == null)
                throw new InvalidOperationException("Could not retrieve the configuration property: " + key);
            return value.Value;
        }

        private static string GetConfigString(ConfigKey key)
        {
            var value = PcreBuild.GetConfigString(key);
            if (value == null)
                throw new InvalidOperationException("Could not retrieve the configuration property: " + key);
            return value;
        }
    }
}
