using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// The result of a match.
/// </summary>
[DebuggerTypeProxy(typeof(DebugProxy))]
public unsafe ref partial struct PcreRefMatchUtf8
{
    private readonly IRegexHolder8Bit? _owner; // Needs to be kept alive as long as this match is used
    internal Span<nuint> OutputVector; // Can be empty when there is no match
    private int _resultCode;
    private byte* _markPtr;

    internal ReadOnlySpan<byte> Subject;

    /// <summary>
    /// A function which returns an output value out of a match.
    /// </summary>
    /// <typeparam name="T">The output value type.</typeparam>
    public delegate T Func<out T>(PcreRefMatchUtf8 match);

    /// <inheritdoc cref="PcreMatch.ToString"/>
    public readonly override string ToString()
    {
        GC.KeepAlive(_owner); // Good enough place to keep the object alive, I suppose.
        return PcreRegexUtf8.GetString(Value);
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal class DebugProxy
    {
        public bool Success { get; }
        public string? Value { get; }

        public PcreRefGroupUtf8.DebugProxy[] Groups { get; }

        public DebugProxy(PcreRefMatchUtf8 match)
        {
            Success = match.Success;
            Value = Success ? PcreRegexUtf8.GetString(match.Value) : null;

            Groups = new PcreRefGroupUtf8.DebugProxy[match.CaptureCount + 1];
            for (var i = 0; i <= match.CaptureCount; ++i)
                Groups[i] = new PcreRefGroupUtf8.DebugProxy(match[i]);
        }

        public override string ToString()
            => Value ?? "<no match>";
    }
}
