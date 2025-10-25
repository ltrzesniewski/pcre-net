using PCRE.Internal;

namespace PCRE;

/// <summary>
/// Optimization directive to be used with <see cref="PcreRegexSettings"/>.
/// </summary>
public enum PcreOptimizationDirective : uint
{
    /// <summary>
    /// <c>PCRE2_OPTIMIZATION_NONE</c> - Disable all optional performance optimizations.
    /// </summary>
    None = PcreConstants.PCRE2_OPTIMIZATION_NONE,

    /// <summary>
    /// <c>PCRE2_OPTIMIZATION_FULL</c> - Enable all optional performance optimizations. This is the default value.
    /// </summary>
    Full = PcreConstants.PCRE2_OPTIMIZATION_FULL,

    /// <summary>
    /// <c>PCRE2_AUTO_POSSESS</c> - Enable "auto-possessification" of variable quantifiers such as <c>*</c> and <c>+</c>.
    /// </summary>
    /// <remarks>
    /// This optimization, for example, turns <c>a+b</c> into <c>a++b</c> in order to avoid backtracks into <c>a+</c> that can never be successful.
    /// However, if callouts are in use, auto-possessification means that some callouts are never taken.
    /// You can disable this optimization if you want the matching functions to do a full, unoptimized search and run all the callouts.
    /// </remarks>
    /// <seealso cref="AutoPossessOff"/>
    AutoPossess = PcreConstants.PCRE2_AUTO_POSSESS,

    /// <summary>
    /// <c>PCRE2_AUTO_POSSESS_OFF</c> - Disable "auto-possessification" of variable quantifiers such as * and +.
    /// </summary>
    /// <remarks>
    /// This optimization, for example, turns <c>a+b</c> into <c>a++b</c> in order to avoid backtracks into <c>a+</c> that can never be successful.
    /// However, if callouts are in use, auto-possessification means that some callouts are never taken.
    /// You can disable this optimization if you want the matching functions to do a full, unoptimized search and run all the callouts.
    /// </remarks>
    /// <seealso cref="AutoPossess"/>
    AutoPossessOff = PcreConstants.PCRE2_AUTO_POSSESS_OFF,

    /// <summary>
    /// <c>PCRE2_DOTSTAR_ANCHOR</c> - Enable an optimization that is applied when <c>.*</c> is the first significant item in a top-level branch of a pattern,
    /// and all the other branches also start with <c>.*</c> or with <c>\A</c> or <c>\G</c> or <c>^</c>.
    /// </summary>
    /// <remarks>
    /// Such a pattern is automatically anchored if <see cref="PcreOptions.DotAll"/> is set for all the <c>.*</c> items and <see cref="PcreOptions.MultiLine"/> is not set for any <c>^</c> items.
    /// Otherwise, the fact that any match must start either at the start of the subject or following a newline is remembered. Like other optimizations, this can cause callouts to be skipped.
    /// Dotstar anchor optimization is automatically disabled for <c>.*</c> if it is inside an atomic group or a capture group that is the subject of a backreference,
    /// or if the pattern contains <c>(*PRUNE)</c> or <c>(*SKIP)</c>.
    /// </remarks>
    /// <seealso cref="DotStarAnchorOff"/>
    DotStarAnchor = PcreConstants.PCRE2_DOTSTAR_ANCHOR,

    /// <summary>
    /// <c>PCRE2_DOTSTAR_ANCHOR_OFF</c> - Disable an optimization that is applied when <c>.*</c> is the first significant item in a top-level branch of a pattern,
    /// and all the other branches also start with <c>.*</c> or with <c>\A</c> or <c>\G</c> or <c>^</c>.
    /// </summary>
    /// <remarks>
    /// Such a pattern is automatically anchored if <see cref="PcreOptions.DotAll"/> is set for all the <c>.*</c> items and <see cref="PcreOptions.MultiLine"/> is not set for any <c>^</c> items.
    /// Otherwise, the fact that any match must start either at the start of the subject or following a newline is remembered. Like other optimizations, this can cause callouts to be skipped.
    /// Dotstar anchor optimization is automatically disabled for <c>.*</c> if it is inside an atomic group or a capture group that is the subject of a backreference,
    /// or if the pattern contains <c>(*PRUNE)</c> or <c>(*SKIP)</c>.
    /// </remarks>
    /// <seealso cref="DotStarAnchor"/>
    DotStarAnchorOff = PcreConstants.PCRE2_DOTSTAR_ANCHOR_OFF,

    /// <summary>
    /// <c>PCRE2_START_OPTIMIZE</c> - Enable optimizations which cause matching functions to scan the subject string for specific code unit values before attempting a match.
    /// </summary>
    /// <remarks>
    /// For example, if it is known that an unanchored match must start with a specific value, the matching code searches the subject for that value, and fails immediately if it cannot find it,
    /// without actually running the main matching function. This means that a special item such as <c>(*COMMIT)</c> at the start of a pattern is not considered until after a suitable starting
    /// point for the match has been found. Also, when callouts or <c>(*MARK)</c> items are in use, these "start-up" optimizations can cause them to be skipped if the pattern is never actually used.
    /// The start-up optimizations are in effect a pre-scan of the subject that takes place before the pattern is run.
    /// </remarks>
    /// <seealso cref="StartOptimizeOff"/>
    StartOptimize = PcreConstants.PCRE2_START_OPTIMIZE,

    /// <summary>
    /// <c>PCRE2_START_OPTIMIZE_OFF</c> - Disable optimizations which cause matching functions to scan the subject string for specific code unit values before attempting a match.
    /// </summary>
    /// <remarks>
    /// For example, if it is known that an unanchored match must start with a specific value, the matching code searches the subject for that value, and fails immediately if it cannot find it,
    /// without actually running the main matching function. This means that a special item such as <c>(*COMMIT)</c> at the start of a pattern is not considered until after a suitable starting
    /// point for the match has been found. Also, when callouts or <c>(*MARK)</c> items are in use, these "start-up" optimizations can cause them to be skipped if the pattern is never actually used.
    /// The start-up optimizations are in effect a pre-scan of the subject that takes place before the pattern is run.
    /// </remarks>
    /// <seealso cref="StartOptimize"/>
    StartOptimizeOff = PcreConstants.PCRE2_START_OPTIMIZE_OFF,
}
