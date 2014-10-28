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
            get { return GetConfigBool(ConfigKey.Utf8); }
        }

        public static bool Utf16
        {
            get { return GetConfigBool(ConfigKey.Utf16); }
        }

        public static bool Utf32
        {
            get { return GetConfigBool(ConfigKey.Utf32); }
        }

        public static bool UnicodeProperties
        {
            get { return GetConfigBool(ConfigKey.UnicodeProperties); }
        }

        public static bool Jit
        {
            get { return GetConfigBool(ConfigKey.Jit); }
        }

        public static string JitTarget
        {
            get { return PcreBuild.GetConfigString(ConfigKey.JitTarget); }
        }

        public static PcreNewLine NewLine
        {
            get { return (PcreNewLine)GetConfigInt(ConfigKey.NewLine); }
        }

        public static bool BackSlashRMatchesUnicode
        {
            get { return GetConfigInt(ConfigKey.Bsr) == 0; }
        }

        public static int LinkSize
        {
            get { return GetConfigInt(ConfigKey.LinkSize); }
        }

        public static long ParensLimit
        {
            get { return GetConfigLong(ConfigKey.ParensLimit); }
        }

        public static long MatchLimit
        {
            get { return GetConfigLong(ConfigKey.MatchLimit); }
        }

        public static long MatchLimitRecursion
        {
            get { return GetConfigLong(ConfigKey.MatchLimitRecursion); }
        }

        public static bool StackRecurse
        {
            get { return GetConfigBool(ConfigKey.StackRecurse); }
        }

        private static bool GetConfigBool(ConfigKey key)
        {
            return PcreBuild.GetConfigInt32(key).GetValueOrDefault() != 0;
        }

        private static int GetConfigInt(ConfigKey key)
        {
            var value = PcreBuild.GetConfigInt32(key);
            if (value == null)
                throw new InvalidOperationException("Could not retrieve the configuration property");
            return value.Value;
        }

        private static long GetConfigLong(ConfigKey key)
        {
            var value = PcreBuild.GetConfigInt64(key);
            if (value == null)
                throw new InvalidOperationException("Could not retrieve the configuration property");
            return value.Value;
        }
    }
}
