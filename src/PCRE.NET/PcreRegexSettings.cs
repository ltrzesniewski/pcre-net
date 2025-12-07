using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using PCRE.Internal;

namespace PCRE;

/// <summary>
/// Advanced regex settings.
/// </summary>
public sealed class PcreRegexSettings
{
    private PcreOptions _options;
    private PcreNewLine? _newLine;
    private PcreBackslashR? _backslashR;
    private uint? _parensLimit;
    private uint? _maxPatternLength;
    private uint? _maxPatternCompiledLength;
    private uint? _maxVarLookbehind;
    private PcreExtraCompileOptions _extraCompileOptions;
    private PcreJitCompileOptions _jitCompileOptions;
    private IList<PcreOptimizationDirective>? _optimizationDirectives;

    /// <summary>
    /// The options to apply to the regex.
    /// </summary>
    public PcreOptions Options
    {
        get => _options;
        set
        {
            EnsureIsMutable();
            _options = value;
        }
    }

    /// <summary>
    /// The character sequence that is recognized as meaning "newline".
    /// </summary>
    public PcreNewLine NewLine
    {
        get => _newLine ?? PcreBuildInfo.NewLine;
        set
        {
            EnsureIsMutable();
            _newLine = value;
        }
    }

    /// <summary>
    /// The character sequences the <c>\R</c> escape sequence matches.
    /// </summary>
    public PcreBackslashR BackslashR
    {
        get => _backslashR ?? PcreBuildInfo.BackslashR;
        set
        {
            EnsureIsMutable();
            _backslashR = value;
        }
    }

    /// <summary>
    /// The maximum depth of nesting of parentheses (of any kind).
    /// </summary>
    public uint ParensLimit
    {
        get => _parensLimit ?? PcreBuildInfo.ParensLimit;
        set
        {
            EnsureIsMutable();
            _parensLimit = value;
        }
    }

    /// <summary>
    /// The maximum length, in code units, for the pattern.
    /// </summary>
    /// <remarks>
    /// This facility is provided so that applications that accept patterns from external sources can limit their size.
    /// The default is the largest number that a <see cref="IntPtr"/> variable can hold, which is effectively unlimited.
    /// </remarks>
    public uint? MaxPatternLength
    {
        get => _maxPatternLength;
        set
        {
            EnsureIsMutable();
            _maxPatternLength = value;
        }
    }

    /// <summary>
    /// The maximum size, in bytes, for the memory needed to hold the compiled version of a pattern.
    /// </summary>
    /// <remarks>
    /// This facility is provided so that applications that accept patterns from external sources can limit the amount of memory they use.
    /// The default is the largest number that a <see cref="IntPtr"/> variable can hold, which is effectively unlimited.
    /// </remarks>
    public uint? MaxPatternCompiledLength
    {
        get => _maxPatternCompiledLength;
        set
        {
            EnsureIsMutable();
            _maxPatternCompiledLength = value;
        }
    }

    /// <summary>
    /// The maximum length for the number of characters matched by a variable-length lookbehind assertion.
    /// </summary>
    /// <remarks>
    /// The default is set when PCRE2 is built, with the ultimate default being 255, the same as Perl.
    /// Lookbehind assertions without a bounding length are not supported.
    /// </remarks>
    public uint MaxVarLookbehind
    {
        get => _maxVarLookbehind ?? 255;
        set
        {
            EnsureIsMutable();
            _maxVarLookbehind = value;
        }
    }

    /// <summary>
    /// Additional compile options.
    /// </summary>
    public PcreExtraCompileOptions ExtraCompileOptions
    {
        get => _extraCompileOptions;
        set
        {
            EnsureIsMutable();
            _extraCompileOptions = value;
        }
    }

    /// <summary>
    /// Additional options for the JIT compiler.
    /// </summary>
    public PcreJitCompileOptions JitCompileOptions
    {
        get => _jitCompileOptions | Options.ToJitCompileOptions();
        set
        {
            EnsureIsMutable();
            _jitCompileOptions = value;
        }
    }

