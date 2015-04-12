
#pragma once

#include "ConfigKey.h"

namespace PCRE {
	namespace Wrapper {

		public ref class PcreBuild abstract sealed
		{
		public:
			static System::String^ PcreBuild::GetConfigString(ConfigKey key);
			static System::Nullable<System::UInt32> PcreBuild::GetConfigUInt32(ConfigKey key);
		};
	}
}
