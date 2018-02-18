using System;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreRegexSettings
    {
        private readonly bool _readOnly;

        private PcreOptions _options;
        private PcreNewLine? _newLine;
        private PcreBackslashR? _backslashR;
        private uint? _parensLimit;
        private uint? _maxPatternLength;
        private PcreExtraCompileOptions _extraCompileOptions;

        public PcreOptions Options
        {
            get => _options;
            set
            {
                EnsureIsMutable();
                _options = value;
            }
        }

        public PcreNewLine NewLine
        {
            get => _newLine ?? PcreBuildInfo.NewLine;
            set
            {
                EnsureIsMutable();
                _newLine = value;
            }
        }

        public PcreBackslashR BackslashR
        {
            get => _backslashR ?? PcreBuildInfo.BackslashR;
            set
            {
                EnsureIsMutable();
                _backslashR = value;
            }
        }

        public uint ParensLimit
        {
            get => _parensLimit ?? PcreBuildInfo.ParensLimit;
            set
            {
                EnsureIsMutable();
                _parensLimit = value;
            }
        }

        public uint? MaxPatternLength
        {
            get => _maxPatternLength;
            set
            {
                EnsureIsMutable();
                _maxPatternLength = value;
            }
        }

        public PcreExtraCompileOptions ExtraCompileOptions
        {
            get => _extraCompileOptions;
            set
            {
                EnsureIsMutable();
                _extraCompileOptions = value;
            }
        }

        internal bool ReadOnlySettings => _readOnly;

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
            _extraCompileOptions = settings._extraCompileOptions;
            _readOnly = readOnly;
        }

        internal bool CompareValues(PcreRegexSettings other)
        {
            return Options == other.Options
                   && NewLine == other.NewLine
                   && BackslashR == other.BackslashR
                   && ParensLimit == other.ParensLimit
                   && MaxPatternLength == other.MaxPatternLength
                   && ExtraCompileOptions == other.ExtraCompileOptions;
        }

        internal PcreRegexSettings AsReadOnly()
        {
            if (_readOnly)
                return this;

            return new PcreRegexSettings(this, true);
        }

        private void EnsureIsMutable()
        {
            if (_readOnly)
                throw new InvalidOperationException("Settings of a compiled pattern cannot be changed");
        }

        internal CompileContext CreateCompileContext(string pattern)
        {
            var context = new CompileContext(pattern)
            {
                Options = Options.ToPatternOptions(),
                JitCompileOptions = Options.ToJitCompileOptions()
            };

            if (_newLine != null && _newLine != PcreNewLine.Default)
                context.NewLine = (NewLine)_newLine;

            if (_backslashR != null && _backslashR != PcreBackslashR.Default)
                context.BackslashR = (BackslashR)_backslashR;

            if (_parensLimit != null)
                context.ParensNestLimit = _parensLimit.GetValueOrDefault();

            if (_maxPatternLength != null)
                context.MaxPatternLength = _maxPatternLength.GetValueOrDefault();

            if (_extraCompileOptions != PcreExtraCompileOptions.None)
                context.ExtraCompileOptions = (ExtraCompileOptions)_extraCompileOptions;

            return context;
        }
    }
}
