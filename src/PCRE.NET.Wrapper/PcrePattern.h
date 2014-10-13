
#pragma once

#include "PatternInfoKey.h"
#include "PcrePatternOptions.h"
#include "PcreStudyOptions.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public ref class PcrePattern sealed
		{
		public:
			PcrePattern(String^ pattern, PcrePatternOptions options, Nullable<PcreStudyOptions> studyOptions);
			~PcrePattern();
			!PcrePattern();

			bool IsMatch(String^ subject);

			int GetInfoInt32(PatternInfoKey key);

		private:
			pcre16* _re;
			pcre16_extra* _extra;
		};
	}
}
