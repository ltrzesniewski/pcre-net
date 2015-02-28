
#pragma once

#include "ConfigKey.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public ref class PcreBuild abstract sealed
		{
		public:
			static String^ PcreBuild::GetConfigString(ConfigKey key);
			static Nullable<UInt32> PcreBuild::GetConfigUInt32(ConfigKey key);
		};
	}
}
