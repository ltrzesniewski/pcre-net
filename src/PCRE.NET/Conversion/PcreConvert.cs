using System;
using PCRE.Internal;

namespace PCRE.Conversion
{
    public static class PcreConvert
    {
        private const uint ImplicitOptions = PcreConstants.UTF;

        public static string FromPosixBasic(string pattern)
            => BasicConvert(pattern, PcreConstants.CONVERT_POSIX_BASIC);

        public static string FromPosixExtended(string pattern)
            => BasicConvert(pattern, PcreConstants.CONVERT_POSIX_EXTENDED);

        public static string FromGlob(string pattern, PcreGlobConversionOptions options)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (options == null) throw new ArgumentNullException(nameof(options));

            using (var context = options.CreateContext())
            {
                return context.Convert(pattern, ImplicitOptions | options.GetConvertOptions());
            }
        }

        private static string BasicConvert(string pattern, uint options)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            using (var context = new ConvertContext())
            {
                return context.Convert(pattern, ImplicitOptions | options);
            }
        }
    }
}
