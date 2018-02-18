#include "stdafx.h"
#include "ConvertContext.h"
#include "Support.h"

using namespace System;
using namespace PCRE::Wrapper;

static void CheckError(int errorCode)
{
	if (errorCode != 0)
		throw gcnew ArgumentException(GetPcreErrorMessage(errorCode));
}

ConvertContext::ConvertContext()
{
	_ctx = pcre2_convert_context_create(nullptr);
}

ConvertContext::~ConvertContext()
{
	this->!ConvertContext();
}

ConvertContext::!ConvertContext()
{
	if (_ctx)
	{
		pcre2_convert_context_free(_ctx);
		_ctx = nullptr;
	}
}

System::String^ ConvertContext::Convert(System::String^ pattern, ConvertOptions options)
{
	PCRE2_UCHAR* buffer = nullptr;
	PCRE2_SIZE bufferLength = 0;

	pin_ptr<const PCRE2_UCHAR> pinnedPattern = GetPtrToString(pattern);

	auto errorCode = pcre2_pattern_convert(
		pinnedPattern,
		pattern->Length,
		static_cast<uint32_t>(options),
		&buffer,
		&bufferLength,
		_ctx
	);

	try
	{
		if (errorCode != 0)
			throw gcnew ArgumentException(String::Format("Could not convert pattern '{0}': {1} at offset {2}", pattern, GetPcreErrorMessage(errorCode), bufferLength));

		return gcnew String(reinterpret_cast<const wchar_t*>(buffer), 0, static_cast<int>(bufferLength));
	}
	finally
	{
		if (buffer != nullptr)
			pcre2_converted_pattern_free(buffer);
	}
}

void ConvertContext::GlobEscape::set(uint32_t value)
{
	CheckError(pcre2_set_glob_escape(_ctx, value));
}

void ConvertContext::GlobSeparator::set(uint32_t value)
{
	CheckError(pcre2_set_glob_separator(_ctx, value));
}
