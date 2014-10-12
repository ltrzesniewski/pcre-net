using System;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreInformation
    {
        internal static readonly PcreInformation Instance = new PcreInformation();

        private PcreInformation()
        {
        }

        public string VersionString
        {
            get { return PcreBuild.VersionString; }
        }

        public bool Utf8
        {
            get { return GetConfigBool(PcreConfigKey.Utf8); }
        }

        public bool Utf16
        {
            get { return GetConfigBool(PcreConfigKey.Utf16); }
        }

        public bool Utf32
        {
            get { return GetConfigBool(PcreConfigKey.Utf32); }
        }

        public bool UnicodeProperties
        {
            get { return GetConfigBool(PcreConfigKey.UnicodeProperties); }
        }

        public bool Jit
        {
            get { return GetConfigBool(PcreConfigKey.Jit); }
        }

        public string JitTarget
        {
            get { return PcreBuild.GetConfigString(PcreConfigKey.JitTarget); }
        }

        public PcreNewLine NewLine
        {
            get { return (PcreNewLine)GetConfigInt(PcreConfigKey.NewLine); }
        }

        public bool BackSlashRMatchesUnicode
        {
            get { return GetConfigInt(PcreConfigKey.Bsr) == 0; }
        }


        public int LinkSize
        {
            get { return GetConfigInt(PcreConfigKey.LinkSize); }
        }


        public long ParensLimit
        {
            get { return GetConfigLong(PcreConfigKey.ParensLimit); }
        }

        public long MatchLimit
        {
            get { return GetConfigLong(PcreConfigKey.MatchLimit); }
        }

        public long MatchLimitRecursion
        {
            get { return GetConfigLong(PcreConfigKey.MatchLimitRecursion); }
        }

        public bool StackRecurse
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
