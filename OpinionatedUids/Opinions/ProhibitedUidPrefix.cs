using System;
using System.Linq;

namespace OpinionatedUids.Opinions
{
    /// <summary>
    /// implement a rule such that uids don't start with a specified prefix
    /// </summary>
    public sealed class ProhibitedUidPrefix : IUidJudge
    {
        private readonly Guid _prefix;
        private readonly int _len;
        private readonly byte[] _buf;

        /// <summary>
        /// construct, given guidform of prefix
        /// </summary>
        /// <param name="prefix">
        /// prefix that we don't want uids to have
        /// </param>
        /// <param name="len">
        /// length of prefix to check
        /// valid range 1 to <see cref="OpinionatedUidConstants.UidSize"/>
        /// </param>
        public ProhibitedUidPrefix(Guid prefix, int len)
        {
            if (len <= 0 || len > OpinionatedUidConstants.UidSize)
            {
                throw new ArgumentOutOfRangeException(nameof(len));
            }

            _prefix = prefix;
            _len = len;
            _buf = _prefix.ToByteArray().Take(_len).ToArray();
        }

        /// <summary>
        /// construct, given buffer form of prefix
        /// </summary>
        /// <param name="prefix">
        /// prefix that we don't want uids to have
        /// valid length 1 to <see cref="OpinionatedUidConstants.UidSize"/>
        /// </param>
        public ProhibitedUidPrefix(byte[] prefix)
        {
            if (prefix is null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }
            if (prefix.Length <= 0 || prefix.Length > OpinionatedUidConstants.UidSize)
            {
                throw new ArgumentOutOfRangeException(nameof(prefix), "prefix array length incorrect");
            }

            _len = prefix.Length;
            _prefix = new Guid(prefix.Concat(new byte[OpinionatedUidConstants.UidSize - _len]).ToArray());
            _buf = _prefix.ToByteArray().Take(_len).ToArray(); // just to be sure we're immutable
        }

        /// <summary>
        /// prefix that we don't want uids to have
        /// </summary>
        public Guid Prefix { get; }

        /// <summary>
        /// length of prefix to check
        /// valid range 1 to <see cref="OpinionatedUidConstants.UidSize"/>
        /// </summary>
        public int Len { get; }


        /// <summary>
        /// ensure uid doesn't match prohibited prefix
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>
        /// true if uid doesn't have prohibited prefix, so we can use it
        /// false if uid DOES have prohibited prefix, so we can't use it
        /// </returns>
        bool IUidJudge.IsUidAlright(Guid uid)
        {
            return !_buf.SequenceEqual(uid.ToByteArray().Take(_len));
        }
    }
}
