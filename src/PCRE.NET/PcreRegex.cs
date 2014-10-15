using System;
using PCRE.Support;
using PCRE.Wrapper;

namespace PCRE
{
    public sealed class PcreRegex
    {
        private readonly PcrePattern _re;

        public PcrePatternInfo PaternInfo { get; private set; }

        internal int CaptureCount
        {
            get { return _re.CaptureCount; }
        }

        public PcreRegex(string pattern, PcreOptions options = PcreOptions.None)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            _re = new PcrePattern(pattern, options.ToPatternOptions(), options.ToStudyOptions());
            PaternInfo = new PcrePatternInfo(_re);
        }

        public bool IsMatch(string subject)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            return _re.IsMatch(subject);
        }

        public PcreMatch Match(string subject)
        {
            if (subject == null)
                throw new ArgumentNullException("subject");

            var offsets = _re.FirstMatch(subject);
            return offsets.IsMatch
                ? new PcreMatch(this, subject, offsets)
                : null;
        }
    }
}
