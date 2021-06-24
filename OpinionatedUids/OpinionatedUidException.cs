using System;

namespace OpinionatedUids
{
    /// <summary>
    /// opinionated uid exception
    /// </summary>
    public class OpinionatedUidException : Exception
    {
        /// <summary>
        /// cons
        /// </summary>
        public OpinionatedUidException()
        {
        }

        /// <summary>
        /// cons, with message
        /// </summary>
        /// <param name="message"></param>
        public OpinionatedUidException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// cons, with message and inner
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public OpinionatedUidException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
