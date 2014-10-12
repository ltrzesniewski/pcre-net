
#include "stdafx.h"

#include "PcreBuild.h"

namespace PCRE {
	namespace Wrapper {

		String^ PcreBuild::VersionString::get()
		{
			return gcnew String(pcre16_version());
		}

		String^ PcreBuild::GetConfigString(PcreConfigKey key)
		{
			const char *result;
			return pcre16_config((int)key, &result) && result ? nullptr : gcnew String(result);
		}

		Nullable<Int32> PcreBuild::GetConfigInt32(PcreConfigKey key)
		{
			Int32 result;
			return pcre16_config((int)key, &result) ? Nullable<Int32>() : Nullable<Int32>(result);
		}

		Nullable<Int64> PcreBuild::GetConfigInt64(PcreConfigKey key)
		{
			Int64 result;
			return pcre16_config((int)key, &result) ? Nullable<Int64>() : Nullable<Int64>(result);
		}

	}
}
