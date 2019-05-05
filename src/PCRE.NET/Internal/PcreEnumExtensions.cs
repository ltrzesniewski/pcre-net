namespace PCRE.Internal
{
    internal static class PcreEnumExtensions
    {
        public static uint ToPatternOptions(this PcreOptions options)
            => (uint)((long)options & 0xFFFFFFFF) | PcreConstants.UTF;

        public static uint ToPatternOptions(this PcreMatchOptions options)
            => (uint)((long)options & 0xFFFFFFFF);

        public static PcreJitCompileOptions ToJitCompileOptions(this PcreOptions options)
        {
            var jitOptions = PcreJitCompileOptions.None;

            if ((options & PcreOptions.Compiled) != 0)
                jitOptions |= PcreJitCompileOptions.Complete;

            if ((options & PcreOptions.CompiledPartial) != 0)
                jitOptions |= PcreJitCompileOptions.PartialSoft | PcreJitCompileOptions.PartialHard;

            return jitOptions;
        }
    }
}
