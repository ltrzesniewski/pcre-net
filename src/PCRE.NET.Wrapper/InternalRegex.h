
#pragma once

#include "StudyOptions.h"

using namespace System;
using namespace System::Collections::Generic;

namespace PCRE {
	namespace Wrapper {

		ref class MatchResult;
		enum struct PatternOptions;
		enum struct InfoKey;

		public ref class InternalRegex sealed
		{
		public:
			InternalRegex(String^ pattern, PatternOptions options, Nullable<StudyOptions> studyOptions);
			~InternalRegex();
			!InternalRegex();

			bool IsMatch(String^ subject, int startOffset);
			MatchResult^ Match(String^ subject, int startOffset, PatternOptions additionalOptions);

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
			initonly int _captureCount;
			initonly Dictionary<String^, array<int>^>^ _captureNames;
		};
	}
}
