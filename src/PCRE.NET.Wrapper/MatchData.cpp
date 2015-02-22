
#include "stdafx.h"
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

		MatchData::MatchData(MatchData^ result)
			: _re(result->_re),
			_subject(result->_subject),
			_mark(result->_mark),
			_resultCode(result->_resultCode)
			//_markPtr(result->_markPtr),
		{
			//result->_offsets->CopyTo(_offsets, 0);
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

		int MatchData::GetPartialStartOffset()
		{
			return static_cast<int>(_oVector[2]);
		}

		int MatchData::GetPartialEndOffset()
		{
			return static_cast<int>(_oVector[1]);
		}

		int MatchData::GetPartialScanStartOffset()
		{
			return static_cast<int>(_oVector[0]);
		}

		String^ MatchData::Mark::get()
		{
			if (_mark == nullptr)
			{
				if (_markPtr == nullptr)
					return nullptr;

				_mark = gcnew String(reinterpret_cast<const wchar_t*>(_markPtr));
			}

			return _mark;
		}

		void MatchData::SetMark(const PCRE2_SPTR mark)
		{
			_markPtr = mark;
			_mark = nullptr;
		}
	}
}
