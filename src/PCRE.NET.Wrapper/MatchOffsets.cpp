
#include "stdafx.h"
#include "MatchOffsets.h"

namespace PCRE {
	namespace Wrapper {

		MatchOffsets::MatchOffsets(int captureCount)
		{
			_offsets = gcnew array<int>((captureCount + 1) * 3);
		}

		int MatchOffsets::GetStartOffset(int index)
		{
			return _offsets[2 * index];
		}

		int MatchOffsets::GetEndOffset(int index)
		{
			return _offsets[2 * index + 1];
		}

	}
}
