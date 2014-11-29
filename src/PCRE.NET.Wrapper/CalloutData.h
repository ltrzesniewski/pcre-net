
#pragma once

namespace PCRE {
	namespace Wrapper {

		ref class MatchResult;

		public enum struct CalloutResult
		{
			Pass = 0,
			Fail = 1,
			NoMatch = PCRE_ERROR_NOMATCH,
			Throw = PCRE_ERROR_CALLOUT
		};

		public ref class CalloutData sealed
		{
		public:
			property int Number { int get() { return _number; } }

		internal:
			CalloutData(MatchResult^ match, pcre16_callout_block *block);

		private:
			initonly MatchResult^ _match;
			initonly int _number;
		};
	}
}
