
#pragma once

#include "Stdafx.h"

namespace PCRE {
	namespace Wrapper {

		[System::Flags]
		public enum struct ConvertOptions : unsigned int
		{
			None = 0,
			Utf                     = PCRE2_CONVERT_UTF,
			NoUtfCheck              = PCRE2_CONVERT_NO_UTF_CHECK,
			PosixBasic              = PCRE2_CONVERT_POSIX_BASIC,
			PosixExtended           = PCRE2_CONVERT_POSIX_EXTENDED,
			Glob                    = PCRE2_CONVERT_GLOB,
			GlobNoWildcardSeparator = PCRE2_CONVERT_GLOB_NO_WILD_SEPARATOR,
			GlobNoStarStar          = PCRE2_CONVERT_GLOB_NO_STARSTAR
		};

	}
}
