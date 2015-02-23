
#include "stdafx.h"
#include "MatchContext.h"
#include "MatchData.h"

namespace PCRE {
	namespace Wrapper {

		int CalloutCallback(pcre2_callout_block* block, void* data);

		MatchContext::MatchContext(MatchData^ matchData)
			: _matchData(matchData)
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

		int CalloutCallback(pcre2_callout_block* block, void* data)
		{
			if (!data)
				return 0;

			auto ctx = *static_cast<interior_ptr<MatchContext^>>(data);
			if (!ctx->OnCallout)
				return 0;

			try
			{
				ctx->Match->CalloutException = nullptr;
				return static_cast<int>(ctx->OnCallout(gcnew CalloutData(ctx->Match, block)));
			}
			catch (Exception^ ex)
			{
				ctx->Match->CalloutException = ex;
				return PCRE2_ERROR_CALLOUT;
			}
		}

		void MatchContext::SetCallout(Func<CalloutData^, CalloutResult>^ callback, void* contextPtr)
		{
			_onCallout = callback;
			pcre2_set_callout(_ctx, &CalloutCallback, contextPtr); // contextPtr is a pinned pointer to this
		}
	}
}
