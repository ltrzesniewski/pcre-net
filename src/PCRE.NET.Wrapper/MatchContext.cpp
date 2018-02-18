
#include "stdafx.h"
#include "MatchContext.h"
#include "MatchData.h"

using namespace System;
using namespace PCRE::Wrapper;

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

void MatchContext::MatchLimit::set(uint32_t value)
{
	pcre2_set_match_limit(_ctx, value);
}

void MatchContext::DepthLimit::set(uint32_t value)
{
	pcre2_set_depth_limit(_ctx, value);
}

void MatchContext::HeapLimit::set(uint32_t value)
{
	pcre2_set_heap_limit(_ctx, value);
}

void MatchContext::OffsetLimit::set(uint32_t value)
{
	static_assert(sizeof(uint32_t) <= sizeof(PCRE2_SIZE), "Parameter size must fit into PCRE2_SIZE");
	pcre2_set_offset_limit(_ctx, value);
}

static int CalloutCallback(pcre2_callout_block* block, void* data)
{
	if (!data)
		return 0;

	auto ctx = *static_cast<interior_ptr<MatchContext^>>(data);
	if (!ctx->CalloutHandler)
		return 0;

	try
	{
		ctx->Match->CalloutException = nullptr;
		return static_cast<int>(ctx->CalloutHandler->Invoke(gcnew CalloutData(ctx->Match, block)));
	}
	catch (Exception^ ex)
	{
		ctx->Match->CalloutException = ex;
		return PCRE2_ERROR_CALLOUT;
	}
}

void MatchContext::EnableCallout(void* contextPtr)
{
	pcre2_set_callout(_ctx, &CalloutCallback, contextPtr); // contextPtr is a pinned pointer to this
}
