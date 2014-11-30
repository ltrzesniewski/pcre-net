

#include "stdafx.h"
#include "CalloutData.h"
#include "MatchResult.h"

namespace PCRE {
	namespace Wrapper {

		CalloutData::CalloutData(MatchResult^ match, pcre16_callout_block *block)
			: _number(block->callout_number),
			_startOffset(block->start_match),
			_currentOffset(block->current_position),
			_maxCapture(block->capture_top),
			_lastCapture(block->capture_last),
			_patternPosition(block->pattern_position),
			_nextItemLength(block->next_item_length)
		{
			_match = gcnew MatchResult(match);
			_match->SetMark(block->mark);
			_match->_offsets[0] = _startOffset;
			_match->_offsets[1] = _currentOffset;
		}

	}
}
