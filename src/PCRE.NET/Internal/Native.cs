using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace PCRE.Internal
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static unsafe partial class Native
    {
        private static readonly LibImpl _impl = GetLibImpl();

        private static LibImpl GetLibImpl()
        {
            try
            {
                var impl = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? new WinImpl()
                    : throw new PlatformNotSupportedException();

                impl.get_error_message(0, null, 0);
                return impl;
            }
            catch (DllNotFoundException) when (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Used in the .NET Framework, and in .NET Core unit tests
                return Environment.Is64BitProcess
                    ? (LibImpl)new Win64Impl()
                    : new Win32Impl();
            }
        }

        public static string GetErrorMessage(int errorCode)
        {
            const int bufferSize = 256;
            var buffer = stackalloc char[256];
            var messageLength = get_error_message(errorCode, buffer, bufferSize);
            return messageLength >= 0
                ? new string(buffer, 0, messageLength)
                : $"Unknown error, code: {errorCode}";
        }

        [StructLayout(LayoutKind.Sequential)]
        internal ref struct compile_input
        {
            public char* pattern;
            public uint pattern_length;
            public uint flags;
            public uint flags_jit;
            public uint new_line;
            public uint bsr;
            public uint parens_nest_limit;
            public uint max_pattern_length;
            public uint compile_extra_options;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal ref struct compile_result
        {
            public IntPtr code;
            public int error_code;
            public uint error_offset;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal ref struct match_input
        {
            public IntPtr code;
            public char* subject;
            public uint subject_length;
            public uint start_index;
            public uint additional_options;
            public uint match_limit;
            public uint depth_limit;
            public uint heap_limit;
            public uint offset_limit;
            public uint* output_vector;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal ref struct match_result
        {
            public int result_code;
        }
    }
}
