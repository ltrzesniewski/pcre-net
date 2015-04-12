
#pragma once

#include "Stdafx.h"
#include "CalloutData.h"
#include "PatternOptions.h"

namespace PCRE {
	namespace Wrapper {

		ref class MatchData;

		public ref class MatchContext sealed
		{
		public:
			MatchContext();
			~MatchContext();
			!MatchContext();

			property System::String^ Subject;
			property int StartIndex;
			property PatternOptions AdditionalOptions;
			property CalloutDelegate^ CalloutHandler;
			property uint32_t MatchLimit { void set(uint32_t); }
			property uint32_t RecursionLimit { void set(uint32_t); }

			property uint32_t DfaMaxResults;
			property uint32_t DfaWorkspaceSize;

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
