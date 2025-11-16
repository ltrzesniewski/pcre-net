using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using PCRE.Dfa;

namespace PCRE.Internal;

internal abstract unsafe class InternalRegex : IDisposable
{
    internal const int MaxStackAllocCaptureCount = 32;
    internal const int SubstituteBufferSizeInChars = 4096;

    private Dictionary<int, PcreCalloutInfo>? _calloutInfoByPatternPosition;

    public void* Code { get; protected set; }

    internal Dictionary<string, int[]> CaptureNames { get; init; } = null!;
    internal int CaptureCount { get; init; }

    public int OutputVectorSize => 2 * (CaptureCount + 1);
    public bool CanStackAllocOutputVector => CaptureCount <= MaxStackAllocCaptureCount;

    public string PatternString { get; }
    public PcreRegexSettings Settings { get; }

    protected InternalRegex(string patternString, PcreRegexSettings settings)
    {
        PatternString = patternString;
        Settings = settings.AsReadOnly();
    }

    ~InternalRegex()
        => FreeCode();

    public void Dispose()
    {
        FreeCode();
        GC.SuppressFinalize(this);
    }

    protected abstract void FreeCode();

    public abstract IReadOnlyList<PcreCalloutInfo> GetCallouts();

    [return: NotNullIfNotNull(nameof(ptr))]
    public abstract string? GetString(void* ptr);

    public abstract uint GetInfoUInt32(uint key);
    public abstract nuint GetInfoNativeInt(uint key);

    public PcreCalloutInfo? TryGetCalloutInfoByPatternPosition(int patternPosition)
    {
        if (_calloutInfoByPatternPosition == null)
        {
            var dict = new Dictionary<int, PcreCalloutInfo>();

            foreach (var info in GetCallouts())
                dict.Add(info.PatternPosition, info);

            Thread.MemoryBarrier();
            _calloutInfoByPatternPosition = dict;
        }

        return _calloutInfoByPatternPosition.TryGetValue(patternPosition, out var result) ? result : null;
    }

    public PcreCalloutInfo GetCalloutInfoByPatternPosition(int patternPosition)
        => TryGetCalloutInfoByPatternPosition(patternPosition) ?? throw new InvalidOperationException($"Could not retrieve callout info at position {patternPosition}.");
}

internal abstract class InternalRegex<TChar>(string patternString, PcreRegexSettings settings)
    : InternalRegex(patternString, settings)
    where TChar : unmanaged;

