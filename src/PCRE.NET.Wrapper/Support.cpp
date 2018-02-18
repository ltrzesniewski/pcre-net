
#include "stdafx.h"
#include "Support.h"

using namespace System;
using namespace PCRE::Wrapper;

String^ __clrcall PCRE::Wrapper::GetPcreErrorMessage(int errorCode)
{
	PCRE2_UCHAR16 errorBuffer[256];
	return pcre2_get_error_message(errorCode, errorBuffer, sizeof(errorBuffer)) >= 0
		? gcnew String(reinterpret_cast<const wchar_t*>(errorBuffer))
		: "Unknown error, code: " + errorCode;
}
