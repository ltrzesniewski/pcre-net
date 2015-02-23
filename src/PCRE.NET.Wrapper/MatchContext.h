
#pragma once

#include "Stdafx.h"
#include "CalloutData.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		ref class MatchData;

		public ref class MatchContext sealed
		{
		public:
			MatchContext(MatchData^ matchData);
			~MatchContext();
			!MatchContext();

		internal:
			void SetCallout(Func<CalloutData^, CalloutResult>^ callback, void* contextPtr);

			property Func<CalloutData^, CalloutResult>^ OnCallout {
				Func<CalloutData^, CalloutResult>^ get() { return _onCallout; }
			}

			property MatchData^ Match {
				MatchData^ get() { return _matchData; }
			}

			property pcre2_match_context* Context {
				pcre2_match_context* get() { return _ctx; }
			}

		private:
			pcre2_match_context* _ctx;
			MatchData^ _matchData;
			Func<CalloutData^, CalloutResult>^ _onCallout;
		};
	}
}
