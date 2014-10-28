
#pragma once

#include "ConfigKey.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public ref class PcreBuild abstract sealed
		{
		public:
			static property String^ VersionString { String^ get(); };

			static String^ PcreBuild::GetConfigString(ConfigKey key);
			static Nullable<Int32> PcreBuild::GetConfigInt32(ConfigKey key);
			static Nullable<Int64> PcreBuild::GetConfigInt64(ConfigKey key);
		};
	}
}
