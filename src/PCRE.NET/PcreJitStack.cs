using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    /// <summary>
    /// JIT stack for advanced usage scenarios. Only use from a single thread at a time.
    /// </summary>
    [SuppressMessage("Naming", "CA1711")]
    public sealed class PcreJitStack : IDisposable
    {
        private IntPtr _stack;

        public PcreJitStack(uint startSize, uint maxSize)
        {
            _stack = Native.jit_stack_create(startSize, maxSize);
        }

        ~PcreJitStack()
        {
            FreeStack();
        }

        public void Dispose()
        {
            FreeStack();
            GC.SuppressFinalize(this);
        }

        private void FreeStack()
        {
            if (_stack == IntPtr.Zero)
                return;

            Native.jit_stack_free(_stack);
            _stack = IntPtr.Zero;
        }

        internal IntPtr GetStack()
        {
            if (_stack == IntPtr.Zero)
                throw new ObjectDisposedException("The JIT stack has been disposed");

            return _stack;
        }
    }
}
