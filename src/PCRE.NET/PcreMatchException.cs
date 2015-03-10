using System;
using System.Runtime.Serialization;
using PCRE.Wrapper;

namespace PCRE
{
    public class PcreMatchException : Exception
    {
        public PcreMatchException()
        {
        }

        public PcreMatchException(string message)
            : base(message)
        {
        }

        public PcreMatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PcreMatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        internal static PcreMatchException FromException(MatchException matchException)
        {
            if (matchException.AttemptedMatchData != null && matchException.AttemptedMatchData.ResultCode == MatchResultCode.Callout)
                return new PcreCalloutException(matchException.Message, matchException.InnerException);

            return new PcreMatchException(matchException.Message, matchException.InnerException);
        }
    }

    public class PcreCalloutException : PcreMatchException
    {
        public PcreCalloutException()
        {
        }

        public PcreCalloutException(string message)
            : base(message)
        {
        }

        public PcreCalloutException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PcreCalloutException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
