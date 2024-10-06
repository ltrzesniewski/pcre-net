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
    private static readonly delegate* unmanaged[Cdecl]<Native.pcre2_callout_block*, void*, int> _calloutHandlerFnPtr = &CalloutHandler;
    private static readonly delegate* unmanaged[Cdecl]<Native.pcre2_callout_block*, void*, int> _substituteMatchCalloutHandlerFnPtr = &SubstituteMatchCalloutHandler;
    private static readonly delegate* unmanaged[Cdecl]<Native.pcre2_substitute_callout_block*, void*, int> _substituteCalloutHandlerFnPtr = &SubstituteCalloutHandler;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int CalloutHandler(Native.pcre2_callout_block* callout, void* data)
        => ToInteropInfo(data).Call(callout);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int SubstituteMatchCalloutHandler(Native.pcre2_callout_block* callout, void* data)
        => ToSubstituteInteropInfo(data).CallMatchCallout(callout);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int SubstituteCalloutHandler(Native.pcre2_substitute_callout_block* callout, void* data)
        => ToSubstituteInteropInfo(data).CallSubstituteCallout(callout);
#else
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int CalloutHandlerFunc(Native.pcre2_callout_block* callout, void* data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SubstituteMatchCalloutHandlerFunc(Native.pcre2_callout_block* callout, void* data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SubstituteCalloutHandlerFunc(Native.pcre2_substitute_callout_block* callout, void* data);

    private static readonly CalloutHandlerFunc _calloutHandlerDelegate = CalloutHandler; // GC root
    private static readonly void* _calloutHandlerFnPtr = Marshal.GetFunctionPointerForDelegate(_calloutHandlerDelegate).ToPointer();

    private static readonly SubstituteMatchCalloutHandlerFunc _substituteMatchCalloutHandlerDelegate = SubstituteMatchCalloutHandler; // GC root
    private static readonly void* _substituteMatchCalloutHandlerFnPtr = Marshal.GetFunctionPointerForDelegate(_substituteMatchCalloutHandlerDelegate).ToPointer();

    private static readonly SubstituteCalloutHandlerFunc _substituteCalloutHandlerDelegate = SubstituteCalloutHandler; // GC root
    private static readonly void* _substituteCalloutHandlerFnPtr = Marshal.GetFunctionPointerForDelegate(_substituteCalloutHandlerDelegate).ToPointer();

    private static int CalloutHandler(Native.pcre2_callout_block* callout, void* data)
        => ToInteropInfo(data).Call(callout);

    private static int SubstituteMatchCalloutHandler(Native.pcre2_callout_block* callout, void* data)
        => ToSubstituteInteropInfo(data).CallMatchCallout(callout);

    private static int SubstituteCalloutHandler(Native.pcre2_substitute_callout_block* callout, void* data)
        => ToSubstituteInteropInfo(data).CallSubstituteCallout(callout);
#endif

    public static void Prepare(string subject,
                               InternalRegex regex,
                               scoped ref Native.match_input input,
                               out CalloutInteropInfo interopInfo,
                               Func<PcreCallout, PcreCalloutResult>? callout)
    {
        if (callout != null)
        {
            interopInfo = new CalloutInteropInfo(subject, regex, callout);

            input.callout = _calloutHandlerFnPtr;
            input.callout_data = interopInfo.ToPointer();
        }
        else
        {
            interopInfo = default;
            input.callout = null;
        }
    }

    public static void Prepare(ReadOnlySpan<char> subject,
                               InternalRegex regex,
                               scoped ref Native.match_input input,
                               out CalloutInteropInfo interopInfo,
                               PcreRefCalloutFunc? callout,
                               nuint[]? calloutOutputVector)
    {
        if (callout != null)
        {
            interopInfo = new CalloutInteropInfo(subject, regex, callout, calloutOutputVector);

            input.callout = _calloutHandlerFnPtr;
            input.callout_data = interopInfo.ToPointer();
        }
        else
        {
            interopInfo = default;
            input.callout = null;
        }
    }

    public static void Prepare(ReadOnlySpan<char> subject,
                               PcreMatchBuffer buffer,
                               scoped ref Native.buffer_match_input input,
                               out CalloutInteropInfo interopInfo,
                               PcreRefCalloutFunc? callout)
    {
        if (callout != null)
        {
            interopInfo = new CalloutInteropInfo(subject, buffer.Regex, callout, buffer.CalloutOutputVector);

            input.callout = _calloutHandlerFnPtr;
            input.callout_data = interopInfo.ToPointer();
        }
        else
        {
            interopInfo = default;
            input.callout = null;
        }
    }

    public static void Prepare(string subject,
                               InternalRegex regex,
                               scoped ref Native.dfa_match_input input,
                               out CalloutInteropInfo interopInfo,
                               Func<PcreCallout, PcreCalloutResult>? callout)
    {
        if (callout != null)
        {
            interopInfo = new CalloutInteropInfo(subject, regex, callout);

            input.callout = _calloutHandlerFnPtr;
            input.callout_data = interopInfo.ToPointer();
        }
        else
        {
            interopInfo = default;
            input.callout = null;
        }
    }

    public static void PrepareSubstitute(InternalRegex regex,
                                         ReadOnlySpan<char> subject,
                                         scoped ref Native.substitute_input input,
                                         out SubstituteCalloutInteropInfo interopInfo,
                                         PcreRefCalloutFunc? matchCallout,
                                         PcreSubstituteCalloutFunc? substituteCallout)
    {
        input.match_callout = matchCallout is not null ? _substituteMatchCalloutHandlerFnPtr : null;
        input.substitute_callout = substituteCallout is not null ? _substituteCalloutHandlerFnPtr : null;

        if (matchCallout is not null || substituteCallout is not null)
        {
            interopInfo = new SubstituteCalloutInteropInfo(regex, subject, matchCallout, substituteCallout);
            input.callout_data = interopInfo.ToPointer();
        }
        else
        {
            interopInfo = default;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("ReSharper", "EntityNameCapturedOnly.Local")]
    private static void* ToPointer(this ref CalloutInteropInfo value)
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
    private static ref CalloutInteropInfo ToInteropInfo(void* data)
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

    public ref struct CalloutInteropInfo
    {
        private readonly string? _subject;
        private readonly ReadOnlySpan<char> _subjectSpan;
        private readonly InternalRegex _regex;
        private readonly Delegate _callout;
        private readonly nuint[]? _outputVector;

        public Exception? Exception { get; private set; }

        public CalloutInteropInfo(string subject, InternalRegex regex, Func<PcreCallout, PcreCalloutResult> callout)
        {
            _subject = subject;
            _subjectSpan = default;
            _regex = regex;
            _callout = callout;
            _outputVector = null;

            Exception = null;
        }

        public CalloutInteropInfo(ReadOnlySpan<char> subject, InternalRegex regex, PcreRefCalloutFunc callout, nuint[]? outputVector)
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
                else
                {
                    var outputVector = _outputVector ?? (
                        callout->capture_top <= InternalRegex.MaxStackAllocCaptureCount
                            ? stackalloc nuint[(int)callout->capture_top * 2]
                            : Span<nuint>.Empty
                    );

                    var func = (PcreRefCalloutFunc)_callout;
                    return (int)func(new PcreRefCallout(_subjectSpan, _regex, callout, outputVector));
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
                return PcreConstants.ERROR_CALLOUT;
            }
        }
    }

    public ref struct SubstituteCalloutInteropInfo
    {
        private readonly InternalRegex _regex;
        private readonly ReadOnlySpan<char> _subject;
        private readonly PcreRefCalloutFunc? _matchCallout;
        private readonly PcreSubstituteCalloutFunc? _substituteCallout;

        public Exception? Exception { get; private set; }

        public SubstituteCalloutInteropInfo(InternalRegex regex, ReadOnlySpan<char> subject, PcreRefCalloutFunc? matchCallout, PcreSubstituteCalloutFunc? substituteCallout)
        {
            _regex = regex;
            _subject = subject;
            _matchCallout = matchCallout;
            _substituteCallout = substituteCallout;

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
    }
}
