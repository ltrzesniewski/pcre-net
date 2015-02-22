

#include "stdafx.h"
#include "CalloutData.h"
#include "MatchData.h"

namespace PCRE {
	namespace Wrapper {

		CalloutData::CalloutData(MatchData^ match, pcre2_callout_block *block)
			: _number(static_cast<int>(block->callout_number)),
			_startOffset(static_cast<int>(block->start_match)),
			_currentOffset(static_cast<int>(block->current_position)),
			_maxCapture(static_cast<int>(block->capture_top)),
			_lastCapture(static_cast<int>(block->capture_last)),
			_patternPosition(static_cast<int>(block->pattern_position)),
			_nextItemLength(static_cast<int>(block->next_item_length))
		{
			_match = gcnew MatchData(match);
			_match->SetMark(block->mark);
			_match->_oVector[0] = _startOffset;
			_match->_oVector[1] = _currentOffset;
		}

	}
}
