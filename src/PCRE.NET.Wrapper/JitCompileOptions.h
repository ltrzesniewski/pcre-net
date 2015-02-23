
#pragma once

#include "Stdafx.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		[Flags]
		public enum struct JitCompileOptions
		{
			None                                = 0,
			Complete                            = PCRE2_JIT_COMPLETE,
			PartialSoft                         = PCRE2_JIT_PARTIAL_SOFT,
			PartialHard                         = PCRE2_JIT_PARTIAL_HARD
		};

	}
}
