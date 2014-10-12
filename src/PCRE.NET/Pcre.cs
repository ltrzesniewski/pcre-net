using PCRE.Wrapper;

namespace PCRE
{
    public class Pcre
    {
        public static string VersionString
        {
            get { return PcreWrapper.GetVersionString(); }
        }
    }
}
