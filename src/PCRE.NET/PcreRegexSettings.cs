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

        public PcreOptions Options
        {
            get { return _options; }
            set
            {
                EnsureIsMutable();
                _options = value;
            }
        }

        public PcreNewLine NewLine
        {
            get { return _newLine ?? PcreBuildInfo.NewLine; }
            set
            {
                EnsureIsMutable();
                _newLine = value;
            }
        }

        public PcreBackslashR BackslashR
        {
            get { return _backslashR ?? PcreBuildInfo.BackslashR; }
            set
            {
                EnsureIsMutable();
                _backslashR = value;
            }
        }

        public uint ParensLimit
        {
            get { return _parensLimit ?? PcreBuildInfo.ParensLimit; }
            set
            {
                EnsureIsMutable();
                _parensLimit = value;
            }
        }

        internal bool ReadOnlySettings
        {
            get { return _readOnly; }
        }

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
            _readOnly = readOnly;
        }

        internal bool CompareValues(PcreRegexSettings other)
        {
            return Options == other.Options
                   && NewLine == other.NewLine
                   && BackslashR == other.BackslashR
                   && ParensLimit == other.ParensLimit;
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
                context.ParensNestLimit = _parensLimit.Value;

            return context;
        }
    }
}
