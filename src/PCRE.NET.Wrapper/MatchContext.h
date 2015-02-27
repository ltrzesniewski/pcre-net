
#pragma once

#include "Stdafx.h"
#include "CalloutData.h"
#include "PatternOptions.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		ref class MatchData;

		public ref class MatchContext sealed
		{
		public:
			MatchContext();
			~MatchContext();
			!MatchContext();

			property String^ Subject;
			property int StartIndex;
			property PatternOptions AdditionalOptions;
			property CalloutDelegate^ CalloutHandler;

		internal:
			void EnableCallout(void* contextPtr);

			property MatchData^ Match;

			property pcre2_match_context* Context {
				pcre2_match_context* get() { return _ctx; }
			}

		private:
			pcre2_match_context* _ctx;
		};
	}
}
