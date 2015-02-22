
#include "stdafx.h"
#include "MatchContext.h"

namespace PCRE {
	namespace Wrapper {

		MatchContext::MatchContext()
		{
			_ctx = pcre2_match_context_create(nullptr);
		}

		MatchContext::~MatchContext()
		{
			this->!MatchContext();
		}

		MatchContext::!MatchContext()
		{
			if (_ctx)
			{
				pcre2_match_context_free(_ctx);
				_ctx = nullptr;
			}
		}
	}
}
