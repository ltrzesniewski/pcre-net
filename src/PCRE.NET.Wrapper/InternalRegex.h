
#pragma once

#include "JitCompileOptions.h"
#include "CalloutData.h"

using namespace System;
using namespace System::Collections::Generic;

namespace PCRE {
	namespace Wrapper {

		ref class MatchResult;
		enum struct PatternOptions : unsigned int;
		enum struct InfoKey;

		public ref class InternalRegex sealed
		{
		public:
			InternalRegex(String^ pattern, PatternOptions options, JitCompileOptions jitCompileOptions);
			~InternalRegex();
			!InternalRegex();

			MatchData^ Match(String^ subject, int startOffset, PatternOptions additionalOptions, Func<CalloutData^, CalloutResult>^ calloutCallback);

			int GetInfoInt32(InfoKey key);

			property int CaptureCount {
				int get() { return _captureCount; }
			}

			property Dictionary<String^, array<int>^>^ CaptureNames {
				Dictionary<String^, array<int>^>^ get() { return _captureNames; }
			}

		internal:
			property pcre2_code* Code {
				pcre2_code* get() { return _re; }
			}

		private:
			//static InternalRegex();

			pcre2_code* _re;
			initonly int _captureCount;
			initonly Dictionary<String^, array<int>^>^ _captureNames;
		};
	}
}
