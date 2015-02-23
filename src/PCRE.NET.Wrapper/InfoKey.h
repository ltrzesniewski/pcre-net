
#pragma once

#include "Stdafx.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public enum struct InfoKey
		{
			AllOptions                 = PCRE2_INFO_ALLOPTIONS,
			ArgOptions                 = PCRE2_INFO_ARGOPTIONS,
			BackRefMax                 = PCRE2_INFO_BACKREFMAX,
			Bsr                        = PCRE2_INFO_BSR,
			CaptureCount               = PCRE2_INFO_CAPTURECOUNT,
			FirstCodeUnit              = PCRE2_INFO_FIRSTCODEUNIT,
			FirstCodeType              = PCRE2_INFO_FIRSTCODETYPE,
			FirstBitmap                = PCRE2_INFO_FIRSTBITMAP,
			HasCrOrLf                  = PCRE2_INFO_HASCRORLF,
			JChanged                   = PCRE2_INFO_JCHANGED,
			JitSize                    = PCRE2_INFO_JITSIZE,
			LastCodeUnit               = PCRE2_INFO_LASTCODEUNIT,
			LastCodeType               = PCRE2_INFO_LASTCODETYPE,
			MatchEmpty                 = PCRE2_INFO_MATCHEMPTY,
			MatchLimit                 = PCRE2_INFO_MATCHLIMIT,
			MaxLookBehind              = PCRE2_INFO_MAXLOOKBEHIND,
			MinLength                  = PCRE2_INFO_MINLENGTH,
			NameCount                  = PCRE2_INFO_NAMECOUNT,
			NameEntrySize              = PCRE2_INFO_NAMEENTRYSIZE,
			NameTable                  = PCRE2_INFO_NAMETABLE,
			NewLine                    = PCRE2_INFO_NEWLINE,
			RecursionLimit             = PCRE2_INFO_RECURSIONLIMIT,
			Size                       = PCRE2_INFO_SIZE
		};

		public enum struct NewLine
		{
			Cr                         = PCRE2_NEWLINE_CR,
			Lf                         = PCRE2_NEWLINE_LF,
			CrLf                       = PCRE2_NEWLINE_CRLF,
			Any                        = PCRE2_NEWLINE_ANY,
			AnyCrLf                    = PCRE2_NEWLINE_ANYCRLF
		};

	}
}
