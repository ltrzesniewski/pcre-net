
#pragma once

#include "Stdafx.h"

namespace PCRE {
	namespace Wrapper {

		[System::Flags]
		public enum struct CalloutFlags : unsigned int
		{
			None = 0,
			StartMatch                 = PCRE2_CALLOUT_STARTMATCH,
			Backtrack                  = PCRE2_CALLOUT_BACKTRACK
		};

	}
}
