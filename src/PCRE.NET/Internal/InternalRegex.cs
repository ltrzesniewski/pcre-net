using System;
using System.Collections.Generic;
using System.Threading;
using PCRE.Dfa;

namespace PCRE.Internal;

internal sealed unsafe class InternalRegex : IDisposable
{
    internal const int MaxStackAllocCaptureCount = 32;

    private Dictionary<int, PcreCalloutInfo>? _calloutInfoByPatternPosition;
    private PcreMatch? _noMatch;

    public IntPtr Code { get; private set; }
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

        Native.compile_result result;

        fixed (char* pPattern = pattern)
        {
            Native.compile_input input;
            _ = &input;

            input.pattern = pPattern;
            input.pattern_length = (uint)pattern.Length;

            Settings.FillCompileInput(ref input);

            Native.compile(&input, &result);
            Code = result.code;

            if (Code == IntPtr.Zero || result.error_code != 0)
            {
                Dispose();
                throw new PcrePatternException((PcreErrorCode)result.error_code, $"Invalid pattern '{pattern}': {Native.GetErrorMessage(result.error_code)} at offset {result.error_offset}.");
            }
        }

        CaptureCount = (int)result.capture_count;
        CaptureNames = new Dictionary<string, int[]>((int)result.name_count, StringComparer.Ordinal);

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
        if (Code != IntPtr.Zero)
        {
            Native.code_free(Code);
            Code = IntPtr.Zero;
        }
    }

    public uint GetInfoUInt32(uint key)
    {
        uint result;
        var errorCode = Native.pattern_info(Code, key, &result);

        if (errorCode != 0)
            throw new PcreException((PcreErrorCode)errorCode, $"Error in pcre2_pattern_info: {Native.GetErrorMessage(errorCode)}");

        return result;
    }

    public nuint GetInfoNativeInt(uint key)
    {
        nuint result;
        var errorCode = Native.pattern_info(Code, key, &result);

        if (errorCode != 0)
            throw new PcreException((PcreErrorCode)errorCode, $"Error in pcre2_pattern_info: {Native.GetErrorMessage(errorCode)}");

        return result;
    }

    public PcreMatch Match(string subject,
                           PcreMatchSettings settings,
                           int startIndex,
                           uint additionalOptions,
                           Func<PcreCallout, PcreCalloutResult>? callout)
    {
        Native.match_input input;
        _ = &input;

        settings.FillMatchSettings(ref input.settings);

        Native.match_result result;
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

            Native.match(&input, &result);
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
        Native.match_input input;
        _ = &input;

        settings.FillMatchSettings(ref input.settings);

        Native.match_result result;
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

            Native.match(&input, &result);
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
        Native.buffer_match_input input;
        _ = &input;

        Native.match_result result;
        CalloutInterop.CalloutInteropInfo calloutInterop;

        fixed (char* pSubject = subject)
        {
            input.buffer = buffer.NativeBuffer;
            input.subject = pSubject;
            input.subject_length = (uint)subject.Length;
            input.start_index = (uint)startIndex;
            input.additional_options = additionalOptions;
            input.callout = null;

            if (input.buffer == IntPtr.Zero)
                ThrowMatchBufferDisposed();

            CalloutInterop.Prepare(subject, buffer, ref input, out calloutInterop, callout);

            Native.buffer_match(&input, &result);
        }

        if (result.result_code < PcreConstants.ERROR_PARTIAL)
            HandleError(result, ref calloutInterop);

        match.Update(subject, result, null);
    }

    public PcreDfaMatchResult DfaMatch(string subject, PcreDfaMatchSettings settings, int startIndex, uint additionalOptions)
    {
        Native.dfa_match_input input;
        _ = &input;

        settings.FillMatchInput(ref input);

        var oVector = new nuint[2 * Math.Max(1, settings.MaxResults)];
        Native.match_result result;
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

            Native.dfa_match(&input, &result);
        }

        if (result.result_code < PcreConstants.ERROR_PARTIAL)
            HandleError(result, ref calloutInterop);

        return new PcreDfaMatchResult(subject, ref result, oVector);
    }

    public string Substitute(string subject, string replacement, uint additionalOptions, int startIndex)
    {
        Native.substitute_input input;
        _ = &input;

        Native.substitute_result result;

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

            Native.substitute(&input, &result);
        }

        try
        {
            return result.result_code switch
            {
                0   => (additionalOptions & PcreConstants.SUBSTITUTE_REPLACEMENT_ONLY) != 0 ? string.Empty : subject,
                < 0 => throw new PcreMatchException((PcreErrorCode)result.result_code),
                _   => new string(result.output, 0, (int)result.output_length)
            };
        }
        finally
        {
            Native.substitute_result_free(&result);
        }
    }

    private static void HandleError(in Native.match_result result, ref CalloutInterop.CalloutInteropInfo calloutInterop)
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
        var calloutCount = Native.get_callout_count(Code);
        if (calloutCount == 0)
            return [];

        var data = calloutCount <= 16
            ? stackalloc Native.pcre2_callout_enumerate_block[(int)calloutCount]
            : new Native.pcre2_callout_enumerate_block[calloutCount];

        fixed (Native.pcre2_callout_enumerate_block* pData = &data[0])
        {
            Native.get_callouts(Code, pData);
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
