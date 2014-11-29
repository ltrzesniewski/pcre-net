
#pragma once

using namespace System;
using namespace System::Diagnostics::Contracts;

namespace PCRE {
	namespace Wrapper {

		public ref class MatchResult
		{
		public:
			MatchResult(int captureCount);

			property bool IsMatch {
				public: bool get() { return _isMatch; }
				internal: void set(bool value) { _isMatch = value; }
			}

			[Pure]
			int GetStartOffset(int index);

			[Pure]
			int GetEndOffset(int index);

		internal:
			bool _isMatch;
			array<int>^ _offsets;
		};
	}
}
