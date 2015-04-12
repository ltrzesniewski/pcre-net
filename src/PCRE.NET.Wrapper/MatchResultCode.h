
#pragma once

#include "Stdafx.h"

namespace PCRE {
	namespace Wrapper {

		public enum struct MatchResultCode
		{
			None                       = 0,
			Success                    = 1,
			NoMatch                    = PCRE2_ERROR_NOMATCH,
			Partial                    = PCRE2_ERROR_PARTIAL,
			BadData                    = PCRE2_ERROR_BADDATA,
			MixedTables                = PCRE2_ERROR_MIXEDTABLES,
			BadMagic                   = PCRE2_ERROR_BADMAGIC,
			BadMode                    = PCRE2_ERROR_BADMODE,
			BadOffset                  = PCRE2_ERROR_BADOFFSET,
			BadOption                  = PCRE2_ERROR_BADOPTION,
			BadReplacement             = PCRE2_ERROR_BADREPLACEMENT,
			BadUtfOffset               = PCRE2_ERROR_BADUTFOFFSET,
			Callout                    = PCRE2_ERROR_CALLOUT,
			DfaBadRestart              = PCRE2_ERROR_DFA_BADRESTART,
			DfaRecurse                 = PCRE2_ERROR_DFA_RECURSE,
			DfaUnsupportedCondition    = PCRE2_ERROR_DFA_UCOND,
			DfaUnsupportedFunction     = PCRE2_ERROR_DFA_UFUNC,
			DfaUnsupportedItem         = PCRE2_ERROR_DFA_UITEM,
			DfaWorkspaceSize           = PCRE2_ERROR_DFA_WSSIZE,
			Internal                   = PCRE2_ERROR_INTERNAL,
			JitBadOption               = PCRE2_ERROR_JIT_BADOPTION,
			JitStackLimit              = PCRE2_ERROR_JIT_STACKLIMIT,
			MatchLimit                 = PCRE2_ERROR_MATCHLIMIT,
			NoMemory                   = PCRE2_ERROR_NOMEMORY,
			NoSubstring                = PCRE2_ERROR_NOSUBSTRING,
			NoUniqueSubstring          = PCRE2_ERROR_NOUNIQUESUBSTRING,
			Null                       = PCRE2_ERROR_NULL,
			RecurseLoop                = PCRE2_ERROR_RECURSELOOP,
			RecursionLimit             = PCRE2_ERROR_RECURSIONLIMIT,
			Unavailable                = PCRE2_ERROR_UNAVAILABLE,
			Unset                      = PCRE2_ERROR_UNSET,
		};

	}
}
