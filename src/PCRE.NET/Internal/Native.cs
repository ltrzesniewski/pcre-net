using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace PCRE.Internal;

internal unsafe partial interface INative
{
    string GetErrorMessage(int errorCode);

    // PCRE2_INFO_NAMETABLE returns a pointer to the first entry of the table. This is a PCRE2_SPTR pointer to a block of code units.
    // In the 8-bit library, the first two bytes of each entry are the number of the capturing parenthesis, most significant byte first.
    // In the 16-bit library, the pointer points to 16-bit code units, the first of which contains the parenthesis number.
    // In the 32-bit library, the pointer points to 32-bit code units, the first of which contains the parenthesis number.
    // The rest of the entry is the corresponding name, zero terminated.
    Dictionary<string, int[]> GetCaptureNames(void* nameEntryTable, uint nameCount, uint nameEntrySize);
}

internal readonly unsafe partial struct NativeStruct8Bit : INative
{
    public string GetErrorMessage(int errorCode)
        => Native8Bit.GetErrorMessage(errorCode);

    public Dictionary<string, int[]> GetCaptureNames(void* nameEntryTable, uint nameCount, uint nameEntrySize)
        => Native8Bit.GetCaptureNames(nameEntryTable, nameCount, nameEntrySize);
}

internal readonly unsafe partial struct NativeStruct16Bit : INative
{
    public string GetErrorMessage(int errorCode)
        => Native16Bit.GetErrorMessage(errorCode);

    public Dictionary<string, int[]> GetCaptureNames(void* nameEntryTable, uint nameCount, uint nameEntrySize)
        => Native16Bit.GetCaptureNames(nameEntryTable, nameCount, nameEntrySize);
}

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal static unsafe partial class Native8Bit
{
    public static string GetErrorMessage(int errorCode)
    {
        const int bufferSize = 512;
        var buffer = stackalloc byte[bufferSize];
        var messageLength = get_error_message(errorCode, buffer, bufferSize);
        return messageLength >= 0
            ? Encoding.ASCII.GetString(buffer, messageLength)
            : $"Unknown error, code: {errorCode}";
    }

    public static Dictionary<string, int[]> GetCaptureNames(void* nameEntryTable, uint nameCount, uint nameEntrySize)
    {
        // PCRE2_INFO_NAMETABLE returns a pointer to the first entry of the table. This is a PCRE2_SPTR pointer to a block of code units.
        // In the 8-bit library, the first two bytes of each entry are the number of the capturing parenthesis, most significant byte first.
        // The rest of the entry is the corresponding name, zero terminated.

        var captureNames = new Dictionary<string, int[]>((int)nameCount, StringComparer.Ordinal);
        var currentItem = (byte*)nameEntryTable;

        for (var i = 0; i < nameCount; ++i)
        {
            var groupIndex = currentItem[0] << 8 | currentItem[1];
            var groupName = GetString(currentItem + 2) ?? string.Empty;

            if (captureNames.TryGetValue(groupName, out var indexes))
            {
                Array.Resize(ref indexes, indexes.Length + 1);
                indexes[indexes.Length - 1] = groupIndex;
            }
            else
            {
                indexes = [groupIndex];
            }

            captureNames[groupName] = indexes;
            currentItem += nameEntrySize;
        }

        return captureNames;
    }

    public static string GetString(ReadOnlySpan<byte> value)
    {
        // TODO: The pattern may not be UTF-8

        fixed (byte* ptr = value)
            return Encoding.UTF8.GetString(ptr, value.Length);
    }

    public static string? GetString(byte* ptr)
    {
        if (ptr is null)
            return null;

        // TODO: The pattern may not be UTF-8
#if NET
        return Marshal.PtrToStringUTF8((IntPtr)ptr) ?? string.Empty;
#else
        return Encoding.UTF8.GetString(ptr, GetStringLength(ptr));

        static int GetStringLength(byte* value)
        {
            var start = value;

            while (*value != 0)
                ++value;

            return (int)(value - start);
        }
#endif
    }
}

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal static unsafe partial class Native16Bit
{
    public static string GetErrorMessage(int errorCode)
    {
        const int bufferSize = 256;
        var buffer = stackalloc char[bufferSize];
        var messageLength = get_error_message(errorCode, buffer, bufferSize);
        return messageLength >= 0
            ? new string(buffer, 0, messageLength)
            : $"Unknown error, code: {errorCode}";
    }

    public static Dictionary<string, int[]> GetCaptureNames(void* nameEntryTable, uint nameCount, uint nameEntrySize)
    {
        // PCRE2_INFO_NAMETABLE returns a pointer to the first entry of the table. This is a PCRE2_SPTR pointer to a block of code units.
        // In the 16-bit library, the pointer points to 16-bit code units, the first of which contains the parenthesis number.
        // The rest of the entry is the corresponding name, zero terminated.

        var captureNames = new Dictionary<string, int[]>((int)nameCount, StringComparer.Ordinal);
        var currentItem = (char*)nameEntryTable;

        for (var i = 0; i < nameCount; ++i)
        {
            var groupIndex = (int)*currentItem;
            var groupName = new string(currentItem + 1);

            if (captureNames.TryGetValue(groupName, out var indexes))
            {
                Array.Resize(ref indexes, indexes.Length + 1);
                indexes[indexes.Length - 1] = groupIndex;
            }
            else
            {
                indexes = [groupIndex];
            }

            captureNames[groupName] = indexes;
            currentItem += nameEntrySize;
        }

        return captureNames;
    }

    public static string? GetString(char* ptr)
        => ptr is not null ? new string(ptr) : null;
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static unsafe class Native
{
    // These structs need to be kept identical between the 8-bit and 16-bit versions:
    // same size, same alignment, no PCRE2_UCHAR field types without indirection.

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct compile_input
    {
        public void* pattern;
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
        public uint optimization_directives_count;
        public uint* optimization_directives;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct compile_result
    {
        public void* code;
        public int error_code;
        public uint error_offset;
        public uint capture_count;
        public uint name_count;
        public uint name_entry_size;
        public void* name_entry_table;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct match_settings
    {
        public uint match_limit;
        public uint depth_limit;
        public uint heap_limit;
        public uint offset_limit;
        public void* jit_stack;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct match_input
    {
        public void* code;
        public void* subject;
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
        public void* buffer;
        public void* subject;
        public uint subject_length;
        public uint start_index;
        public uint additional_options;
        public void* callout;
        public void* callout_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct dfa_match_input
    {
        public void* code;
        public void* subject;
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
        public void* code;
        public void* subject;
        public uint subject_length;
        public uint start_index;
        public uint additional_options;
        public match_settings settings;
        public void* replacement;
        public uint replacement_length;
        public void* buffer;
        public uint buffer_length;
        public void* match_callout;
        public void* substitute_callout;
        public void* substitute_case_callout;
        public void* callout_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct match_result
    {
        public int result_code;
        public void* mark;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct substitute_result
    {
        public int result_code;
        public void* output;
        public nuint output_length;
        public byte output_on_heap;
        public uint substitute_call_count;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct match_buffer_info
    {
        // Input
        public void* code;
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
        public void* mark; /* Pointer to current mark or NULL */
        public void* subject; /* The subject being matched */
        public nuint subject_length; /* The length of the subject */
        public nuint start_match; /* Offset to start of this match attempt */
        public nuint current_position; /* Where we currently are in the subject */
        public nuint pattern_position; /* Offset to next item in the pattern */
        public nuint next_item_length; /* Length of next item in the pattern */

        /* ------------------- Added for Version 1 -------------------------- */
        public nuint callout_string_offset; /* Offset to string within pattern */
        public nuint callout_string_length; /* Length of string compiled into pattern */
        public void* callout_string; /* String compiled into pattern */

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
        public void* input; /* Pointer to input subject string */
        public void* output; /* Pointer to output buffer */
        public nuint output_offset_start; /* Changed portion of the output */
        public nuint output_offset_end; /* Changed portion of the output */
        public nuint* ovector; /* Pointer to current ovector */
        public uint oveccount; /* Count of pairs set in ovector */
        public uint subscount; /* Substitution number */
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct convert_input
    {
        public void* pattern;
        public uint pattern_length;
        public uint options;
        public uint glob_escape;
        public uint glob_separator;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal ref struct convert_result
    {
        public void* output;
        public uint output_length;
    }
}