internal abstract unsafe class InternalRegex<TChar, TNative> : InternalRegex<TChar>
    where TChar : unmanaged
    where TNative : struct, INative
{
    protected InternalRegex(ReadOnlySpan<TChar> pattern, string patternString, PcreRegexSettings settings)
        : base(patternString, settings)
    {
        Debug.Assert(typeof(TChar) == typeof(char) || typeof(TChar) == typeof(byte));

        Compile(pattern, out var captureCount, out var captureNames);
        CaptureCount = captureCount;
        CaptureNames = captureNames;

        GC.KeepAlive(this);
    }

    private void Compile(ReadOnlySpan<TChar> pattern,
                         out int captureCount,
                         out Dictionary<string, int[]> captureNames)
    {
        Native.compile_result result;

        fixed (TChar* pPattern = pattern)
        {
            Native.compile_input input;
            _ = &input;

            input.pattern = pPattern;
            input.pattern_length = (uint)pattern.Length;

            using (Settings.FillCompileInput(ref input))
            {
                default(TNative).compile(&input, &result);
                Code = result.code;
            }

            if (Code == null || result.error_code != 0)
            {
                Dispose();
                throw new PcrePatternException((PcreErrorCode)result.error_code, $"Invalid pattern '{PatternString}': {default(TNative).GetErrorMessage(result.error_code)} at offset {result.error_offset}.");
            }
        }

        captureCount = (int)result.capture_count;
        captureNames = GetCaptureNames(result.name_entry_table, result.name_count, result.name_entry_size);
    }

    protected override void FreeCode()
    {
        if (Code != null)
        {
            default(TNative).code_free(Code);
            Code = null;
        }
    }

    public void Match(ref Span<nuint> matchOVector,
                      ReadOnlySpan<TChar> subject,
                      PcreMatchSettings settings,
                      int startIndex,
                      uint additionalOptions,
                      Delegate? callout,
                      nuint[]? calloutOutputVector,
                      out TChar* markPtr,
                      out int resultCode)
    {
        Native.match_input input;
        _ = &input;

        settings.FillMatchSettings(ref input.settings, out var jitStack);

        Native.match_result result;
        CalloutInterop.CalloutInteropInfo<TChar> calloutInterop;

        var oVectorArray = default(nuint[]);

        var oVector = matchOVector.Length == OutputVectorSize
            ? matchOVector
            : CanStackAllocOutputVector
                ? stackalloc nuint[OutputVectorSize]
                : oVectorArray = new nuint[OutputVectorSize];

        fixed (TChar* pSubject = subject)
        fixed (nuint* pOVec = &oVector[0])
        {
            input.code = Code;
            input.subject = pSubject;
            input.subject_length = (uint)subject.Length;
            input.output_vector = pOVec;
            input.start_index = (uint)startIndex;
            input.additional_options = additionalOptions;

            CalloutInterop.Prepare(subject, this, ref input, out calloutInterop, callout, calloutOutputVector);

            default(TNative).match(&input, &result);

            GC.KeepAlive(this);
            GC.KeepAlive(jitStack);
        }

        if (result.result_code < PcreConstants.PCRE2_ERROR_PARTIAL)
            HandleError(result, ref calloutInterop);

        if (result.result_code != PcreConstants.PCRE2_ERROR_NOMATCH && oVector != matchOVector)
            oVectorArray ??= oVector.ToArray();

        if (oVectorArray != null)
            matchOVector = oVectorArray;

        markPtr = (TChar*)result.mark;
        resultCode = result.result_code;
    }

    public void BufferMatch(ReadOnlySpan<TChar> subject,
                            IPcreMatchBuffer buffer,
                            int startIndex,
                            uint additionalOptions,
                            Delegate? callout,
                            out TChar* markPtr,
                            out int resultCode)
    {
        Native.buffer_match_input input;
        _ = &input;

        Native.match_result result;
        CalloutInterop.CalloutInteropInfo<TChar> calloutInterop;

        fixed (TChar* pSubject = subject)
        {
            input.buffer = (void*)buffer.NativeBuffer;
            input.subject = pSubject;
            input.subject_length = (uint)subject.Length;
            input.start_index = (uint)startIndex;
            input.additional_options = additionalOptions;
            input.callout = null;

            if (input.buffer == null)
                ThrowMatchBufferDisposed();

            CalloutInterop.Prepare(subject, buffer, ref input, out calloutInterop, callout);

            default(TNative).buffer_match(&input, &result);

            GC.KeepAlive(buffer); // The buffer keeps alive all the other required stuff
        }

        if (result.result_code < PcreConstants.PCRE2_ERROR_PARTIAL)
            HandleError(result, ref calloutInterop);

        markPtr = (TChar*)result.mark;
        resultCode = result.result_code;
        return;

        static void ThrowMatchBufferDisposed()
            => throw new ObjectDisposedException("The match buffer has been disposed");
    }

    protected static void HandleError(in Native.match_result result, ref CalloutInterop.CalloutInteropInfo<TChar> calloutInterop)
    {
        switch (result.result_code)
        {
            case PcreConstants.PCRE2_ERROR_NOMATCH:
            case PcreConstants.PCRE2_ERROR_PARTIAL:
                break;

            case PcreConstants.PCRE2_ERROR_CALLOUT:
                throw new PcreCalloutException("An exception was thrown by the callout: " + calloutInterop.Exception?.Message, calloutInterop.Exception);

            default:
                if (result.result_code < 0)
                    throw new PcreMatchException((PcreErrorCode)result.result_code);

                break;
        }
    }

    public override uint GetInfoUInt32(uint key)
    {
        uint result;
        var errorCode = default(TNative).pattern_info(Code, key, &result);

        GC.KeepAlive(this);

        if (errorCode != 0)
            throw new PcreException((PcreErrorCode)errorCode, $"Error in pcre2_pattern_info: {default(TNative).GetErrorMessage(errorCode)}");

        return result;
    }

    public override nuint GetInfoNativeInt(uint key)
    {
        nuint result;
        var errorCode = default(TNative).pattern_info(Code, key, &result);

        GC.KeepAlive(this);

        if (errorCode != 0)
            throw new PcreException((PcreErrorCode)errorCode, $"Error in pcre2_pattern_info: {default(TNative).GetErrorMessage(errorCode)}");

        return result;
    }

    public override IReadOnlyList<PcreCalloutInfo> GetCallouts()
    {
        var calloutCount = default(TNative).get_callout_count(Code);
        if (calloutCount == 0)
            return [];

        var data = calloutCount <= 16
            ? stackalloc Native.pcre2_callout_enumerate_block[(int)calloutCount]
            : new Native.pcre2_callout_enumerate_block[calloutCount];

        fixed (Native.pcre2_callout_enumerate_block* pData = &data[0])
            default(TNative).get_callouts(Code, pData);

        var result = new List<PcreCalloutInfo>((int)calloutCount);

        for (var i = 0; i < data.Length; ++i)
            result.Add(new PcreCalloutInfo(this, ref data[i]));

        GC.KeepAlive(this);
        return result.AsReadOnly();
    }

    /// <summary>
    /// Retrieves the capture names from the name entry table.
    /// </summary>
    /// <remarks>
    /// PCRE2_INFO_NAMETABLE returns a pointer to the first entry of the table. This is a PCRE2_SPTR pointer to a block of code units.
    /// In the 8-bit library, the first two bytes of each entry are the number of the capturing parenthesis, most significant byte first.
    /// In the 16-bit library, the pointer points to 16-bit code units, the first of which contains the parenthesis number.
    /// In the 32-bit library, the pointer points to 32-bit code units, the first of which contains the parenthesis number.
    /// The rest of the entry is the corresponding name, zero terminated.
    /// </remarks>
    protected abstract Dictionary<string, int[]> GetCaptureNames(void* nameEntryTable, uint nameCount, uint nameEntrySize);

    public override string ToString()
        => PatternString;
}

