
#pragma once

#include "Stdafx.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		ref class MatchData;

		public ref class MatchException sealed : Exception
		{
		public:
			MatchException(MatchData^ matchData, String^ message, Exception^ innerException)
				: Exception(message, innerException),
				_matchData(matchData)
			{
			}

			property MatchData^ AttemptedMatchData { MatchData^ get() { return _matchData; } }

		private:
			MatchData^ _matchData;
		};

	}
}
