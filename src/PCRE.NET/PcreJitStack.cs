using System;

namespace PCRE
{
    /// <summary>
    /// JIT stack for advanced usage scenarios. Only use from a single thread at a time.
    /// </summary>
    public sealed class PcreJitStack : IDisposable
    {
        private object _stack; // See remark about JIT in PcreRegex

        public PcreJitStack(uint startSize, uint maxSize)
        {
            _stack = new JitStack(startSize, maxSize);
        }

        internal JitStack GetStack()
        {
            if (_stack != null)
                return (JitStack)_stack;

            throw new ObjectDisposedException("The JIT stack has been disposed");
        }

        public void Dispose()
        {
            if (_stack is IDisposable disposable)
            {
                disposable.Dispose();
                _stack = null;
            }
        }
    }
}
