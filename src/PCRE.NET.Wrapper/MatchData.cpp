
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
			_oVectorCount = pcre2_get_ovector_count(_matchData);
		}

		MatchData::MatchData(InternalRegex^ re, String^ subject, uint32_t oVectorSize)
			: _re(re),
			_subject(subject)
		{
			_matchData = pcre2_match_data_create(oVectorSize, nullptr);
			_oVector = pcre2_get_ovector_pointer(_matchData);
			_oVectorCount = pcre2_get_ovector_count(_matchData);
		}

		MatchData::MatchData(MatchData^ result, pcre2_callout_block *calloutBlock)
			: _re(result->_re),
			_subject(result->_subject),
			_markPtr(calloutBlock->mark)
		{
			_oVectorCount = calloutBlock->capture_top;
			_matchData = pcre2_match_data_create(static_cast<uint32_t>(_oVectorCount), nullptr);
			_oVector = pcre2_get_ovector_pointer(_matchData);

			memcpy(_oVector, calloutBlock->offset_vector, _oVectorCount * sizeof(PCRE2_SIZE) * 2);
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

		int MatchData::GetStartOffset(unsigned int index)
		{
			return index < _oVectorCount ? static_cast<int>(_oVector[2 * index]) : -1;
		}

		int MatchData::GetEndOffset(unsigned int index)
		{
			return index < _oVectorCount ? static_cast<int>(_oVector[2 * index + 1]) : -1;
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
	}
}
