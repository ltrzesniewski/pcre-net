
#pragma once

#include "Stdafx.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public ref class MatchContext sealed
		{
		public:
			MatchContext();
			~MatchContext();
			!MatchContext();

		internal:
			property pcre2_match_context* Context {
				pcre2_match_context* get() { return _ctx; }
			}

		private:
			pcre2_match_context* _ctx;
		};
	}
}
