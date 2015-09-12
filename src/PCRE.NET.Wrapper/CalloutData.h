
#pragma once

namespace PCRE {
	namespace Wrapper {

		ref class MatchData;
		ref class CalloutInfo;

		public ref class CalloutData sealed
		{
		public:
			property int Number { int get() { return _number; } }
			property int StartOffset { int get() { return _startOffset; } }
			property int CurrentOffset { int get() { return _currentOffset; } }
			property int MaxCapture { int get() { return _maxCapture; } }
			property int LastCapture { int get() { return _lastCapture; } }
			property int PatternPosition { int get() { return _patternPosition; } }
			property int NextPatternItemLength { int get() { return _nextItemLength; } }
			property CalloutInfo^ Info { CalloutInfo^ get(); }

			property MatchData^ Match { MatchData^ get() { return _match; } }

		internal:
			CalloutData(MatchData^ match, pcre2_callout_block* block);

		private:
			initonly MatchData^ _match;
			initonly int _number;
			initonly int _startOffset;
			initonly int _currentOffset;
			initonly int _maxCapture;
			initonly int _lastCapture;
			initonly int _patternPosition;
			initonly int _nextItemLength;
			CalloutInfo^ _calloutInfo;
		};

		public enum struct CalloutResult
		{
			Pass = 0,
			Fail = 1,
			NoMatch = PCRE2_ERROR_NOMATCH,
			Throw = PCRE2_ERROR_CALLOUT
		};

		typedef System::Func<CalloutData^, CalloutResult> CalloutDelegate;

	}
}
