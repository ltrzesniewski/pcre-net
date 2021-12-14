using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InlineIL;
using static InlineIL.IL.Emit;

namespace PCRE.Internal
{
    internal static unsafe class CalloutInterop
    {
#if NETCOREAPP
        private static readonly delegate* unmanaged[Cdecl]<Native.pcre2_callout_block*, void*, int> _calloutHandlerFnPtr = &CalloutHandler;

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static int CalloutHandler(Native.pcre2_callout_block* callout, void* data)
            => ToInteropInfo(data).Call(callout);
#else
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CalloutHandlerFunc(Native.pcre2_callout_block* callout, void* data);

        private static readonly CalloutHandlerFunc _calloutHandlerDelegate = CalloutHandler; // GC root
        private static readonly void* _calloutHandlerFnPtr = Marshal.GetFunctionPointerForDelegate(_calloutHandlerDelegate).ToPointer();

        private static int CalloutHandler(Native.pcre2_callout_block* callout, void* data)
            => ToInteropInfo(data).Call(callout);
#endif

        public static void Prepare(string subject, InternalRegex regex, ref Native.match_input input, out CalloutInteropInfo interopInfo, Func<PcreCallout, PcreCalloutResult>? callout)
        {
            if (callout != null)
            {
                interopInfo = new CalloutInteropInfo(subject, regex, callout);

                input.callout = _calloutHandlerFnPtr;
                input.callout_data = interopInfo.ToPointer();
            }
            else
            {
                SkipInitInteropInfo(out interopInfo);
                input.callout = null;
            }
        }

        public static void Prepare(ReadOnlySpan<char> subject,
                                   InternalRegex regex,
                                   ref Native.match_input input,
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
                SkipInitInteropInfo(out interopInfo);
                input.callout = null;
            }
        }

        public static void Prepare(ReadOnlySpan<char> subject,
                                   InternalRegex regex,
                                   ref Native.buffer_match_input input,
                                   out CalloutInteropInfo interopInfo,
                                   PcreRefCalloutFunc? callout,
                                   nuint[]? calloutOutputVector)
        {
            // TODO: Deduplicate this

            if (callout != null)
            {
                interopInfo = new CalloutInteropInfo(subject, regex, callout, calloutOutputVector);

                input.callout = _calloutHandlerFnPtr;
                input.callout_data = interopInfo.ToPointer();
            }
            else
            {
                SkipInitInteropInfo(out interopInfo);
                input.callout = null;
            }
        }

        public static void Prepare(string subject,
                                   InternalRegex regex,
                                   ref Native.dfa_match_input input,
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
                SkipInitInteropInfo(out interopInfo);
                input.callout = null;
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
        private static ref CalloutInteropInfo ToInteropInfo(void* data)
        {
#if NETCOREAPP
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
        [SuppressMessage("Usage", "CA1801")]
        private static void SkipInitInteropInfo(out CalloutInteropInfo value)
        {
            Ret();
            throw IL.Unreachable();
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
    }
}
