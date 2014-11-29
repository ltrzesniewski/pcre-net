
#include "stdafx.h"
#include "MatchResult.h"

namespace PCRE {
	namespace Wrapper {

		MatchResult::MatchResult(int captureCount)
		{
			_offsets = gcnew array<int>((captureCount + 1) * 3);
		}

		int MatchResult::GetStartOffset(int index)
		{
			return _offsets[2 * index];
		}

		int MatchResult::GetEndOffset(int index)
		{
			return _offsets[2 * index + 1];
		}

	}
}
