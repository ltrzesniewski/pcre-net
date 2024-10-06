using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace PCRE.Internal;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("Security", "CA5392")]
internal static unsafe partial class Native
{
#if !NET
    private static readonly LibImpl _impl = GetLibImpl();

    private static LibImpl GetLibImpl()
    {
        try
        {
            var impl = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new WinImpl()
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? new LinuxImpl()
                    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                        ? (LibImpl)new OSXImpl()
                        : throw new PlatformNotSupportedException();

            impl.get_error_message(0, null, 0);
            return impl;
        }
        catch (DllNotFoundException) when (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Used in the .NET Framework
            return Environment.Is64BitProcess
                ? new Win64Impl()
                : new Win32Impl();
        }
    }
#endif

    public static string GetErrorMessage(int errorCode)
    {
        const int bufferSize = 256;
        var buffer = stackalloc char[bufferSize];
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
        public uint max_var_lookbehind;
        public uint max_pattern_compiled_length;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct compile_result
    {
        public IntPtr code;
        public int error_code;
        public uint error_offset;
        public uint capture_count;
        public uint name_count;
        public uint name_entry_size;
        public char* name_entry_table;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct match_settings
    {
        public uint match_limit;
        public uint depth_limit;
        public uint heap_limit;
        public uint offset_limit;
        public IntPtr jit_stack;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct match_input
    {
        public IntPtr code;
        public char* subject;
        public uint subject_length;
        public uint start_index;
        public uint additional_options;
        public match_settings settings;
        public nuint* output_vector;
        public void* callout;
        public void* callout_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct buffer_match_input
    {
        public IntPtr buffer;
        public char* subject;
        public uint subject_length;
        public uint start_index;
        public uint additional_options;
        public void* callout;
        public void* callout_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct dfa_match_input
    {
        public IntPtr code;
        public char* subject;
        public uint subject_length;
        public uint start_index;
        public uint additional_options;
        public nuint* output_vector;
        public void* callout;
        public void* callout_data;
        public uint max_results;
        public uint workspace_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct substitute_input
    {
        public IntPtr code;
        public char* subject;
        public uint subject_length;
        public uint start_index;
        public uint additional_options;
        public match_settings settings;
        public char* replacement;
        public uint replacement_length;
        public char* buffer;
        public uint buffer_length;
        public void* match_callout;
        public void* substitute_callout;
        public void* callout_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct match_result
    {
        public int result_code;
        public char* mark;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct substitute_result
    {
        public int result_code;
        public char* output;
        public nuint output_length;
        public byte output_on_heap;
        public uint substitute_call_count;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct match_buffer_info
    {
        // Input
        public IntPtr code;
        public match_settings settings;

        // Output
        public nuint* output_vector;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct pcre2_callout_block
    {
        public uint version;

        /* ------------------------ Version 0 ------------------------------- */
        public uint callout_number; /* Number compiled into pattern */
        public uint capture_top; /* Max current capture */
        public uint capture_last; /* Most recently closed capture */
        public nuint* offset_vector; /* The offset vector */
        public char* mark; /* Pointer to current mark or NULL */
        public char* subject; /* The subject being matched */
        public nuint subject_length; /* The length of the subject */
        public nuint start_match; /* Offset to start of this match attempt */
        public nuint current_position; /* Where we currently are in the subject */
        public nuint pattern_position; /* Offset to next item in the pattern */
        public nuint next_item_length; /* Length of next item in the pattern */

        /* ------------------- Added for Version 1 -------------------------- */
        public nuint callout_string_offset; /* Offset to string within pattern */
        public nuint callout_string_length; /* Length of string compiled into pattern */
        public char* callout_string; /* String compiled into pattern */

        /* ------------------- Added for Version 2 -------------------------- */
        public uint callout_flags; /* See above for list */
        /* ------------------------------------------------------------------ */
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct pcre2_callout_enumerate_block
    {
        public uint version; /* Identifies version of block */

        /* ------------------------ Version 0 ------------------------------- */
        public nuint pattern_position; /* Offset to next item in the pattern */
        public nuint next_item_length; /* Length of next item in the pattern */
        public uint callout_number; /* Number compiled into pattern */
        public nuint callout_string_offset; /* Offset to string within pattern */
        public nuint callout_string_length; /* Length of string compiled into pattern */
        public IntPtr callout_string; /* String compiled into pattern */
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct pcre2_substitute_callout_block
    {
        public uint version; /* Identifies version of block */

        /* ------------------------ Version 0 ------------------------------- */
        public char* input; /* Pointer to input subject string */
        public char* output; /* Pointer to output buffer */
        public nuint output_offset_start; /* Changed portion of the output */
        public nuint output_offset_end; /* Changed portion of the output */
        public nuint* ovector; /* Pointer to current ovector */
        public uint oveccount; /* Count of pairs set in ovector */
        public uint subscount; /* Substitution number */
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct convert_input
    {
        public char* pattern;
        public uint pattern_length;
        public uint options;
        public uint glob_escape;
        public uint glob_separator;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct convert_result
    {
        public char* output;
        public uint output_length;
    }
}
