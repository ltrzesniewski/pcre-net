using System;

namespace PCRE.Internal
{
    internal unsafe class InternalRegex : IDisposable
    {
        private IntPtr _code;

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
    }
}
