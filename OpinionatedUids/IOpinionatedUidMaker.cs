using System;

namespace OpinionatedUids
{
    /// <summary>
    /// opinionated UID maker interface
    /// </summary> 
    public interface IOpinionatedUidMaker
    {
        /// <summary>
        /// provide a uid transformed as configured, that satisfies all the configured judges
        /// </summary>
        /// <returns>
        /// a uid suitable for use as configured
        /// </returns>
        Guid NewUid();
    }
}