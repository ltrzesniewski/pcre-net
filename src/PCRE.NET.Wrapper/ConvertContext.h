
#pragma once

#include "Stdafx.h"
#include "ConvertOptions.h"

namespace PCRE {
	namespace Wrapper {

		public ref class ConvertContext sealed
		{
		public:
			__clrcall ConvertContext();
			__clrcall ~ConvertContext();
			__clrcall !ConvertContext();

			property uint32_t GlobEscape { void set(uint32_t); }
			property uint32_t GlobSeparator { void set(uint32_t); }

			System::String^ Convert(System::String^ pattern, ConvertOptions options);

		private:
			pcre2_convert_context * _ctx;
		};
	}
}
