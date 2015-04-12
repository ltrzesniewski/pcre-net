
#pragma once

#include "JitCompileOptions.h"
#include "CompileContext.h"
#include "MatchContext.h"

namespace PCRE {
	namespace Wrapper {

		enum struct PatternOptions : unsigned int;
		enum struct InfoKey;

		public ref class InternalRegex sealed
		{
		public:
			InternalRegex(CompileContext^ context);
			~InternalRegex();
			!InternalRegex();

			MatchData^ Match(MatchContext^ context);
			MatchData^ DfaMatch(MatchContext^ context);

			int GetInfoInt32(InfoKey key);

			property int CaptureCount {
				int get() { return _captureCount; }
			}

			property System::Collections::Generic::Dictionary<System::String^, array<int>^>^ CaptureNames {
				System::Collections::Generic::Dictionary<System::String^, array<int>^>^ get() { return _captureNames; }
			}

		internal:
			property pcre2_code* Code {
				pcre2_code* get() { return _re; }
			}

		private:
			pcre2_code* _re;
			initonly int _captureCount;
			initonly System::Collections::Generic::Dictionary<System::String^, array<int>^>^ _captureNames;
		};
	}
}
