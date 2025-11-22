using System;
using System.Diagnostics.CodeAnalysis;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// A callout function.
/// </summary>
public delegate PcreCalloutResult PcreRefCalloutFuncUtf8(PcreRefCalloutUtf8 callout);

/// <inheritdoc cref="PcreCallout"/>
public unsafe ref partial struct PcreRefCalloutUtf8
{
    private readonly ReadOnlySpan<byte> _subject;
    private readonly InternalRegex8Bit _regex;
    private readonly Native.pcre2_callout_block* _callout;

    internal Span<nuint> OutputVector;
    private bool _oVectorInitialized;
}
