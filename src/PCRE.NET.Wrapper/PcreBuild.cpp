
#include "stdafx.h"
#include "PcreBuild.h"

using namespace System;
using namespace PCRE::Wrapper;

String^ PcreBuild::GetConfigString(ConfigKey key)
{
	PCRE2_UCHAR result[256];
	return pcre2_config(static_cast<int>(key), result) >= 0
		? gcnew String(reinterpret_cast<const wchar_t*>(result))
		: nullptr;
}

Nullable<UInt32> PcreBuild::GetConfigUInt32(ConfigKey key)
{
	UInt32 result;
	return pcre2_config(static_cast<int>(key), &result) >= 0
		? result
		: Nullable<UInt32>();
}
