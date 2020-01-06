using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InlineIL;
using static InlineIL.IL.Emit;

namespace PCRE.Internal
{
    internal static unsafe class CalloutInterop
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int CalloutHandlerFunc(Native.pcre2_callout_block* callout, void* data);

        private static readonly CalloutHandlerFunc _calloutHandlerDelegate = CalloutHandler; // GC root
        private static readonly void* _calloutHandlerFnPtr = Marshal.GetFunctionPointerForDelegate(_calloutHandlerDelegate).ToPointer();

        private static int CalloutHandler(Native.pcre2_callout_block* callout, void* data)
            => ToInteropInfo(data).Call(callout);

        public static void Prepare(string subject, InternalRegex regex, ref Native.match_input input, out CalloutInteropInfo interopInfo, Func<PcreCallout, PcreCalloutResult> callout)
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

        public static void Prepare(ReadOnlySpan<char> subject, InternalRegex regex, ref Native.match_input input, out CalloutInteropInfo interopInfo, PcreRefCalloutFunc callout)
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

        public static void Prepare(string subject, InternalRegex regex, ref Native.dfa_match_input input, out CalloutInteropInfo interopInfo, Func<PcreCallout, PcreCalloutResult> callout)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void* ToPointer(this ref CalloutInteropInfo value)
        {
            Ldarg(nameof(value));
            Conv_U();
            return IL.ReturnPointer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref CalloutInteropInfo ToInteropInfo(void* data)
        {
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
        }

        public ref struct CalloutInteropInfo
        {
            private readonly string _subject;
            private readonly ReadOnlySpan<char> _subjectSpan;
            private readonly InternalRegex _regex;
            private readonly Delegate _callout;
            public Exception Exception { get; private set; }

            public CalloutInteropInfo(string subject, InternalRegex regex, Func<PcreCallout, PcreCalloutResult> callout)
            {
                _subject = subject;
                _subjectSpan = default;
                _regex = regex;
                _callout = callout;
                Exception = null;
            }

            public CalloutInteropInfo(ReadOnlySpan<char> subject, InternalRegex regex, PcreRefCalloutFunc callout)
            {
                _subject = null;
                _subjectSpan = subject;
                _regex = regex;
                _callout = callout;
                Exception = null;
            }

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
                        var func = (PcreRefCalloutFunc)_callout;
                        return (int)func(new PcreRefCallout(_subjectSpan, _regex, callout));
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
