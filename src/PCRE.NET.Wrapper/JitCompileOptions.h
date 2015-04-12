
#pragma once

#include "Stdafx.h"

namespace PCRE {
	namespace Wrapper {

		[System::Flags]
		public enum struct JitCompileOptions
		{
			None                                = 0,
			Complete                            = PCRE2_JIT_COMPLETE,
			PartialSoft                         = PCRE2_JIT_PARTIAL_SOFT,
			PartialHard                         = PCRE2_JIT_PARTIAL_HARD
		};

	}
}
