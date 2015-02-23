
#include "stdafx.h"
#include <memory>
#include "MatchData.h"
#include "InternalRegex.h"

namespace PCRE {
	namespace Wrapper {

		MatchData::MatchData(InternalRegex^ re, String^ subject)
			: _re(re),
			_subject(subject)
		{
			_matchData = pcre2_match_data_create_from_pattern(re->Code, nullptr);
			_oVector = pcre2_get_ovector_pointer(_matchData);
		}

		MatchData::MatchData(MatchData^ result, pcre2_callout_block *calloutBlock)
			: _re(result->_re),
			_subject(result->_subject),
			_resultCode(result->_resultCode),
			_markPtr(calloutBlock->mark)
		{
			_oVector = calloutBlock->offset_vector;
			_oVector[0] = calloutBlock->start_match;
			_oVector[1] = calloutBlock->current_position;
		}

		MatchData::~MatchData()
		{
			this->!MatchData();
		}

		MatchData::!MatchData()
		{
			if (_matchData)
			{
				pcre2_match_data_free(_matchData);
				_matchData = nullptr;
			}
		}

		int MatchData::GetStartOffset(int index)
		{
			return static_cast<int>(_oVector[2 * index]);
		}

		int MatchData::GetEndOffset(int index)
		{
			return static_cast<int>(_oVector[2 * index + 1]);
		}

		String^ MatchData::Mark::get()
		{
			if (_mark == nullptr)
			{
				PCRE2_SPTR markPtr;

				if (_markPtr)
					markPtr = _markPtr;
				else if (_matchData)
					markPtr = pcre2_get_mark(_matchData);
				else
					markPtr = nullptr;

				if (markPtr)
					_mark = gcnew String(reinterpret_cast<const wchar_t*>(markPtr));
			}

			return _mark;
		}

		void MatchData::EmptyOffsetVector()
		{
			auto oVectorCount = pcre2_get_ovector_count(_matchData);
			memset(_oVector, -1, sizeof(size_t) * oVectorCount);
		}
	}
}
