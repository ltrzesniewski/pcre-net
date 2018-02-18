using PCRE.Wrapper;

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

        internal ConvertContext CreateContext()
        {
            return new ConvertContext
            {
                GlobEscape = EscapeCharacter,
                GlobSeparator = SeparatorCharacter
            };
        }

        internal ConvertOptions GetConvertOptions()
        {
            var options = ConvertOptions.Glob;

            if (NoWildcardSeparator)
                options |= ConvertOptions.GlobNoWildcardSeparator;

            if (NoStarStar)
                options |= ConvertOptions.GlobNoStarStar;

            return options;
        }
    }
}
