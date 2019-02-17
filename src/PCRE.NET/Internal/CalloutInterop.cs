using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InlineIL;
using static InlineIL.IL.Emit;

namespace PCRE.Internal
{
    internal static unsafe class CalloutInterop
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int CalloutHandlerFunc(ref Native.pcre2_callout_block callout, void* data);

        private static readonly CalloutHandlerFunc _calloutHandlerDelegate = CalloutHandler;
        private static readonly void* _calloutHandlerFnPtr = Marshal.GetFunctionPointerForDelegate(_calloutHandlerDelegate).ToPointer();

        private static int CalloutHandler(ref Native.pcre2_callout_block callout, void* data)
        {
            ref var interopInfo = ref ToInteropInfo(data);

            try
            {
                return (int)interopInfo.Callout(new PcreCallout(interopInfo.Subject, interopInfo.Regex, ref callout));
            }
            catch (Exception ex)
            {
                interopInfo.Exception = ex;
                return PcreConstants.ERROR_CALLOUT;
            }
        }

        public static void Prepare(string subject, InternalRegex regex, ref Native.match_input input, out CalloutInteropInfo interopInfo, Func<PcreCallout, PcreCalloutResult> callout)
        {
            if (callout != null)
            {
                interopInfo = new CalloutInteropInfo
                {
                    Subject = subject,
                    Regex = regex,
                    Callout = callout
                };

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
        public static void* ToPointer(this ref CalloutInteropInfo value)
        {
            Ldarg(nameof(value));
            Conv_U();
            Ret();
            throw IL.Unreachable();
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
            public string Subject;
            public InternalRegex Regex;
            public Func<PcreCallout, PcreCalloutResult> Callout;
            public Exception Exception;
        }
    }
}
