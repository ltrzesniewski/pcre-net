
#pragma once

using namespace System;
using namespace System::Diagnostics::Contracts;

namespace PCRE {
	namespace Wrapper {

		public value struct MatchOffsets
		{
		public:
			MatchOffsets(int captureCount);

			static MatchOffsets NoMatch;

			property bool IsMatch {
				public: bool get() { return _offsets != nullptr; }
			}

			[Pure]
			int GetStartOffset(int index);

			[Pure]
			int GetEndOffset(int index);

		internal:
			array<int>^ _offsets;
		};
	}
}
