using System;

namespace PCRE.Internal
{
    internal unsafe class InternalRegex : IDisposable
    {
        private IntPtr _code;

        public int CaptureCount { get; }

        public InternalRegex(string pattern, PcreRegexSettings settings)
        {
            fixed (char* pPattern = pattern)
            {
                var input = new Native.compile_input { pattern = pPattern };
                settings.FillCompileInput(ref input);

                Native.compile(ref input, out var result);
                _code = result.code;

                if (_code == IntPtr.Zero || result.error_code != 0)
                    throw new ArgumentException("PCRE pattern compilation failed"); // TODO get real error message
            }

            CaptureCount = (int)GetInfoUInt32(PcreConstants.INFO_CAPTURECOUNT);
        }

        ~InternalRegex()
        {
            FreeCode();
        }

        public void Dispose()
        {
            FreeCode();
            GC.SuppressFinalize(this);
        }

        private void FreeCode()
        {
            if (_code != IntPtr.Zero)
            {
                Native.code_free(_code);
                _code = IntPtr.Zero;
            }
        }

        public uint GetInfoUInt32(uint key)
        {
            uint result;
            var errorCode = Native.pattern_info(_code, key, &result);

            if (errorCode != 0)
                throw new InvalidOperationException($"Error in pcre2_pattern_info: {Native.GetErrorMessage(errorCode)}");

            return result;
        }

        public UIntPtr GetInfoNativeInt(uint key)
        {
            UIntPtr result;
            var errorCode = Native.pattern_info(_code, key, &result);

            if (errorCode != 0)
                throw new InvalidOperationException($"Error in pcre2_pattern_info: {Native.GetErrorMessage(errorCode)}");

            return result;
        }
    }
}
