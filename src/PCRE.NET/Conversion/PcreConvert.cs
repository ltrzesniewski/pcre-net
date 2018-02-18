using System;
using PCRE.Wrapper;

namespace PCRE.Conversion
{
    public static class PcreConvert
    {
        private const ConvertOptions ImplicitOptions = ConvertOptions.Utf;

        public static string FromPosixBasic(string pattern)
            => BasicConvert(pattern, ConvertOptions.PosixBasic);

        public static string FromPosixExtended(string pattern)
            => BasicConvert(pattern, ConvertOptions.PosixExtended);

        public static string FromGlob(string pattern, PcreGlobConversionOptions options)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (options == null) throw new ArgumentNullException(nameof(options));

            using (var context = options.CreateContext())
            {
                return context.Convert(pattern, ImplicitOptions | options.GetConvertOptions());
            }
        }

        private static string BasicConvert(string pattern, ConvertOptions options)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            using (var context = new ConvertContext())
            {
                return context.Convert(pattern, ImplicitOptions | options);
            }
        }
    }
}
