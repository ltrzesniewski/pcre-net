
#pragma once

#include "CalloutData.h"
#include "MatchResultCode.h"

namespace PCRE {
	namespace Wrapper {

		ref class InternalRegex;

		public ref class MatchData sealed
		{
		public:
			__clrcall MatchData(InternalRegex^ const re, System::String^ subject);
			__clrcall MatchData(InternalRegex^ const re, System::String^ subject, uint32_t oVectorSize);
			__clrcall ~MatchData();
			__clrcall !MatchData();

			property int RawResultCode {
				public: int __clrcall get() { return _rawResultCode; }
				internal: void __clrcall set(int value) { _rawResultCode = value; }
			}

			property MatchResultCode ResultCode {
				public: MatchResultCode __clrcall get() { return _resultCode; }
				internal: void __clrcall set(MatchResultCode value) { _resultCode = value; }
			}

			property InternalRegex^ Regex { InternalRegex^ __clrcall get() { return _re; } }
			property System::String^ Subject { System::String^ __clrcall get() { return _subject; } }
			property System::String^ Mark { System::String^ __clrcall get(); }

			property uint32_t OutputVectorLength { uint32_t __clrcall get() { return static_cast<uint32_t>(_oVectorCount); } }

			[System::Diagnostics::Contracts::Pure]
			int __clrcall GetStartOffset(unsigned int index);

			[System::Diagnostics::Contracts::Pure]
			int __clrcall GetEndOffset(unsigned int index);

		internal:
			__clrcall MatchData(MatchData^ result, pcre2_callout_block *calloutBlock);

			property System::Exception^ CalloutException;

			property pcre2_match_data* Block {
				pcre2_match_data* __clrcall get() { return _matchData; }
			}

		private:
			initonly InternalRegex^ _re;
			initonly System::String^ _subject;
			int _rawResultCode;
			MatchResultCode _resultCode;
			System::String^ _mark;
			PCRE2_SPTR _markPtr;
			pcre2_match_data* _matchData;
			PCRE2_SIZE* _oVector;
			PCRE2_SIZE _oVectorCount;
		};
	}
}
