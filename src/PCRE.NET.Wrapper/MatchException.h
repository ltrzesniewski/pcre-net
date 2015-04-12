
#pragma once

#include "Stdafx.h"

namespace PCRE {
	namespace Wrapper {

		ref class MatchData;

		public ref class MatchException sealed : System::Exception
		{
		public:
			MatchException(MatchData^ matchData, System::String^ message, System::Exception^ innerException)
				: System::Exception(message, innerException),
				_matchData(matchData)
			{
			}

			property MatchData^ AttemptedMatchData { MatchData^ get() { return _matchData; } }

		private:
			MatchData^ _matchData;
		};

	}
}
