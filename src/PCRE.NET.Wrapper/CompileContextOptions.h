
#pragma once

#include "Stdafx.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		[Flags]
		public enum struct CompileContextBackslashROptions
		{
			BsrUnicode = PCRE2_BSR_UNICODE,
			BsrAnyCrLf = PCRE2_BSR_ANYCRLF
		};

	}
}
