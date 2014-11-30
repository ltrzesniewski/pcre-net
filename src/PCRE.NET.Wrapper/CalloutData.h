
#pragma once

using namespace System;

namespace PCRE {
	namespace Wrapper {

		ref class MatchResult;

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
			property MatchResult^ Match { MatchResult^ get() { return _match; } }

		internal:
			CalloutData(MatchResult^ match, pcre16_callout_block *block);

		private:
			initonly MatchResult^ _match;
			initonly int _number;
			initonly int _startOffset;
			initonly int _currentOffset;
			initonly int _maxCapture;
			initonly int _lastCapture;
			initonly int _patternPosition;
			initonly int _nextItemLength;
		};

		public enum struct CalloutResult
		{
			Pass = 0,
			Fail = 1,
			NoMatch = PCRE_ERROR_NOMATCH,
			Throw = PCRE_ERROR_CALLOUT
		};

	}
}
