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
                var input = new Native.compile_input
                {
                    pattern = pPattern,
                    pattern_length = (uint)pattern.Length
                };

                settings.FillCompileInput(ref input);

                Native.compile(ref input, out var result);
                _code = result.code;

                if (_code == IntPtr.Zero || result.error_code != 0)
                    throw new ArgumentException($"Invalid pattern '{pattern}': {Native.GetErrorMessage(result.error_code)} at offset {result.error_offset}");
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

        public PcreMatch Match(string subject, ref Native.match_input input)
        {
            var oVector = new uint[2 * (CaptureCount + 1)];

            fixed (char* pSubject = subject)
            fixed (uint* pOVec = &oVector[0])
            {
                input.code = _code;
                input.subject = pSubject;
                input.subject_length = (uint)subject.Length;
                input.output_vector = pOVec;

                Native.match(ref input, out var result);

                switch (result.result_code)
                {
                    case PcreConstants.ERROR_NOMATCH:
                    case PcreConstants.ERROR_PARTIAL:
                        break;

                    case PcreConstants.ERROR_CALLOUT:
                        throw new NotImplementedException();

                    default:
                        if (result.result_code < 0)
                            throw new PcreMatchException(Native.GetErrorMessage(result.result_code));

                        break;
                }

                return new PcreMatch(subject, this, ref result, oVector);
            }
        }
    }
}
