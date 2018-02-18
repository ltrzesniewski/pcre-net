#pragma once

#include "Stdafx.h"

namespace PCRE {
	namespace Wrapper {

		inline interior_ptr<const PCRE2_UCHAR> __clrcall GetPtrToString(System::String^ string)
		{
			return reinterpret_cast<interior_ptr<const PCRE2_UCHAR>>(PtrToStringChars(string));
		};

		System::String^ __clrcall GetPcreErrorMessage(int errorCode);

	}
}
