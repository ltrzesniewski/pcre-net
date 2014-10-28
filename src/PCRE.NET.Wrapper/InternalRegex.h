
#pragma once

#include "InfoKey.h"
#include "PatternOptions.h"
#include "StudyOptions.h"
#include "MatchOffsets.h"

using namespace System;
using namespace System::Collections::Generic;

namespace PCRE {
	namespace Wrapper {

		public ref class InternalRegex sealed
		{
		public:
			InternalRegex(String^ pattern, PatternOptions options, Nullable<StudyOptions> studyOptions);
			~InternalRegex();
			!InternalRegex();

			bool IsMatch(String^ subject, int startOffset);
			MatchOffsets Match(String^ subject, int startOffset, PatternOptions additionalOptions);

			int GetInfoInt32(InfoKey key);

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
