using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using PCRE.Dfa;

namespace PCRE.Internal;

internal sealed unsafe class InternalRegex : IDisposable
{
    internal const int MaxStackAllocCaptureCount = 32;
    internal const int SubstituteBufferSizeInChars = 4096;

    private Dictionary<int, PcreCalloutInfo>? _calloutInfoByPatternPosition;
    private PcreMatch? _noMatch;

    public void* Code { get; private set; }
    public string Pattern { get; }
    public PcreRegexSettings Settings { get; }

    public Dictionary<string, int[]> CaptureNames { get; }
    public int CaptureCount { get; }

    public int OutputVectorSize => 2 * (CaptureCount + 1);
    public bool CanStackAllocOutputVector => CaptureCount <= MaxStackAllocCaptureCount;

    public InternalRegex(string pattern, PcreRegexSettings settings)
    {
        Pattern = pattern;
        Settings = settings.AsReadOnly();

        Native16Bit.compile_result result;

        fixed (char* pPattern = pattern)
        {
            Native16Bit.compile_input input;
            _ = &input;

            input.pattern = pPattern;
            input.pattern_length = (uint)pattern.Length;

            using (Settings.FillCompileInput(ref input))
            {
                Native16Bit.compile(&input, &result);
                Code = result.code;
            }

            if (Code == null || result.error_code != 0)
            {
                Dispose();
                throw new PcrePatternException((PcreErrorCode)result.error_code, $"Invalid pattern '{pattern}': {Native16Bit.GetErrorMessage(result.error_code)} at offset {result.error_offset}.");
            }
        }

        CaptureCount = (int)result.capture_count;
        CaptureNames = new Dictionary<string, int[]>((int)result.name_count, StringComparer.Ordinal);

        // PCRE2_INFO_NAMETABLE returns a pointer to the first entry of the table. This is a PCRE2_SPTR pointer to a block of code units.
        // In the 8-bit library, the first two bytes of each entry are the number of the capturing parenthesis, most significant byte first.
        // In the 16-bit library, the pointer points to 16-bit code units, the first of which contains the parenthesis number.
        // In the 32-bit library, the pointer points to 32-bit code units, the first of which contains the parenthesis number.
        // The rest of the entry is the corresponding name, zero terminated.
        var currentItem = result.name_entry_table;

        for (var i = 0; i < result.name_count; ++i)
        {
            var groupIndex = (int)*currentItem;
            var groupName = new string(currentItem + 1);

            if (CaptureNames.TryGetValue(groupName, out var indexes))
            {
                Array.Resize(ref indexes, indexes.Length + 1);
                indexes[indexes.Length - 1] = groupIndex;
            }
            else
            {
                indexes = [groupIndex];
            }

            CaptureNames[groupName] = indexes;

            currentItem += result.name_entry_size;
        }
    }

    ~InternalRegex()
        => FreeCode();

    public void Dispose()
    {
        FreeCode();
        GC.SuppressFinalize(this);
    }

    private void FreeCode()
    {
        if (Code != null)
        {
            Native16Bit.code_free(Code);
            Code = null;
        }
    }

    public uint GetInfoUInt32(uint key)
    {
        uint result;
        var errorCode = Native16Bit.pattern_info(Code, key, &result);

        GC.KeepAlive(this);

        if (errorCode != 0)
            throw new PcreException((PcreErrorCode)errorCode, $"Error in pcre2_pattern_info: {Native16Bit.GetErrorMessage(errorCode)}");

        return result;
    }

    public nuint GetInfoNativeInt(uint key)
    {
        nuint result;
        var errorCode = Native16Bit.pattern_info(Code, key, &result);

        GC.KeepAlive(this);

        if (errorCode != 0)
            throw new PcreException((PcreErrorCode)errorCode, $"Error in pcre2_pattern_info: {Native16Bit.GetErrorMessage(errorCode)}");

        return result;
    }

    public PcreMatch Match(string subject,
                           PcreMatchSettings settings,
                           int startIndex,
                           uint additionalOptions,
                           Func<PcreCallout, PcreCalloutResult>? callout)
    {
        Native16Bit.match_input input;
        _ = &input;

        settings.FillMatchSettings(ref input.settings, out var jitStack);

        Native16Bit.match_result result;
        CalloutInterop.CalloutInteropInfo calloutInterop;

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

            Native16Bit.match(&input, &result);

            GC.KeepAlive(this);
            GC.KeepAlive(jitStack);
        }

        if (result.result_code == PcreConstants.ERROR_NOMATCH)
            return _noMatch ??= new PcreMatch(this);

        if (result.result_code < PcreConstants.ERROR_PARTIAL)
            HandleError(result, ref calloutInterop);

