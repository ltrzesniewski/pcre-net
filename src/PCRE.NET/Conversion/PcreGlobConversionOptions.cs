using PCRE.Internal;

namespace PCRE.Conversion
{
    public sealed class PcreGlobConversionOptions
    {
        public bool NoWildcardSeparator { get; set; }
        public bool NoStarStar { get; set; }
        public char EscapeCharacter { get; set; } = '`';
        public char SeparatorCharacter { get; set; } = '\\';

        public static PcreGlobConversionOptions DefaultWindows()
            => new PcreGlobConversionOptions();

        public static PcreGlobConversionOptions DefaultUnix()
            => new PcreGlobConversionOptions
            {
                EscapeCharacter = '\\',
                SeparatorCharacter = '/'
            };

        internal void FillConvertInput(ref Native.convert_input input)
        {
            input.options = GetConvertOptions();
            input.glob_escape = EscapeCharacter;
            input.glob_separator = SeparatorCharacter;
        }

        private uint GetConvertOptions()
        {
            var options = PcreConstants.CONVERT_GLOB;

            if (NoWildcardSeparator)
                options |= PcreConstants.CONVERT_GLOB_NO_WILD_SEPARATOR;

            if (NoStarStar)
                options |= PcreConstants.CONVERT_GLOB_NO_STARSTAR;

            return options;
        }
    }
}
