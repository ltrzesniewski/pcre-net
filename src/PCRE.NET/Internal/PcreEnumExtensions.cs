namespace PCRE.Internal
{
    internal static class PcreEnumExtensions
    {
        public static uint ToPatternOptions(this PcreOptions options)
            => (uint)((long)options & 0xFFFFFFFF) | PcreConstants.UTF;

        public static uint ToPatternOptions(this PcreMatchOptions options)
            => (uint)((long)options & 0xFFFFFFFF);

        public static uint ToJitCompileOptions(this PcreOptions options)
        {
            var jitOptions = 0u;

            if ((options & PcreOptions.Compiled) != 0)
                jitOptions |= PcreConstants.JIT_COMPLETE;

            if ((options & PcreOptions.CompiledPartial) != 0)
                jitOptions |= PcreConstants.JIT_PARTIAL_SOFT | PcreConstants.JIT_PARTIAL_HARD;

            return jitOptions;
        }
    }
}
