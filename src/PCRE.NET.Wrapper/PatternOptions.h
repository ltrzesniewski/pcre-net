
#pragma once

#include "Stdafx.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		[Flags]
		public enum class PatternOptions
		{
			None                       = 0,
			CaseLess                   = PCRE_CASELESS,  
			MultiLine                  = PCRE_MULTILINE,  
			DotAll                     = PCRE_DOTALL,  
			Extended                   = PCRE_EXTENDED,  
			Anchored                   = PCRE_ANCHORED,  
			DollarEndOnly              = PCRE_DOLLAR_ENDONLY,  
			Extra                      = PCRE_EXTRA,  
			NotBol                     = PCRE_NOTBOL,  
			NotEol                     = PCRE_NOTEOL,  
			Ungreedy                   = PCRE_UNGREEDY,  
			NotEmpty                   = PCRE_NOTEMPTY,  
			Utf8                       = PCRE_UTF8,  
			Utf16                      = PCRE_UTF16,  
			Utf32                      = PCRE_UTF32,  
			NoAutoCapture              = PCRE_NO_AUTO_CAPTURE,  
			NoUtf8Check                = PCRE_NO_UTF8_CHECK,  
			NoUtf16Check               = PCRE_NO_UTF16_CHECK,
			NoUtf32Check               = PCRE_NO_UTF32_CHECK,
			AutoCallout                = PCRE_AUTO_CALLOUT,  
			PartialSoft                = PCRE_PARTIAL_SOFT,  
			Partial                    = PCRE_PARTIAL,  
			NeverUtf                   = PCRE_NEVER_UTF,  
			DfaShortest                = PCRE_DFA_SHORTEST,  
			NoAutoPossess              = PCRE_NO_AUTO_POSSESS,  
			DfaRestart                 = PCRE_DFA_RESTART,
			FirstLine                  = PCRE_FIRSTLINE,  
			DupNames                   = PCRE_DUPNAMES,  
			NewLineCr                  = PCRE_NEWLINE_CR,  
			NewLineLf                  = PCRE_NEWLINE_LF,  
			NewLineCrLf                = PCRE_NEWLINE_CRLF,  
			NewLineAny                 = PCRE_NEWLINE_ANY,  
			NewLineAnyCrLf             = PCRE_NEWLINE_ANYCRLF,  
			BsrAnyCrLf                 = PCRE_BSR_ANYCRLF,  
			BsrUnicode                 = PCRE_BSR_UNICODE,  
			JavaScriptCompat           = PCRE_JAVASCRIPT_COMPAT,  
			NoStartOptimize            = PCRE_NO_START_OPTIMIZE,  
			NoStartOptimise            = PCRE_NO_START_OPTIMISE,  
			PartialHard                = PCRE_PARTIAL_HARD,  
			NotEmptyAtStart            = PCRE_NOTEMPTY_ATSTART,  
			Ucp                        = PCRE_UCP
		};

	}
}
