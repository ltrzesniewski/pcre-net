using System;
using PCRE.Internal;

namespace PCRE.Conversion
{
    /// <summary>
    /// Pattern conversions.
    /// </summary>
    public static unsafe class PcreConvert
    {
        /// <summary>
        /// Converts a POSIX basic pattern to a PCRE pattern.
        /// </summary>
        /// <param name="pattern">The pattern to convert.</param>
        public static string FromPosixBasic(string pattern)
            => BasicConvert(pattern, PcreConstants.CONVERT_POSIX_BASIC);

        /// <summary>
        /// Converts a POSIX extended pattern to a PCRE pattern.
        /// </summary>
        /// <param name="pattern">The pattern to convert.</param>
        public static string FromPosixExtended(string pattern)
            => BasicConvert(pattern, PcreConstants.CONVERT_POSIX_EXTENDED);

        /// <summary>
        /// Converts a glob pattern to a PCRE pattern.
        /// </summary>
        /// <param name="pattern">The pattern to convert.</param>
        /// <param name="options">Conversion options.</param>
        /// <remarks>
        /// Globs are used to match file names, and consequently have the concept of a "path separator", which defaults to backslash under Windows and forward slash otherwise.
        /// The wildcards <c>*</c> and <c>?</c> are not permitted to match separator characters, but the double-star (<c>**</c>) feature (which does match separators) is supported.
        /// </remarks>
        public static string FromGlob(string pattern, PcreGlobConversionOptions options)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (options == null) throw new ArgumentNullException(nameof(options));

            Native.convert_input input;
            _ = &input;

            options.FillConvertInput(ref input);

            return Convert(pattern, &input);
        }

        private static string BasicConvert(string pattern, uint options)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            Native.convert_input input;
            input.options = options;

            return Convert(pattern, &input);
        }

        private static string Convert(string pattern, Native.convert_input* input)
        {
            fixed (char* pPattern = pattern)
            {
                input->pattern = pPattern;
                input->pattern_length = (uint)pattern.Length;
                input->options |= PcreConstants.CONVERT_UTF;

                Native.convert_result result;
                var errorCode = Native.convert(input, &result);

                try
                {
                    if (errorCode != 0)
                        throw new PcreException((PcreErrorCode)errorCode, $"Could not convert pattern '{pattern}': {Native.GetErrorMessage(errorCode)} at offset {result.output_length}.");

                    return new string(result.output, 0, (int)result.output_length);
                }
                finally
                {
                    if (result.output != null)
                        Native.convert_result_free(result.output);
                }
            }
        }
    }
}
