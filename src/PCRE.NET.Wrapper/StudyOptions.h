
#pragma once

#include "Stdafx.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		[Flags]
		public enum class StudyOptions
		{
			None                                = 0,
			JitCompile                          = PCRE_STUDY_JIT_COMPILE,
			JitPartialSoftCompile               = PCRE_STUDY_JIT_PARTIAL_SOFT_COMPILE,
			JitPartialHardCompile               = PCRE_STUDY_JIT_PARTIAL_HARD_COMPILE,
			ExtraNeeded                         = PCRE_STUDY_EXTRA_NEEDED
		};

	}
}
