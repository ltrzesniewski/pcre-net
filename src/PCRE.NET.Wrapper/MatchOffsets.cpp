
#include "stdafx.h"
#include "MatchOffsets.h"

namespace PCRE {
	namespace Wrapper {

		MatchOffsets::MatchOffsets(int captureCount)
		{
			_offsets = gcnew array<int>((captureCount + 1) * 3);
		}

	}
}
