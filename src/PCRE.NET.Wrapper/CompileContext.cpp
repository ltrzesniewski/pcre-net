
#include "stdafx.h"
#include "CompileContext.h"

using namespace System;
using namespace PCRE::Wrapper;

CompileContext::CompileContext(String^ pattern)
{
	if (pattern == nullptr)
		throw gcnew ArgumentNullException("pattern");

	_ctx = pcre2_compile_context_create(nullptr);
	_pattern = pattern;
}

CompileContext::~CompileContext()
{
	this->!CompileContext();
}

CompileContext::!CompileContext()
{
	if (_ctx)
	{
		pcre2_compile_context_free(_ctx);
		_ctx = nullptr;
	}
}

void CompileContext::NewLine::set(PCRE::Wrapper::NewLine value)
{
	pcre2_set_newline(_ctx, static_cast<uint32_t>(value));
}

void CompileContext::BackslashR::set(PCRE::Wrapper::BackslashR value)
{
	pcre2_set_bsr(_ctx, static_cast<uint32_t>(value));
}

void CompileContext::ParensNestLimit::set(uint32_t value)
{
	pcre2_set_parens_nest_limit(_ctx, value);
}

void CompileContext::MaxPatternLength::set(uint32_t value)
{
	static_assert(sizeof(uint32_t) <= sizeof(PCRE2_SIZE), "Parameter size must fit into PCRE2_SIZE");
	pcre2_set_max_pattern_length(_ctx, value);
}

void CompileContext::ExtraCompileOptions::set(PCRE::Wrapper::ExtraCompileOptions value)
{
	pcre2_set_compile_extra_options(_ctx, static_cast<uint32_t>(value));
}
