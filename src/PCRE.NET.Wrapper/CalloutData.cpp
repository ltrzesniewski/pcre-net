

#include "stdafx.h"
#include "CalloutData.h"
#include "MatchResult.h"

namespace PCRE {
	namespace Wrapper {

		CalloutData::CalloutData(MatchResult^ match, pcre16_callout_block *block)
		{
			_match = match;
			_number = block->callout_number;
		}

	}
}