internal sealed unsafe class InternalRegex8Bit(ReadOnlySpan<byte> pattern, string patternString, PcreRegexSettings settings, Encoding encoding)
    : InternalRegex<byte, Native8Bit>(pattern, patternString, settings)
{
    public override string? GetString(void* ptr)
        => GetString((byte*)ptr, encoding);

    private static string? GetString(byte* ptr, Encoding encoding)
    {
        if (ptr is null)
            return null;
#if NET
        if (ReferenceEquals(encoding, Encoding.UTF8))
            return Marshal.PtrToStringUTF8((IntPtr)ptr) ?? string.Empty;
#endif
        return encoding.GetString(ptr, GetStringLength(ptr));

        static int GetStringLength(byte* ptr)
        {
            var start = ptr;

            while (*ptr != 0)
                ++ptr;

            return (int)(ptr - start);
        }
    }

    protected override Dictionary<string, int[]> GetCaptureNames(void* nameEntryTable, uint nameCount, uint nameEntrySize)
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
}

internal sealed unsafe class InternalRegex16Bit(string pattern, PcreRegexSettings settings)
    : InternalRegex<char, Native16Bit>(pattern, pattern, settings)
{
    private PcreMatch? _noMatch;

    public PcreMatch Match(string subject,
                           PcreMatchSettings settings,
                           int startIndex,
                           uint additionalOptions,
                           Func<PcreCallout, PcreCalloutResult>? callout)
    {
        Native.match_input input;
        _ = &input;

        settings.FillMatchSettings(ref input.settings, out var jitStack);

        Native.match_result result;
        CalloutInterop.CalloutInteropInfo<char> calloutInterop;

        var oVectorArray = default(nuint[]);

        var oVector = CanStackAllocOutputVector
            ? stackalloc nuint[OutputVectorSize]
            : oVectorArray = new nuint[OutputVectorSize];

        fixed (char* pSubject = subject)
        fixed (nuint* pOVec = &oVector[0])
        {
            input.code = Code;
            input.subject = pSubject;
            input.subject_length = (uint)subject.Length;
            input.output_vector = pOVec;
            input.start_index = (uint)startIndex;
            input.additional_options = additionalOptions;

            CalloutInterop.Prepare(subject, this, ref input, out calloutInterop, callout);

            default(Native16Bit).match(&input, &result);

            GC.KeepAlive(this);
            GC.KeepAlive(jitStack);
        }

        if (result.result_code == PcreConstants.PCRE2_ERROR_NOMATCH)
            return _noMatch ??= new PcreMatch(this);

        if (result.result_code < PcreConstants.PCRE2_ERROR_PARTIAL)
            HandleError(result, ref calloutInterop);

        oVectorArray ??= oVector.ToArray();
        return new PcreMatch(subject, this, ref result, oVectorArray);
    }

    public PcreDfaMatchResult DfaMatch(string subject, PcreDfaMatchSettings settings, int startIndex, uint additionalOptions)
    {
        Native.dfa_match_input input;
        _ = &input;

        settings.FillMatchInput(ref input);

        var oVector = new nuint[2 * Math.Max(1, settings.MaxResults)];
        Native.match_result result;
        CalloutInterop.CalloutInteropInfo<char> calloutInterop;

        fixed (char* pSubject = subject)
        fixed (nuint* pOVec = &oVector[0])
        {
            input.code = Code;
            input.subject = pSubject;
            input.subject_length = (uint)subject.Length;
            input.output_vector = pOVec;
            input.start_index = (uint)startIndex;
            input.additional_options = additionalOptions;

            CalloutInterop.Prepare(subject, this, ref input, out calloutInterop, settings.Callout);

            default(Native16Bit).dfa_match(&input, &result);

            GC.KeepAlive(this);
        }

        if (result.result_code < PcreConstants.PCRE2_ERROR_PARTIAL)
            HandleError(result, ref calloutInterop);

        return new PcreDfaMatchResult(subject, ref result, oVector);
    }

    public string Substitute(ReadOnlySpan<char> subject,
                             string? subjectAsString,
                             ReadOnlySpan<char> replacement,
                             PcreMatchSettings? settings,
                             int startIndex,
                             uint additionalOptions,
                             PcreRefCalloutFunc? matchCallout,
                             PcreSubstituteCalloutFunc? substituteCallout,
                             PcreSubstituteCaseCalloutFunc? substituteCaseCallout,
                             out uint substituteCallCount)
    {
        Debug.Assert(subjectAsString is null || subjectAsString.AsSpan() == subject);

        Native.substitute_input input;
        _ = &input;

        (settings ?? PcreMatchSettings.Default).FillMatchSettings(ref input.settings, out var jitStack);

        Native.substitute_result result;
        CalloutInterop.SubstituteCalloutInteropInfo calloutInterop;

        var buffer = stackalloc char[SubstituteBufferSizeInChars];
        input.buffer = buffer;
        input.buffer_length = SubstituteBufferSizeInChars;

        fixed (char* pSubject = subject)
        fixed (char* pReplacement = replacement)
        {
            input.code = Code;
            input.subject = pSubject;
            input.subject_length = (uint)subject.Length;
            input.start_index = (uint)startIndex;
            input.additional_options = additionalOptions;
            input.replacement = pReplacement;
            input.replacement_length = (uint)replacement.Length;

            CalloutInterop.PrepareSubstitute(this, subject, ref input, out calloutInterop, matchCallout, substituteCallout, substituteCaseCallout);

            default(Native16Bit).substitute(&input, &result);

            GC.KeepAlive(this);
            GC.KeepAlive(jitStack);

            substituteCallCount = result.substitute_call_count;
        }

        try
        {
            if (calloutInterop.Exception is { } ex)
            {
                if (result.result_code == PcreConstants.PCRE2_ERROR_REPLACECASE)
                    throw new PcreCalloutException(PcreErrorCode.ReplaceCase, "An exception was thrown by the case-transformation callout: " + ex.Message, ex);

                throw new PcreCalloutException("An exception was thrown by the callout: " + ex.Message, ex);
            }

            switch (result.result_code)
            {
                case < 0: // An error occured
                    throw new PcreSubstituteException((PcreErrorCode)result.result_code);

                case 0: // No substitution was made, avoid allocating a new string if possible
                    if ((additionalOptions & PcreConstants.PCRE2_SUBSTITUTE_REPLACEMENT_ONLY) != 0)
                        return string.Empty;

                    return subjectAsString ?? subject.ToString();

                default: // At least one substitution was made, return the result as a new string
                    if (result.output_length > int.MaxValue)
                        throw new PcreSubstituteException(PcreErrorCode.Internal, "Invalid output string length", null);

                    return new string((char*)result.output, 0, (int)result.output_length);
            }
        }
        finally
        {
            if (result.output != null && result.output_on_heap != 0)
                default(Native16Bit).substitute_result_free(&result);
        }
    }

    public override string? GetString(void* ptr)
        => ptr is not null ? new string((char*)ptr) : null;

    protected override Dictionary<string, int[]> GetCaptureNames(void* nameEntryTable, uint nameCount, uint nameEntrySize)
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
}