    /// <summary>
    /// Additional optimization directives.
    /// </summary>
    /// <remarks>
    /// By default, all available optimizations are enabled. However, in rare cases, one might wish to disable specific optimizations.
    /// For example, if it is known that some optimizations cannot benefit a certain regex, it might be desirable to disable them to speed up compilation.
    /// </remarks>
    public IList<PcreOptimizationDirective> OptimizationDirectives => _optimizationDirectives ??= new List<PcreOptimizationDirective>();

    internal bool ReadOnlySettings { get; }

    /// <summary>
    /// Creates a new <see cref="PcreRegexSettings"/> object.
    /// </summary>
    public PcreRegexSettings()
    {
    }

    internal PcreRegexSettings(PcreOptions options)
        : this()
    {
        _options = options;
    }

    private PcreRegexSettings(PcreRegexSettings settings, bool readOnly, PcreOptions additionalOptions)
    {
        _options = settings._options | additionalOptions;
        _newLine = settings._newLine;
        _backslashR = settings._backslashR;
        _parensLimit = settings._parensLimit;
        _maxPatternLength = settings._maxPatternLength;
        _maxPatternCompiledLength = settings._maxPatternCompiledLength;
        _maxVarLookbehind = settings._maxVarLookbehind;
        _extraCompileOptions = settings._extraCompileOptions;
        _jitCompileOptions = settings._jitCompileOptions;

        _optimizationDirectives = readOnly
            ? settings._optimizationDirectives?.Count is not (null or 0)
                ? new ReadOnlyCollection<PcreOptimizationDirective>(settings._optimizationDirectives.ToArray())
                : Array.Empty<PcreOptimizationDirective>()
            : settings._optimizationDirectives?.ToList();

        ReadOnlySettings = readOnly;
    }

    internal bool CompareValues(PcreRegexSettings other)
    {
        return Options == other.Options
               && NewLine == other.NewLine
               && BackslashR == other.BackslashR
               && ParensLimit == other.ParensLimit
               && MaxPatternLength == other.MaxPatternLength
               && MaxPatternCompiledLength == other.MaxPatternCompiledLength
               && MaxVarLookbehind == other.MaxVarLookbehind
               && ExtraCompileOptions == other.ExtraCompileOptions
               && JitCompileOptions == other.JitCompileOptions
               && (_optimizationDirectives ?? Enumerable.Empty<PcreOptimizationDirective>()).SequenceEqual(other._optimizationDirectives ?? Enumerable.Empty<PcreOptimizationDirective>());
    }

    internal PcreRegexSettings ToReadOnlySnapshot(PcreOptions additionalOptions)
    {
        if (ReadOnlySettings && (Options & additionalOptions) == additionalOptions)
            return this;

        return new PcreRegexSettings(this, true, additionalOptions);
    }

    private void EnsureIsMutable()
    {
        if (ReadOnlySettings)
            throw new InvalidOperationException("Settings of a compiled pattern cannot be changed.");
    }

    internal unsafe IDisposable? FillCompileInput(ref Native.compile_input input)
    {
        input.flags = Options.ToPatternOptions();
        input.flags_jit = (uint)JitCompileOptions;
        input.new_line = (uint)_newLine.GetValueOrDefault();
        input.bsr = (uint)_backslashR.GetValueOrDefault();
        input.parens_nest_limit = _parensLimit.GetValueOrDefault();
        input.max_pattern_length = _maxPatternLength.GetValueOrDefault();
        input.max_pattern_compiled_length = _maxPatternCompiledLength.GetValueOrDefault();
        input.max_var_lookbehind = MaxVarLookbehind;
        input.compile_extra_options = (uint)_extraCompileOptions;
        input.optimization_directives_count = (uint)(_optimizationDirectives?.Count ?? 0);

        if (_optimizationDirectives?.Count is null or 0)
        {
            input.optimization_directives = null;
            return null;
        }

        var directives = _optimizationDirectives.Cast<uint>().ToArray();
        var handle = GCHandle.Alloc(directives, GCHandleType.Pinned);
        input.optimization_directives = (uint*)handle.AddrOfPinnedObject();
        return new GCHandleDisposable(handle);
    }

    private class GCHandleDisposable(GCHandle handle) : IDisposable
    {
        private GCHandle _handle = handle;

        public void Dispose()
        {
            if (_handle.IsAllocated)
                _handle.Free();

            _handle = default;
        }
    }
}
