
#pragma once

#include "Stdafx.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public enum struct MatchResultCode
		{
			None                       = 0,
			Success                    = 1,
			NoMatch                    = PCRE_ERROR_NOMATCH,
			Null                       = PCRE_ERROR_NULL,
			BadOption                  = PCRE_ERROR_BADOPTION,
			BadMagic                   = PCRE_ERROR_BADMAGIC,
			UnknownOpCode              = PCRE_ERROR_UNKNOWN_OPCODE,
			NoMemory                   = PCRE_ERROR_NOMEMORY,
			NoSubstring                = PCRE_ERROR_NOSUBSTRING,
			MatchLimit                 = PCRE_ERROR_MATCHLIMIT,
			Callout                    = PCRE_ERROR_CALLOUT,
			BadUtf                     = PCRE_ERROR_BADUTF16,
			BadUtfOffset               = PCRE_ERROR_BADUTF16_OFFSET,
			Partial                    = PCRE_ERROR_PARTIAL,
			BadPartial                 = PCRE_ERROR_BADPARTIAL,
			Internal                   = PCRE_ERROR_INTERNAL,
			BadCount                   = PCRE_ERROR_BADCOUNT,
			DfaUnsupportedItem         = PCRE_ERROR_DFA_UITEM,
			DfaUnsupportedCondition    = PCRE_ERROR_DFA_UCOND,
			DfaUnsupportedMatchLimit   = PCRE_ERROR_DFA_UMLIMIT,
			DfaWorkspaceSize           = PCRE_ERROR_DFA_WSSIZE,
			DfaRecurse                 = PCRE_ERROR_DFA_RECURSE,
			RecursionLimit             = PCRE_ERROR_RECURSIONLIMIT,
			NullWorkspaceLimit         = PCRE_ERROR_NULLWSLIMIT,
			BadNewLine                 = PCRE_ERROR_BADNEWLINE,
			BadOffset                  = PCRE_ERROR_BADOFFSET,
			ShortUtf                   = PCRE_ERROR_SHORTUTF16,
			RecurseLoop                = PCRE_ERROR_RECURSELOOP,
			JitStackLimit              = PCRE_ERROR_JIT_STACKLIMIT,
			BadMode                    = PCRE_ERROR_BADMODE,
			BadEndianness              = PCRE_ERROR_BADENDIANNESS,
			DfaBadRestart              = PCRE_ERROR_DFA_BADRESTART,
			JitBadOption               = PCRE_ERROR_JIT_BADOPTION,
			BadLength                  = PCRE_ERROR_BADLENGTH,
			Unset                      = PCRE_ERROR_UNSET
		};

	}
}
