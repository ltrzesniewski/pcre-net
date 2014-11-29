
#include "stdafx.h"
#include "MatchResult.h"
#include "InternalRegex.h"

namespace PCRE {
	namespace Wrapper {

		MatchResult::MatchResult(InternalRegex^ const re)
		{
			_re = re;
			_offsets = gcnew array<int>((re->CaptureCount + 1) * 3);
		}

		int MatchResult::GetStartOffset(int index)
		{
			return _offsets[2 * index];
		}

		int MatchResult::GetEndOffset(int index)
		{
			return _offsets[2 * index + 1];
		}

		String^ MatchResult::Mark::get()
		{
			if (_mark == nullptr)
			{
				if (_markPtr == nullptr)
					return nullptr;

				_mark = gcnew String(_markPtr);
			}

			return _mark;
		}

		void MatchResult::SetMark(PCRE_UCHAR16 *mark)
		{
			_markPtr = mark;
			_mark = nullptr;
		}

	}
}
