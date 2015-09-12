
#include "stdafx.h"
#include "CalloutData.h"
#include "MatchData.h"
#include "InternalRegex.h"

using namespace PCRE::Wrapper;

CalloutData::CalloutData(MatchData^ match, pcre2_callout_block *block)
	: _number(static_cast<int>(block->callout_number)),
	_startOffset(static_cast<int>(block->start_match)),
	_currentOffset(static_cast<int>(block->current_position)),
	_maxCapture(static_cast<int>(block->capture_top)),
	_lastCapture(static_cast<int>(block->capture_last)),
	_patternPosition(static_cast<int>(block->pattern_position)),
	_nextItemLength(static_cast<int>(block->next_item_length))
{
	_match = gcnew MatchData(match, block);
}

CalloutInfo^ CalloutData::Info::get()
{
	if (_calloutInfo == nullptr)
		_calloutInfo = _match->Regex->GetCalloutInfoByPatternPosition(_patternPosition);

	return _calloutInfo;
}
