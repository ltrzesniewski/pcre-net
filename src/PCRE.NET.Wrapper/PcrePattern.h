
#pragma once

#include "PatternInfoKey.h"

using namespace System;

namespace PCRE {
	namespace Wrapper {

		public ref class PcrePattern sealed
		{
		public:
			PcrePattern(String^ pattern, int options);
			~PcrePattern();
			!PcrePattern();

			bool IsMatch(String^ subject);

			int GetInfoInt32(PatternInfoKey key);

		private:
			pcre16* _re;
			pcre16_extra* _extra;
		};
	}
}
