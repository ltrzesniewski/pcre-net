using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public class PcrePossibleMatch : PcreMatch
    {
        internal PcrePossibleMatch(MatchResult result)
            : base(result)
        {
            Result = result.ResultCode.ToMatchResult();
        }

        public PcreMatchResult Result { get; private set; }

        public bool IsMatch
        {
            get { return Result == PcreMatchResult.Success; }
        }
    }
}
