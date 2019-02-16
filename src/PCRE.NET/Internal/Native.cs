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

                impl.test();
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct compile_input
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
        internal struct compile_result
        {
            public IntPtr code;
            public uint error_code;
            public uint error_offset;
        }
    }
}
