using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InlineIL;
using static InlineIL.IL.Emit;

namespace PCRE.Internal;

internal static unsafe class CalloutInterop
{
#if NET
    private static readonly delegate* unmanaged[Cdecl]<Native.pcre2_callout_block*, void*, int> _calloutHandlerFnPtr8Bit = &CalloutHandler8Bit;
    private static readonly delegate* unmanaged[Cdecl]<Native.pcre2_callout_block*, void*, int> _calloutHandlerFnPtr16Bit = &CalloutHandler16Bit;

    private static readonly delegate* unmanaged[Cdecl]<Native.pcre2_callout_block*, void*, int> _substituteMatchCalloutHandlerFnPtr = &SubstituteMatchCalloutHandler;
    private static readonly delegate* unmanaged[Cdecl]<Native.pcre2_substitute_callout_block*, void*, int> _substituteCalloutHandlerFnPtr = &SubstituteCalloutHandler;
    private static readonly delegate* unmanaged[Cdecl]<char*, nuint, char*, nuint, int, void*, nuint> _substituteCaseCalloutHandlerFnPtr = &SubstituteCaseCalloutHandler;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int CalloutHandler8Bit(Native.pcre2_callout_block* callout, void* data)
        => ToInteropInfo<byte>(data).Call(callout);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int CalloutHandler16Bit(Native.pcre2_callout_block* callout, void* data)
        => ToInteropInfo<char>(data).Call(callout);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int SubstituteMatchCalloutHandler(Native.pcre2_callout_block* callout, void* data)
        => ToSubstituteInteropInfo(data).CallMatchCallout(callout);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int SubstituteCalloutHandler(Native.pcre2_substitute_callout_block* callout, void* data)
        => ToSubstituteInteropInfo(data).CallSubstituteCallout(callout);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static nuint SubstituteCaseCalloutHandler(char* input, nuint inputLength, char* output, nuint outputLength, int targetCase, void* data)
        => ToSubstituteInteropInfo(data).CallSubstituteCaseCallout(input, inputLength, output, outputLength, targetCase);
#else
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int CalloutHandlerFunc(Native.pcre2_callout_block* callout, void* data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SubstituteMatchCalloutHandlerFunc(Native.pcre2_callout_block* callout, void* data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SubstituteCalloutHandlerFunc(Native.pcre2_substitute_callout_block* callout, void* data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nuint SubstituteCaseCalloutHandlerFunc(char* input, nuint inputLength, char* output, nuint outputLength, int targetCase, void* data);

    private static readonly CalloutHandlerFunc _calloutHandlerDelegate8Bit = CalloutHandler8Bit; // GC root
    private static readonly CalloutHandlerFunc _calloutHandlerDelegate16Bit = CalloutHandler16Bit; // GC root
    private static readonly void* _calloutHandlerFnPtr8Bit = Marshal.GetFunctionPointerForDelegate(_calloutHandlerDelegate8Bit).ToPointer();
    private static readonly void* _calloutHandlerFnPtr16Bit = Marshal.GetFunctionPointerForDelegate(_calloutHandlerDelegate16Bit).ToPointer();

    private static readonly SubstituteMatchCalloutHandlerFunc _substituteMatchCalloutHandlerDelegate = SubstituteMatchCalloutHandler; // GC root
    private static readonly void* _substituteMatchCalloutHandlerFnPtr = Marshal.GetFunctionPointerForDelegate(_substituteMatchCalloutHandlerDelegate).ToPointer();

    private static readonly SubstituteCalloutHandlerFunc _substituteCalloutHandlerDelegate = SubstituteCalloutHandler; // GC root
    private static readonly void* _substituteCalloutHandlerFnPtr = Marshal.GetFunctionPointerForDelegate(_substituteCalloutHandlerDelegate).ToPointer();

    private static readonly SubstituteCaseCalloutHandlerFunc _substituteCaseCalloutHandlerDelegate = SubstituteCaseCalloutHandler; // GC root
    private static readonly void* _substituteCaseCalloutHandlerFnPtr = Marshal.GetFunctionPointerForDelegate(_substituteCaseCalloutHandlerDelegate).ToPointer();

    private static int CalloutHandler8Bit(Native.pcre2_callout_block* callout, void* data)
        => ToInteropInfo<byte>(data).Call(callout);

    private static int CalloutHandler16Bit(Native.pcre2_callout_block* callout, void* data)
        => ToInteropInfo<char>(data).Call(callout);

    private static int SubstituteMatchCalloutHandler(Native.pcre2_callout_block* callout, void* data)
        => ToSubstituteInteropInfo(data).CallMatchCallout(callout);

    private static int SubstituteCalloutHandler(Native.pcre2_substitute_callout_block* callout, void* data)
        => ToSubstituteInteropInfo(data).CallSubstituteCallout(callout);

    private static nuint SubstituteCaseCalloutHandler(char* input, nuint inputLength, char* output, nuint outputLength, int targetCase, void* data)
        => ToSubstituteInteropInfo(data).CallSubstituteCaseCallout(input, inputLength, output, outputLength, targetCase);
#endif

    public static void Prepare(string subject,
                               InternalRegex16Bit regex,
                               scoped ref Native.match_input input,
                               out CalloutInteropInfo<char> interopInfo,
                               Func<PcreCallout, PcreCalloutResult>? callout)
    {
        if (callout != null)
        {
            interopInfo = new CalloutInteropInfo<char>(subject, regex, callout);

            input.callout = _calloutHandlerFnPtr16Bit;
            input.callout_data = interopInfo.ToPointer();
        }
        else
        {
            interopInfo = default;
            input.callout = null;
        }
    }

    public static void Prepare<TChar>(ReadOnlySpan<TChar> subject,
                                      InternalRegex<TChar> regex,
                                      scoped ref Native.match_input input,
                                      out CalloutInteropInfo<TChar> interopInfo,
                                      Delegate? callout,
                                      nuint[]? calloutOutputVector)
        where TChar : unmanaged
    {
        if (callout != null)
        {
            interopInfo = new CalloutInteropInfo<TChar>(subject, regex, callout, calloutOutputVector);

            input.callout = sizeof(TChar) switch
            {
                sizeof(byte) => _calloutHandlerFnPtr8Bit,
                sizeof(char) => _calloutHandlerFnPtr16Bit,
                _            => null
            };

            input.callout_data = interopInfo.ToPointer();
        }
        else
        {
            interopInfo = default;
            input.callout = null;
        }
    }

    public static void Prepare<TChar>(ReadOnlySpan<TChar> subject,
                                      IPcreMatchBuffer buffer,
                                      scoped ref Native.buffer_match_input input,
                                      out CalloutInteropInfo<TChar> interopInfo,
                                      Delegate? callout)
        where TChar : unmanaged
    {
        if (callout != null)
        {
            interopInfo = new CalloutInteropInfo<TChar>(subject, (InternalRegex<TChar>)buffer.Regex, callout, buffer.CalloutOutputVector);

            input.callout = sizeof(TChar) switch
            {
                sizeof(byte) => _calloutHandlerFnPtr8Bit,
                sizeof(char) => _calloutHandlerFnPtr16Bit,
                _            => null
            };

            input.callout_data = interopInfo.ToPointer();
        }
        else
        {
            interopInfo = default;
            input.callout = null;
        }
    }

    public static void Prepare(string subject,
                               InternalRegex16Bit regex,
                               scoped ref Native.dfa_match_input input,
                               out CalloutInteropInfo<char> interopInfo,
                               Func<PcreCallout, PcreCalloutResult>? callout)
    {
        if (callout != null)
        {
            interopInfo = new CalloutInteropInfo<char>(subject, regex, callout);

            input.callout = _calloutHandlerFnPtr16Bit;
            input.callout_data = interopInfo.ToPointer();
        }
        else
        {
            interopInfo = default;
            input.callout = null;
        }
    }

    public static void PrepareSubstitute(InternalRegex16Bit regex,
                                         ReadOnlySpan<char> subject,
                                         scoped ref Native.substitute_input input,
                                         out SubstituteCalloutInteropInfo interopInfo,
                                         PcreRefCalloutFunc? matchCallout,
                                         PcreSubstituteCalloutFunc? substituteCallout,
                                         PcreSubstituteCaseCalloutFunc? substituteCaseCallout)
    {
        input.match_callout = matchCallout is not null ? _substituteMatchCalloutHandlerFnPtr : null;
        input.substitute_callout = substituteCallout is not null ? _substituteCalloutHandlerFnPtr : null;
        input.substitute_case_callout = substituteCaseCallout is not null ? _substituteCaseCalloutHandlerFnPtr : null;

        if (matchCallout is not null || substituteCallout is not null || substituteCaseCallout is not null)
        {
            interopInfo = new SubstituteCalloutInteropInfo(regex, subject, matchCallout, substituteCallout, substituteCaseCallout);
            input.callout_data = interopInfo.ToPointer();
        }
        else
        {
            interopInfo = default;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "EntityNameCapturedOnly.Local")]
    private static void* ToPointer<TChar>(this ref CalloutInteropInfo<TChar> value)
        where TChar : unmanaged
    {
        Ldarg(nameof(value));
        Conv_U();
        return IL.ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "EntityNameCapturedOnly.Local")]
    private static void* ToPointer(this ref SubstituteCalloutInteropInfo value)
    {
        Ldarg(nameof(value));
        Conv_U();
        return IL.ReturnPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ref CalloutInteropInfo<TChar> ToInteropInfo<TChar>(void* data)
        where TChar : unmanaged
    {
#if NET
        IL.Push(data);
        Ret();
        throw IL.Unreachable();
#else
        // Roundtrip via a local to avoid type mismatch on return that the JIT inliner chokes on.
        IL.DeclareLocals(
            false,
            new LocalVar("local", typeof(int).MakeByRefType())
        );

        IL.Push(data);
        Stloc("local");
        Ldloc("local");
        Ret();
        throw IL.Unreachable();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ref SubstituteCalloutInteropInfo ToSubstituteInteropInfo(void* data)
    {
#if NET
        IL.Push(data);
        Ret();
        throw IL.Unreachable();
#else
        // Roundtrip via a local to avoid type mismatch on return that the JIT inliner chokes on.
        IL.DeclareLocals(
            false,
            new LocalVar("local", typeof(int).MakeByRefType())
        );

        IL.Push(data);
        Stloc("local");
        Ldloc("local");
        Ret();
        throw IL.Unreachable();
#endif
    }

    public ref struct CalloutInteropInfo<TChar>
        where TChar : unmanaged
    {
        private readonly string? _subject;
        private readonly ReadOnlySpan<TChar> _subjectSpan;
        private readonly InternalRegex<TChar> _regex;
        private readonly Delegate _callout;
        private readonly nuint[]? _outputVector;

        public Exception? Exception { get; private set; }

        public CalloutInteropInfo(string subject, InternalRegex<TChar> regex, Func<PcreCallout, PcreCalloutResult> callout)
        {
            _subject = subject;
            _subjectSpan = default;
            _regex = regex;
            _callout = callout;
            _outputVector = null;

            Exception = null;
        }

        public CalloutInteropInfo(ReadOnlySpan<TChar> subject, InternalRegex<TChar> regex, Delegate callout, nuint[]? outputVector)
        {
            _subject = null;
            _subjectSpan = subject;
            _regex = regex;
            _callout = callout;
            _outputVector = outputVector;

            Exception = null;
        }

        [SuppressMessage("Microsoft.Design", "CA1031")]
        public int Call(Native.pcre2_callout_block* callout)
        {
            try
            {
                if (_subject != null)
                {
                    var func = (Func<PcreCallout, PcreCalloutResult>)_callout;
                    return (int)func(new PcreCallout(_subject, _regex, callout));
                }

                var outputVector = _outputVector ?? (
                    callout->capture_top <= InternalRegex.MaxStackAllocCaptureCount
                        ? stackalloc nuint[(int)callout->capture_top * 2]
                        : Span<nuint>.Empty
                );

                if (typeof(TChar) == typeof(byte))
                {
                    fixed (TChar* ptr = _subjectSpan)
                    {
                        switch (_callout)
                        {
                            case PcreRefCalloutFuncUtf8 func:
                            {
                                return (int)func(
                                    new PcreRefCalloutUtf8(
                                        new ReadOnlySpan<byte>(ptr, _subjectSpan.Length),
                                        (InternalRegex8Bit)(object)_regex,
                                        callout,
                                        outputVector
                                    )
                                );
                            }

                            case PcreRefCalloutFunc8Bit func:
                            {
                                return (int)func(
                                    new PcreRefCallout8Bit(
                                        new ReadOnlySpan<byte>(ptr, _subjectSpan.Length),
                                        (InternalRegex8Bit)(object)_regex,
                                        callout,
                                        outputVector
                                    )
                                );
                            }

                            default:
                                throw new InvalidOperationException("Unexpected callout funtion type");
                        }
                    }
                }

                if (typeof(TChar) == typeof(char))
                {
                    var func = (PcreRefCalloutFunc)_callout;
                    fixed (TChar* ptr = _subjectSpan)
                    {
                        return (int)func(
                            new PcreRefCallout(
                                new ReadOnlySpan<char>(ptr, _subjectSpan.Length),
                                (InternalRegex16Bit)(object)_regex,
                                callout,
                                outputVector
                            )
                        );
                    }
                }

                throw new InvalidOperationException("Unreachable code path");
            }
            catch (Exception ex)
            {
                Exception = ex;
                return PcreConstants.PCRE2_ERROR_CALLOUT;
            }
        }
    }

    public ref struct SubstituteCalloutInteropInfo
    {
        private readonly InternalRegex16Bit _regex;
        private readonly ReadOnlySpan<char> _subject;
        private readonly PcreRefCalloutFunc? _matchCallout;
        private readonly PcreSubstituteCalloutFunc? _substituteCallout;
        private readonly PcreSubstituteCaseCalloutFunc? _substituteCaseCallout;

        public Exception? Exception { get; private set; }

        public SubstituteCalloutInteropInfo(InternalRegex16Bit regex,
                                            ReadOnlySpan<char> subject,
                                            PcreRefCalloutFunc? matchCallout,
                                            PcreSubstituteCalloutFunc? substituteCallout,
                                            PcreSubstituteCaseCalloutFunc? substituteCaseCallout)
        {
            _regex = regex;
            _subject = subject;
            _matchCallout = matchCallout;
            _substituteCallout = substituteCallout;
            _substituteCaseCallout = substituteCaseCallout;

            Exception = null;
        }

        [SuppressMessage("Microsoft.Design", "CA1031")]
        public int CallMatchCallout(Native.pcre2_callout_block* callout)
        {
            try
            {
                var outputVector = callout->capture_top <= InternalRegex.MaxStackAllocCaptureCount
                    ? stackalloc nuint[(int)callout->capture_top * 2]
                    : Span<nuint>.Empty;

                return (int)(_matchCallout?.Invoke(new PcreRefCallout(_subject, _regex, callout, outputVector)) ?? PcreCalloutResult.Pass);
            }
            catch (Exception ex)
            {
                Exception = ex;
                return (int)PcreCalloutResult.Abort;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031")]
        public int CallSubstituteCallout(Native.pcre2_substitute_callout_block* callout)
        {
            try
            {
                return (int)(_substituteCallout?.Invoke(new PcreSubstituteCallout(_regex, _subject, callout)) ?? PcreSubstituteCalloutResult.Pass);
            }
            catch (Exception ex)
            {
                Exception = ex;
                return (int)PcreSubstituteCalloutResult.Abort;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031")]
        public nuint CallSubstituteCaseCallout(char* input, nuint inputLength, char* output, nuint outputLength, int targetCase)
        {
            try
            {
                if (_substituteCaseCallout is null)
                    return ~(nuint)0;

                var result = _substituteCaseCallout(new ReadOnlySpan<char>(input, (int)inputLength), (PcreSubstituteCase)targetCase);

                if (ReferenceEquals(result, null))
                    return ~(nuint)0;

                if ((nuint)result.Length <= outputLength)
                    result.AsSpan().CopyTo(new Span<char>(output, result.Length));

                return (nuint)result.Length;
            }
            catch (Exception ex)
            {
                Exception = ex;
                return ~(nuint)0;
            }
        }
    }
}
