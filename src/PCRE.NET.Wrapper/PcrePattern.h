
#pragma once

#include "PatternInfoKey.h"
#include "PcrePatternOptions.h"
#include "PcreStudyOptions.h"
#include "MatchOffsets.h"

using namespace System;
using namespace System::Collections::Generic;

namespace PCRE {
	namespace Wrapper {

		public ref class PcrePattern sealed
		{
		public:
			PcrePattern(String^ pattern, PcrePatternOptions options, Nullable<PcreStudyOptions> studyOptions);
			~PcrePattern();
			!PcrePattern();

			bool IsMatch(String^ subject, int startOffset);
			MatchOffsets Match(String^ subject, int startOffset, PcrePatternOptions additionalOptions);

			int GetInfoInt32(PatternInfoKey key);

			property int CaptureCount {
				int get() { return _captureCount; }
			}

			property Dictionary<String^, array<int>^>^ CaptureNames {
				Dictionary<String^, array<int>^>^ get() { return _captureNames; }
			}

		private:
			pcre16* _re;
			pcre16_extra* _extra;
			int _captureCount;
			Dictionary<String^, array<int>^>^ _captureNames;
		};
	}
}
