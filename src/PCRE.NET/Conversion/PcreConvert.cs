using System;
using System.Runtime.CompilerServices;
using InlineIL;
using PCRE.Internal;
using static InlineIL.IL.Emit;

namespace PCRE.Conversion
{
    public static unsafe class PcreConvert
    {
        public static string FromPosixBasic(string pattern)
            => BasicConvert(pattern, PcreConstants.CONVERT_POSIX_BASIC);

        public static string FromPosixExtended(string pattern)
            => BasicConvert(pattern, PcreConstants.CONVERT_POSIX_EXTENDED);

        public static string FromGlob(string pattern, PcreGlobConversionOptions options)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (options == null) throw new ArgumentNullException(nameof(options));

            SkipInitConvertInput(out var input);
            options.FillConvertInput(ref input);

            return Convert(pattern, ref input);
        }

        private static string BasicConvert(string pattern, uint options)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            SkipInitConvertInput(out var input);
            input.options = options;

            return Convert(pattern, ref input);
        }

        private static string Convert(string pattern, ref Native.convert_input input)
        {
            fixed (char* pPattern = pattern)
            {
                input.pattern = pPattern;
                input.pattern_length = (uint)pattern.Length;
                input.options |= PcreConstants.CONVERT_UTF;

                var errorCode = Native.convert(ref input, out var result);

                try
                {
                    if (errorCode != 0)
                        throw new ArgumentException($"Could not convert pattern '{pattern}': {Native.GetErrorMessage(errorCode)} at offset {result.output_length}");

                    return new string(result.output, 0, (int)result.output_length);
                }
                finally
                {
                    if (result.output != null)
                        Native.convert_result_free(result.output);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SkipInitConvertInput(out Native.convert_input input)
        {
            Ret();
            throw IL.Unreachable();
        }
    }
}
