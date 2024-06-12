using System;
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

    internal bool ReadOnlySettings { get; }

    /// <summary>
    /// Creates a new <see cref="PcreRegexSettings"/> object.
    /// </summary>
    public PcreRegexSettings()
    {
    }

    internal PcreRegexSettings(PcreOptions options)
    {
        _options = options;
    }

    private PcreRegexSettings(PcreRegexSettings settings, bool readOnly)
    {
        _options = settings._options;
        _newLine = settings._newLine;
        _backslashR = settings._backslashR;
        _parensLimit = settings._parensLimit;
        _maxPatternLength = settings._maxPatternLength;
        _maxPatternCompiledLength = settings._maxPatternCompiledLength;
        _maxVarLookbehind = settings._maxVarLookbehind;
        _extraCompileOptions = settings._extraCompileOptions;
        _jitCompileOptions = settings._jitCompileOptions;

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
               && JitCompileOptions == other.JitCompileOptions;
    }

    internal PcreRegexSettings AsReadOnly()
    {
        if (ReadOnlySettings)
            return this;

        return new PcreRegexSettings(this, true);
    }

    private void EnsureIsMutable()
    {
        if (ReadOnlySettings)
            throw new InvalidOperationException("Settings of a compiled pattern cannot be changed.");
    }

    internal void FillCompileInput(ref Native.compile_input input)
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
    }
}
