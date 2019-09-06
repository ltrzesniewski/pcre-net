using System;
using System.Runtime.Serialization;

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

        public PcreMatchException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected PcreMatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
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

        public PcreCalloutException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected PcreCalloutException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
