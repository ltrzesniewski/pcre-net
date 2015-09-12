
#pragma once

namespace PCRE {
	namespace Wrapper {

		ref class MatchData;

		public ref class CalloutInfo sealed
		{
		public:
			property int PatternPosition { int get() { return _patternPosition; } }
			property int NextItemLength { int get() { return _nextItemLength; } }
			property int Number { int get() { return _number; } }
			property int StringOffset { int get() { return _stringOffset; } }
			property System::String^ String { System::String^ get() { return _string; } }

		internal:
			CalloutInfo(pcre2_callout_enumerate_block* block);
			static System::Collections::Generic::IList<CalloutInfo^>^ GetCallouts(pcre2_code* re);

		private:
			initonly int _patternPosition;
			initonly int _nextItemLength;
			initonly int _number;
			initonly int _stringOffset;
			initonly System::String^ _string;
		};

	}
}
