using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// JIT stack for advanced usage scenarios.
/// </summary>
/// <remarks>
/// <para>
/// Not thread-safe and not reentrant.
/// </para>
/// <para>
/// When the compiled JIT code runs, it needs a block of memory to use as a stack. By default, it uses 32KiB on the machine stack.
/// However, some large or complicated patterns need more than this. The error <see cref="PcreErrorCode.JitStackLimit"/> is given when there is not enough stack.
/// </para>
/// <para>
/// You may safely use the same JIT stack for more than one pattern (either by assigning directly or by callback), as long as the patterns are matched sequentially in the same thread.
/// Currently, the only way to set up non-sequential matches in one thread is to use callouts: if a callout function starts another match,
/// that match must use a different JIT stack to the one used for currently suspended match(es).
/// </para>
/// </remarks>
[SuppressMessage("Naming", "CA1711")]
public sealed unsafe class PcreJitStack : IDisposable
{
    private void* _stack;

    /// <summary>
    /// Creates a JIT stack.
    /// </summary>
    /// <param name="startSize">The initial stack size.</param>
    /// <param name="maxSize">The maximum stack size.</param>
    public PcreJitStack(uint startSize, uint maxSize)
    {
        // The JIT stack is independent of the character width.
        _stack = default(Native16Bit).jit_stack_create(startSize, maxSize);
    }

    /// <summary>
    /// Releases the JIT stack.
    /// </summary>
    ~PcreJitStack()
        => FreeStack();

    /// <summary>
    /// Disposes the JIT stack.
    /// </summary>
    public void Dispose()
    {
        FreeStack();
        GC.SuppressFinalize(this);
    }

    private void FreeStack()
    {
        if (_stack == null)
            return;

        default(Native16Bit).jit_stack_free(_stack);
        _stack = null;
    }

    internal void* GetStack()
    {
        if (_stack == null)
            ThrowDisposed();

        return _stack;
    }

    private static void ThrowDisposed()
        => throw new ObjectDisposedException("The JIT stack has been disposed.");
}
