
#include "stdafx.h"
#include "MatchResult.h"
#include "InternalRegex.h"

namespace PCRE {
	namespace Wrapper {

		MatchResult::MatchResult(InternalRegex^ re, String^ subject)
			: _re(re),
			_subject(subject),
			_offsets(gcnew array<int>((re->CaptureCount + 1) * 3))
		{
		}

		MatchResult::MatchResult(MatchResult^ result)
			: _re(result->_re),
			_subject(result->_subject),
			_mark(result->_mark),
			_resultCode(result->_resultCode),
			_markPtr(result->_markPtr),
			_offsets(gcnew array<int>(result->_offsets->Length))
		{
			result->_offsets->CopyTo(_offsets, 0);
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

		void MatchResult::SetMark(const PCRE_UCHAR16 *mark)
		{
			_markPtr = mark;
			_mark = nullptr;
		}
	}
}
