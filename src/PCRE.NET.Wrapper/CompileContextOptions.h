
#pragma once

#include "Stdafx.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		[Flags]
		public enum struct CompileContextNewLineOptions
		{
			NewLineCr = PCRE2_NEWLINE_CR,
			NewLineLf = PCRE2_NEWLINE_LF,
			NewLineCrLf = PCRE2_NEWLINE_CRLF,
			NewLineAny = PCRE2_NEWLINE_ANY,
			NewLineAnyCrLf = PCRE2_NEWLINE_ANYCRLF
		};

		[Flags]
		public enum struct CompileContextBackslashROptions
		{
			BsrUnicode = PCRE2_BSR_UNICODE,
			BsrAnyCrLf = PCRE2_BSR_ANYCRLF
		};

	}
}
