
#pragma once

#include "CalloutData.h"

using namespace System;
using namespace System::Diagnostics::Contracts;

namespace PCRE {
	namespace Wrapper {

		ref class InternalRegex;

		public ref class MatchResult sealed
		{
		public:
			MatchResult(InternalRegex^ const re);

			property bool IsMatch {
				public: bool get() { return _isMatch; }
				internal: void set(bool value) { _isMatch = value; }
			}

			property String^ Mark { String^ get(); }

			[Pure]
			int GetStartOffset(int index);

			[Pure]
			int GetEndOffset(int index);

		internal:
			property Func<CalloutData^, CalloutResult>^ OnCallout;
			void SetMark(PCRE_UCHAR16 *mark);

			array<int>^ _offsets;			

		private:
			initonly InternalRegex^ _re;
			bool _isMatch;
			String^ _mark;
			PCRE_UCHAR16 *_markPtr;
		};
	}
}
