using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

internal abstract class InternalRegex<TChar> : InternalRegex
    where TChar : unmanaged;

internal abstract unsafe class InternalRegex<TChar, TNative> : InternalRegex<TChar>
    where TChar : unmanaged
    where TNative : struct, INative
{
    private readonly TChar[] _pattern;
    private string? _patternString;

    public ReadOnlySpan<TChar> Pattern => _pattern;

    public string PatternString
    {
        get
        {
            if (_patternString == null)
            {
                if (typeof(TChar) == typeof(char))
                {
                    _patternString = Pattern.ToString();
                }
                else
                {
                    fixed (byte* ptr = (byte[])(object)_pattern)
                        _patternString = GetString(ptr);
                }
            }

            return _patternString;
        }

        protected init => _patternString = value;
    }

    public PcreRegexSettings Settings { get; }

    protected InternalRegex(ReadOnlySpan<TChar> pattern, PcreRegexSettings settings)
    {
        Debug.Assert(typeof(TChar) == typeof(char) || typeof(TChar) == typeof(byte));

        _pattern = pattern.ToArray();
        Settings = settings.AsReadOnly();

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
        captureNames = default(TNative).GetCaptureNames(result.name_entry_table, result.name_count, result.name_entry_size);
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

            default(NativeStruct16Bit).match(&input, &result);

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

    public uint GetInfoUInt32(uint key)
    {
        uint result;
        var errorCode = default(TNative).pattern_info(Code, key, &result);

        GC.KeepAlive(this);

        if (errorCode != 0)
            throw new PcreException((PcreErrorCode)errorCode, $"Error in pcre2_pattern_info: {default(TNative).GetErrorMessage(errorCode)}");

        return result;
    }

    public nuint GetInfoNativeInt(uint key)
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

    public override string ToString()
        => PatternString;

    protected static void ThrowMatchBufferDisposed()
        => throw new ObjectDisposedException("The match buffer has been disposed");
}

internal sealed unsafe class InternalRegex16Bit : InternalRegex<char, NativeStruct16Bit>
{
    private PcreMatch? _noMatch;

    public new string Pattern => PatternString;

    public InternalRegex16Bit(string pattern, PcreRegexSettings settings)
        : base(pattern.AsSpan(), settings)
    {
        PatternString = pattern;
    }

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

            default(NativeStruct16Bit).match(&input, &result);

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

            default(NativeStruct16Bit).dfa_match(&input, &result);

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

            default(NativeStruct16Bit).substitute(&input, &result);

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
                default(NativeStruct16Bit).substitute_result_free(&result);
        }
    }

    public override string? GetString(void* ptr)
        => Native16Bit.GetString((char*)ptr);
}

internal sealed unsafe class InternalRegex8Bit : InternalRegex<byte, NativeStruct8Bit>
{
    public InternalRegex8Bit(ReadOnlySpan<byte> pattern, PcreRegexSettings settings)
        : base(pattern, settings)
    {
    }

    public override string? GetString(void* ptr)
        => Native8Bit.GetString((byte*)ptr);
}
