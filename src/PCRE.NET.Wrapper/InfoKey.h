
#pragma once

#include "Stdafx.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public enum class InfoKey
		{
			Options                    = PCRE_INFO_OPTIONS,
			Size                       = PCRE_INFO_SIZE,
			CaptureCount               = PCRE_INFO_CAPTURECOUNT,
			BackRefMax                 = PCRE_INFO_BACKREFMAX,
			FirstByte                  = PCRE_INFO_FIRSTBYTE,
			FirstChar                  = PCRE_INFO_FIRSTCHAR,
			FirstTable                 = PCRE_INFO_FIRSTTABLE,
			LastLiteral                = PCRE_INFO_LASTLITERAL,
			NameEntrySize              = PCRE_INFO_NAMEENTRYSIZE,
			NameCount                  = PCRE_INFO_NAMECOUNT,
			NameTable                  = PCRE_INFO_NAMETABLE,
			StudySize                  = PCRE_INFO_STUDYSIZE,
			DefaultTables              = PCRE_INFO_DEFAULT_TABLES,
			PkPartial                  = PCRE_INFO_OKPARTIAL,
			JChanged                   = PCRE_INFO_JCHANGED,
			HasCrOrLf                  = PCRE_INFO_HASCRORLF,
			MinLength                  = PCRE_INFO_MINLENGTH,
			Jit                        = PCRE_INFO_JIT,
			JitSize                    = PCRE_INFO_JITSIZE,
			MaxLookBehind              = PCRE_INFO_MAXLOOKBEHIND,
			FirstCharacter             = PCRE_INFO_FIRSTCHARACTER,
			FirstCharacterFlags        = PCRE_INFO_FIRSTCHARACTERFLAGS,
			RequiredChar               = PCRE_INFO_REQUIREDCHAR,
			RequiredCharFlags          = PCRE_INFO_REQUIREDCHARFLAGS,
			MatchLimit                 = PCRE_INFO_MATCHLIMIT,
			RecursionLimit             = PCRE_INFO_RECURSIONLIMIT,
			MatchEmpty                 = PCRE_INFO_MATCH_EMPTY
		};

	}
}
