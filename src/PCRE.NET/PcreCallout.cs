using System.Runtime.CompilerServices;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreCallout
    {
        private readonly object _data; // See remark about JIT in PcreRegex

        internal PcreCallout(CalloutData data)
        {
            _data = data;
        }

        private CalloutData InternalData
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return (CalloutData)_data; }
        }

        public int Number { get { return InternalData.Number; } }
    }
}
