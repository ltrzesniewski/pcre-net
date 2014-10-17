
#pragma once

#include "Stdafx.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public enum class PcreConfigKey
		{
			Utf8                       = PCRE_CONFIG_UTF8,
			NewLine                    = PCRE_CONFIG_NEWLINE,
			LinkSize                   = PCRE_CONFIG_LINK_SIZE,
			PosixMallocThreshold       = PCRE_CONFIG_POSIX_MALLOC_THRESHOLD,
			MatchLimit                 = PCRE_CONFIG_MATCH_LIMIT,
			StackRecurse               = PCRE_CONFIG_STACKRECURSE,
			UnicodeProperties          = PCRE_CONFIG_UNICODE_PROPERTIES,
			MatchLimitRecursion        = PCRE_CONFIG_MATCH_LIMIT_RECURSION,
			Bsr                        = PCRE_CONFIG_BSR,
			Jit                        = PCRE_CONFIG_JIT,
			Utf16                      = PCRE_CONFIG_UTF16,
			JitTarget                  = PCRE_CONFIG_JITTARGET,
			Utf32                      = PCRE_CONFIG_UTF32,
			ParensLimit                = PCRE_CONFIG_PARENS_LIMIT
		};

	}
}
