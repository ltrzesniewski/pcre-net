using System;
using PCRE.Wrapper;

namespace PCRE
{
    public static class PcreBuildInfo
    {
        // TODO : Bsr

        public static bool Jit
        {
            get { return GetConfigBool(ConfigKey.Jit); }
        }

        public static string JitTarget
        {
            get { return PcreBuild.GetConfigString(ConfigKey.JitTarget); }
        }

        public static int LinkSize
        {
            get { return GetConfigInt(ConfigKey.LinkSize); }
        }

        public static long MatchLimit
        {
            get { return GetConfigInt(ConfigKey.MatchLimit); }
        }

        public static PcreNewLine NewLine
        {
            get { return (PcreNewLine)GetConfigInt(ConfigKey.NewLine); }
        }

        public static long ParensLimit
        {
            get { return GetConfigInt(ConfigKey.ParensLimit); }
        }

        public static long RecursionLimit
        {
            get { return GetConfigInt(ConfigKey.RecursionLimit); }
        }

        public static bool StackRecurse
        {
            get { return GetConfigBool(ConfigKey.StackRecurse); }
        }

        public static string UnicodeVersion
        {
            get { return GetConfigString(ConfigKey.Unicode); }
        }

        public static bool Unicode
        {
            get { return GetConfigBool(ConfigKey.Unicode); }
        }

        public static string VersionString
        {
            get { return GetConfigString(ConfigKey.Version); }
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

        private static string GetConfigString(ConfigKey key)
        {
            var value = PcreBuild.GetConfigString(key);
            if (value == null)
                throw new InvalidOperationException("Could not retrieve the configuration property");
            return value;
        }
    }
}
