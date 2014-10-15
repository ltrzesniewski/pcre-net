using System;
using PCRE.Wrapper;

namespace PCRE
{
    public static class PcreBuildInfo
    {
        public static string VersionString
        {
            get { return PcreBuild.VersionString; }
        }

        public static bool Utf8
        {
            get { return GetConfigBool(PcreConfigKey.Utf8); }
        }

        public static bool Utf16
        {
            get { return GetConfigBool(PcreConfigKey.Utf16); }
        }

        public static bool Utf32
        {
            get { return GetConfigBool(PcreConfigKey.Utf32); }
        }

        public static bool UnicodeProperties
        {
            get { return GetConfigBool(PcreConfigKey.UnicodeProperties); }
        }

        public static bool Jit
        {
            get { return GetConfigBool(PcreConfigKey.Jit); }
        }

        public static string JitTarget
        {
            get { return PcreBuild.GetConfigString(PcreConfigKey.JitTarget); }
        }

        public static PcreNewLine NewLine
        {
            get { return (PcreNewLine)GetConfigInt(PcreConfigKey.NewLine); }
        }

        public static bool BackSlashRMatchesUnicode
        {
            get { return GetConfigInt(PcreConfigKey.Bsr) == 0; }
        }

        public static int LinkSize
        {
            get { return GetConfigInt(PcreConfigKey.LinkSize); }
        }

        public static long ParensLimit
        {
            get { return GetConfigLong(PcreConfigKey.ParensLimit); }
        }

        public static long MatchLimit
        {
            get { return GetConfigLong(PcreConfigKey.MatchLimit); }
        }

        public static long MatchLimitRecursion
        {
            get { return GetConfigLong(PcreConfigKey.MatchLimitRecursion); }
        }

        public static bool StackRecurse
        {
            get { return GetConfigBool(PcreConfigKey.StackRecurse); }
        }

        private static bool GetConfigBool(PcreConfigKey key)
        {
            return PcreBuild.GetConfigInt32(key).GetValueOrDefault() != 0;
        }

        private static int GetConfigInt(PcreConfigKey key)
        {
            var value = PcreBuild.GetConfigInt32(key);
            if (value == null)
                throw new InvalidOperationException("Could not retrieve the configuration property");
            return value.Value;
        }

        private static long GetConfigLong(PcreConfigKey key)
        {
            var value = PcreBuild.GetConfigInt64(key);
            if (value == null)
                throw new InvalidOperationException("Could not retrieve the configuration property");
            return value.Value;
        }
    }
}
