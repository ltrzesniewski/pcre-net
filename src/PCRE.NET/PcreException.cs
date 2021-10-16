using System;
using PCRE.Internal;

#pragma warning disable CA1032

namespace PCRE
{
    public class PcreException : Exception
    {
        public PcreErrorCode ErrorCode { get; }

        public PcreException(PcreErrorCode errorCode)
            : this(errorCode, Native.GetErrorMessage((int)errorCode), null)
        {
        }

        public PcreException(PcreErrorCode errorCode, string message)
            : this(errorCode, message, null)
        {
            ErrorCode = errorCode;
        }

        public PcreException(PcreErrorCode errorCode, string message, Exception? innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    public class PcrePatternException : PcreException
    {
        public PcrePatternException(PcreErrorCode errorCode, string message)
            : base(errorCode, message)
        {
        }
    }

    public class PcreMatchException : PcreException
    {
        public PcreMatchException(PcreErrorCode errorCode)
            : base(errorCode)
        {
        }

        protected PcreMatchException(PcreErrorCode errorCode, string message, Exception? innerException)
            : base(errorCode, message, innerException)
        {
        }
    }

    public class PcreCalloutException : PcreMatchException
    {
        public PcreCalloutException(string message, Exception? innerException)
            : base(PcreErrorCode.Callout, message, innerException)
        {
        }
    }
}