        oVectorArray ??= oVector.ToArray();
        return new PcreMatch(subject, this, ref result, oVectorArray);
    }

    public void Match(ref PcreRefMatch match,
                      ReadOnlySpan<char> subject,
                      PcreMatchSettings settings,
                      int startIndex,
                      uint additionalOptions,
                      PcreRefCalloutFunc? callout,
                      nuint[]? calloutOutputVector)
    {
        Native16Bit.match_input input;
        _ = &input;

        settings.FillMatchSettings(ref input.settings, out var jitStack);

        Native16Bit.match_result result;
        CalloutInterop.CalloutInteropInfo calloutInterop;

        var oVectorArray = default(nuint[]);

        var oVector = match.OutputVector.Length == OutputVectorSize
            ? match.OutputVector
            : CanStackAllocOutputVector
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

            CalloutInterop.Prepare(subject, this, ref input, out calloutInterop, callout, calloutOutputVector);

            Native16Bit.match(&input, &result);

            GC.KeepAlive(this);
            GC.KeepAlive(jitStack);
        }

        if (result.result_code < PcreConstants.ERROR_PARTIAL)
            HandleError(result, ref calloutInterop);

        if (result.result_code != PcreConstants.ERROR_NOMATCH && oVector != match.OutputVector)
            oVectorArray ??= oVector.ToArray();

        match.Update(subject, result, oVectorArray);
    }

    public void BufferMatch(ref PcreRefMatch match,
                            ReadOnlySpan<char> subject,
                            PcreMatchBuffer buffer,
                            int startIndex,
                            uint additionalOptions,
                            PcreRefCalloutFunc? callout)
    {
        Native16Bit.buffer_match_input input;
        _ = &input;

        Native16Bit.match_result result;
        CalloutInterop.CalloutInteropInfo calloutInterop;

        fixed (char* pSubject = subject)
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

            Native16Bit.buffer_match(&input, &result);

            GC.KeepAlive(buffer); // The buffer keeps alive all the other required stuff
        }

        if (result.result_code < PcreConstants.ERROR_PARTIAL)
            HandleError(result, ref calloutInterop);

        match.Update(subject, result, null);
    }

    public PcreDfaMatchResult DfaMatch(string subject, PcreDfaMatchSettings settings, int startIndex, uint additionalOptions)
    {
        Native16Bit.dfa_match_input input;
        _ = &input;

        settings.FillMatchInput(ref input);

        var oVector = new nuint[2 * Math.Max(1, settings.MaxResults)];
        Native16Bit.match_result result;
        CalloutInterop.CalloutInteropInfo calloutInterop;

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

            Native16Bit.dfa_match(&input, &result);

            GC.KeepAlive(this);
        }

        if (result.result_code < PcreConstants.ERROR_PARTIAL)
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

        Native16Bit.substitute_input input;
        _ = &input;

        (settings ?? PcreMatchSettings.Default).FillMatchSettings(ref input.settings, out var jitStack);

        Native16Bit.substitute_result result;
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

            Native16Bit.substitute(&input, &result);

            GC.KeepAlive(this);
            GC.KeepAlive(jitStack);

            substituteCallCount = result.substitute_call_count;
        }

        try
        {
            if (calloutInterop.Exception is { } ex)
            {
                if (result.result_code == PcreConstants.ERROR_REPLACECASE)
                    throw new PcreCalloutException(PcreErrorCode.ReplaceCase, "An exception was thrown by the case-transformation callout: " + ex.Message, ex);

                throw new PcreCalloutException("An exception was thrown by the callout: " + ex.Message, ex);
            }

            switch (result.result_code)
            {
                case < 0: // An error occured
                    throw new PcreSubstituteException((PcreErrorCode)result.result_code);

                case 0: // No substitution was made, avoid allocating a new string if possible
                    if ((additionalOptions & PcreConstants.SUBSTITUTE_REPLACEMENT_ONLY) != 0)
                        return string.Empty;

                    return subjectAsString ?? subject.ToString();

                default: // At least one substitution was made, return the result as a new string
                    if (result.output_length > int.MaxValue)
                        throw new PcreSubstituteException(PcreErrorCode.Internal, "Invalid output string length", null);

                    return new string(result.output, 0, (int)result.output_length);
            }
        }
        finally
        {
            if (result.output != null && result.output_on_heap != 0)
                Native16Bit.substitute_result_free(&result);
        }
    }

    private static void HandleError(in Native16Bit.match_result result, ref CalloutInterop.CalloutInteropInfo calloutInterop)
    {
        switch (result.result_code)
        {
            case PcreConstants.ERROR_NOMATCH:
            case PcreConstants.ERROR_PARTIAL:
                break;

            case PcreConstants.ERROR_CALLOUT:
                throw new PcreCalloutException("An exception was thrown by the callout: " + calloutInterop.Exception?.Message, calloutInterop.Exception);

            default:
                if (result.result_code < 0)
                    throw new PcreMatchException((PcreErrorCode)result.result_code);

                break;
        }
    }

    public IReadOnlyList<PcreCalloutInfo> GetCallouts()
    {
        var calloutCount = Native16Bit.get_callout_count(Code);
        if (calloutCount == 0)
            return [];

        var data = calloutCount <= 16
            ? stackalloc Native16Bit.pcre2_callout_enumerate_block[(int)calloutCount]
            : new Native16Bit.pcre2_callout_enumerate_block[calloutCount];

        fixed (Native16Bit.pcre2_callout_enumerate_block* pData = &data[0])
        {
            Native16Bit.get_callouts(Code, pData);

            GC.KeepAlive(this);
        }

        var result = new List<PcreCalloutInfo>((int)calloutCount);

        for (var i = 0; i < data.Length; ++i)
            result.Add(new PcreCalloutInfo(ref data[i]));

        return result.AsReadOnly();
    }

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

    private static void ThrowMatchBufferDisposed()
        => throw new ObjectDisposedException("The match buffer has been disposed");
}
