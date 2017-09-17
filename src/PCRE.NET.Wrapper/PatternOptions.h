
#pragma once

#include "Stdafx.h"

namespace PCRE {
	namespace Wrapper {

		[System::Flags]
		public enum struct PatternOptions : unsigned int
		{
			None                       = 0,
			Anchored                   = PCRE2_ANCHORED,
			EndAnchored                = PCRE2_ENDANCHORED,
			NoUtfCheck                 = PCRE2_NO_UTF_CHECK,
			AllowEmptyClass            = PCRE2_ALLOW_EMPTY_CLASS,
			AltBsUX                    = PCRE2_ALT_BSUX,
			AltCircumflex              = PCRE2_ALT_CIRCUMFLEX,
			AltVerbNames               = PCRE2_ALT_VERBNAMES,
			AutoCallout                = PCRE2_AUTO_CALLOUT,
			Caseless                   = PCRE2_CASELESS,
			DollarEndOnly              = PCRE2_DOLLAR_ENDONLY,
			DotAll                     = PCRE2_DOTALL,
			DupNames                   = PCRE2_DUPNAMES,
			Extended                   = PCRE2_EXTENDED,
			ExtendedMore               = PCRE2_EXTENDED_MORE,
			FirstLine                  = PCRE2_FIRSTLINE,
			MatchUnsetBackref          = PCRE2_MATCH_UNSET_BACKREF,
			MultiLine                  = PCRE2_MULTILINE,
			NeverBackslashC            = PCRE2_NEVER_BACKSLASH_C,
			NeverUcp                   = PCRE2_NEVER_UCP,
			NeverUtf                   = PCRE2_NEVER_UTF,
			NoAutoCapture              = PCRE2_NO_AUTO_CAPTURE,
			NoAutoPossess              = PCRE2_NO_AUTO_POSSESS,
			NoDotStarAnchor            = PCRE2_NO_DOTSTAR_ANCHOR,
			NoStartOptimize            = PCRE2_NO_START_OPTIMIZE,
			Ucp                        = PCRE2_UCP,
			Ungreedy                   = PCRE2_UNGREEDY,
			UseOffsetLimit             = PCRE2_USE_OFFSET_LIMIT,
			Utf                        = PCRE2_UTF,
			NotBol                     = PCRE2_NOTBOL,
			NotEol                     = PCRE2_NOTEOL,
			NotEmpty                   = PCRE2_NOTEMPTY,
			NotEmptyAtStart            = PCRE2_NOTEMPTY_ATSTART,
			Literal                    = PCRE2_LITERAL,
			PartialSoft                = PCRE2_PARTIAL_SOFT,
			PartialHard                = PCRE2_PARTIAL_HARD,
			DfaRestart                 = PCRE2_DFA_RESTART,
			DfaShortest                = PCRE2_DFA_SHORTEST,
			NoJit                      = PCRE2_NO_JIT
		};

	}
}
