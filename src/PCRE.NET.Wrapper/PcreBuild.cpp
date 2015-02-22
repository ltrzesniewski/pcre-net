
#include "stdafx.h"
#include "PcreBuild.h"

namespace PCRE {
	namespace Wrapper {

		String^ PcreBuild::GetConfigString(ConfigKey key)
		{
			PCRE2_UCHAR result[256];
			return pcre2_config(static_cast<int>(key), result) >= 0
				? gcnew String(reinterpret_cast<const wchar_t*>(result))
				: nullptr;
		}

		Nullable<Int32> PcreBuild::GetConfigInt32(ConfigKey key)
		{
			Int32 result;
			return pcre2_config(static_cast<int>(key), &result) >= 0
				? Nullable<Int32>(result)
				: Nullable<Int32>();
		}

		Nullable<Int64> PcreBuild::GetConfigInt64(ConfigKey key)
		{
			Int64 result;
			return pcre2_config(static_cast<int>(key), &result) >= 0
				? Nullable<Int64>(result)
				: Nullable<Int64>();
		}

	}
}
