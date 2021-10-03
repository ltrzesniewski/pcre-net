using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE
{
    /// <summary>
    /// JIT stack for advanced usage scenarios.
    /// </summary>
    /// <remarks>
    /// Not thread-safe and not reentrant.
    /// </remarks>
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
                ThrowDisposed();

            return _stack;
        }

        private static void ThrowDisposed()
            => throw new ObjectDisposedException("The JIT stack has been disposed.");
    }
}
