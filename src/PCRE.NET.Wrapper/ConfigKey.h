
#pragma once

#include "Stdafx.h"

namespace PCRE {
	namespace Wrapper {

		public enum struct ConfigKey
		{
			Bsr                        = PCRE2_CONFIG_BSR,
			Jit                        = PCRE2_CONFIG_JIT,
			JitTarget                  = PCRE2_CONFIG_JITTARGET,
			LinkSize                   = PCRE2_CONFIG_LINKSIZE,
			MatchLimit                 = PCRE2_CONFIG_MATCHLIMIT,
			NewLine                    = PCRE2_CONFIG_NEWLINE,
			ParensLimit                = PCRE2_CONFIG_PARENSLIMIT,
			DepthLimit                 = PCRE2_CONFIG_DEPTHLIMIT,
			Unicode                    = PCRE2_CONFIG_UNICODE,
			UnicodeVersion             = PCRE2_CONFIG_UNICODE_VERSION,
			Version                    = PCRE2_CONFIG_VERSION,
			HeapLimit                  = PCRE2_CONFIG_HEAPLIMIT
		};

	}
}
